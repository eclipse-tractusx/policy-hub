/********************************************************************************
 * Copyright (c) 2023 Contributors to the Eclipse Foundation
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Org.Eclipse.TractusX.PolicyHub.DbAccess;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Models;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Repositories;
using Org.Eclipse.TractusX.PolicyHub.Entities.Entities;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.Extensions;
using Org.Eclipse.TractusX.PolicyHub.Service.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Linq;
using System.Text.RegularExpressions;

namespace Org.Eclipse.TractusX.PolicyHub.Service.BusinessLogic;

public class PolicyHubBusinessLogic(IHubRepositories hubRepositories)
    : IPolicyHubBusinessLogic
{
    public IAsyncEnumerable<string> GetAttributeKeys() =>
        hubRepositories.GetInstance<IPolicyRepository>().GetAttributeKeys();

    public IAsyncEnumerable<PolicyTypeResponse> GetPolicyTypes(PolicyTypeId? type, UseCaseId? useCase) =>
        hubRepositories.GetInstance<IPolicyRepository>().GetPolicyTypes(type, useCase);

    public async Task<PolicyResponse> GetPolicyContentWithFiltersAsync(UseCaseId? useCase, PolicyTypeId type, string credential, OperatorId operatorId, string? value)
    {
        var (exists, leftOperand, attributes, rightOperandValue) = await hubRepositories.GetInstance<IPolicyRepository>().GetPolicyContentAsync(useCase, type, credential).ConfigureAwait(false);
        if (!exists)
        {
            throw new NotFoundException($"Policy for type {type} and technicalKey {credential} does not exists");
        }

        var rightOperands = attributes.Values.Select(a => rightOperandValue != null ? string.Format(rightOperandValue, a) : a);
        if (attributes.Key == null && rightOperandValue == null)
        {
            throw new UnexpectedConditionException("There must be one configured rightOperand value");
        }

        var (rightOperand, additionalAttribute) = attributes.Key != null ?
            GetRightOperand(operatorId, attributes, rightOperands, value, leftOperand, useCase) :
            (rightOperandValue!, null);

        return new PolicyResponse(CreateFileContent(type, operatorId, "cx-policy:" + leftOperand, rightOperand), additionalAttribute == null ? null : Enumerable.Repeat(additionalAttribute, 1));
    }

    private static (object rightOperand, AdditionalAttributes? additionalAttribute) GetRightOperand(OperatorId operatorId, (AttributeKeyId? Key, IEnumerable<string> Values) attributes, IEnumerable<string> rightOperands, string? value, string leftOperand, UseCaseId? useCase) =>
        attributes.Key switch
        {
            AttributeKeyId.DynamicValue => (value ?? "{dynamicValue}", null),
            AttributeKeyId.Regex => (GetRegexValue(attributes, value), null),
            _ => operatorId == OperatorId.Equals
                ? rightOperands.Count() > 1 ?
                    ($"@{leftOperand}{(useCase != null ?
                        useCase.ToString().Insert(0, ".") :
                        string.Empty)}-{attributes.Key}",
                        new AdditionalAttributes($"@{leftOperand}{(useCase != null ?
                            useCase.ToString().Insert(0, ".") :
                            string.Empty)}-{attributes.Key}", rightOperands)) :
                    (rightOperands.Single(), null)
                : (rightOperands, null)
        };

    private static object GetRegexValue((AttributeKeyId? Key, IEnumerable<string> Values) attributes, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ControllerArgumentException("you must provide a value for the regex", nameof(value));
        }

        if (attributes.Values.Count() != 1)
        {
            throw new UnexpectedConditionException("There should only be one regex pattern defined");
        }

        if (!Regex.IsMatch(value, attributes.Values.Single(), RegexOptions.Compiled, TimeSpan.FromSeconds(1)))
        {
            throw new ControllerArgumentException($"The provided value {value} does not match the regex pattern {attributes.Values.Single()}", nameof(value));
        }

        return value;
    }

    private static PolicyFileContent CreateFileContent(PolicyTypeId type, OperatorId operatorId, string leftOperand, object rightOperand) =>
        new(
            GetContext(),
            "Offer",
            "....",
            new Permission(
                type.TypeToJsonString(),
                new Constraint(
                    null,
                    null,
                    leftOperand,
                    operatorId.OperatorToJsonString(),
                    rightOperand
                )
            ));

    public async Task<PolicyResponse> GetPolicyContentAsync(PolicyContentRequest requestData)
    {
        if (requestData.PolicyType == PolicyTypeId.Usage && requestData.ConstraintOperand == ConstraintOperandId.Or)
        {
            throw new ControllerArgumentException($"The support of OR constraintOperand for Usage constraints are not supported for now");
        }

        if (requestData.PolicyType == PolicyTypeId.Access && requestData.ConstraintOperand == ConstraintOperandId.And && requestData.Constraints.Any(x => x.Key == "BusinessPartnerNumber" && (x.Value!.Split(",").Count() > 1)))
        {
            throw new ControllerArgumentException($"Only a single value BPNL is allowed with an AND constraint");
        }

        if (requestData.Constraints.Any(x => x.Key == "BusinessPartnerNumber") && !requestData.Constraints.Any(x => x.Operator == OperatorId.Equals))
        {
            throw new ControllerArgumentException($"The operator for BPNLs should always be Equals");
        }

        if (requestData.PolicyType == PolicyTypeId.Usage && requestData.Constraints.Any(x => x.Key == "BusinessPartnerNumber" && (x.Value!.Split(",").Count() > 1)))
        {
            throw new ControllerArgumentException($"For usage policies only a single BPNL is allowed");
        }

        var keyCounts = requestData.Constraints
            .GroupBy(pair => pair.Key)
            .ToDictionary(group => group.Key, group => group.Count());
        var multipleDefinedKey = keyCounts.Where(x => x.Value != 1);
        if (multipleDefinedKey.Any())
        {
            throw new ControllerArgumentException($"Keys {string.Join(",", multipleDefinedKey.Select(x => x.Key).Distinct())} have been defined multiple times");
        }

        var technicalKeys = requestData.Constraints.Select(x => x.Key);
        var attributeValuesForTechnicalKeys = await hubRepositories.GetInstance<IPolicyRepository>().GetAttributeValuesForTechnicalKeys(requestData.PolicyType, technicalKeys).ConfigureAwait(false);
        if (technicalKeys.Except(attributeValuesForTechnicalKeys.Select(a => a.TechnicalKey)).Any())
        {
            throw new ControllerArgumentException($"Policy for type {requestData.PolicyType} and requested technicalKeys does not exists. TechnicalKeys {string.Join(",", attributeValuesForTechnicalKeys.Select(x => x.TechnicalKey))} are allowed");
        }

        IEnumerable<(string TechnicalKey, IEnumerable<string> Values)> keyValues = requestData.Constraints.GroupBy(x => x.Key).Select(x => new ValueTuple<string, IEnumerable<string>>(x.Key, x.Where(y => y.Value != null).SelectMany(y => y.Value!.Split(","))));
        IEnumerable<(string TechnicalKey, IEnumerable<string> Values)> missingValues = keyValues
            .Join(attributeValuesForTechnicalKeys, secondItem => secondItem.TechnicalKey, firstItem => firstItem.TechnicalKey,
                (secondItem, firstItem) => new { secondItem, firstItem })
            .Select(t => new { t, missing = t.secondItem.Values.Except(t.firstItem.Values) })
            .Where(t => t.missing.Any())
            .Select(t => (Key: t.t.secondItem.TechnicalKey, MissingValues: t.missing));

        var attributesToIgnore = new[] { AttributeKeyId.Regex, AttributeKeyId.DynamicValue };
        var technicalKeysToIgnore = attributeValuesForTechnicalKeys.Where(x => x.AttributeKey != null && attributesToIgnore.Any(y => y == x.AttributeKey)).Select(x => x.TechnicalKey);
        var invalidValues = missingValues.Select(x => x.TechnicalKey).Except(technicalKeysToIgnore);
        if (invalidValues.Any())
        {
            var x = missingValues.Where(x => invalidValues.Contains(x.TechnicalKey)).Select(x =>
                $"Key: {x.TechnicalKey}, invalid values: {string.Join(',', x.Values)}");
            throw new ControllerArgumentException($"Invalid values set for {string.Join(',', x)}");
        }

        var policies = await hubRepositories.GetInstance<IPolicyRepository>().GetPolicyForOperandContent(requestData.PolicyType, technicalKeys).ToListAsync().ConfigureAwait(false);
        if (policies.Count != requestData.Constraints.Count())
        {
            throw new NotFoundException($"Policy for type {requestData.PolicyType} and technicalKeys {string.Join(",", technicalKeys.Except(policies.Select(x => x.TechnicalKey)))} does not exists");
        }

        var constraints = new List<Constraint>();
        List<AdditionalAttributes>? additionalAttributes = null;
        foreach (var policy in policies)
        {
            var constraint = requestData.Constraints.Single(x => x.Key == policy.TechnicalKey);
            var rightOperands = policy.Attributes.Values.Select(a => policy.RightOperandValue != null ? string.Format(policy.RightOperandValue, a) : a);
            if (policy.Attributes.Key == null && policy.RightOperandValue == null)
            {
                throw new UnexpectedConditionException("There must be one configured rightOperand value");
            }

            if (constraint.Value != null)
            {
                foreach (var keyValue in constraint.Value.Split(","))
                {
                    var (rightOperand, additionalAttribute) = policy.Attributes.Key != null ?
                                    GetRightOperand(constraint.Operator, policy.Attributes, rightOperands, keyValue, policy.LeftOperand, null) :
                                    (policy.RightOperandValue!, null);
                    if (additionalAttribute != null)
                    {
                        additionalAttributes ??= new List<AdditionalAttributes>();
                        additionalAttributes.Add(additionalAttribute);
                    }

                    constraints.Add(new Constraint(null,
                        null,
                        "cx-policy:" + policy.LeftOperand,
                        constraint.Operator.OperatorToJsonString(),
                        rightOperand
                    ));

                }
            }
            else
            {
                var (rightOperand, additionalAttribute) = policy.Attributes.Key != null ?
                                    GetRightOperand(constraint.Operator, policy.Attributes, rightOperands, null, policy.LeftOperand, null) :
                                    (policy.RightOperandValue!, null);
                if (additionalAttribute != null)
                {
                    additionalAttributes ??= new List<AdditionalAttributes>();
                    additionalAttributes.Add(additionalAttribute);
                }

                constraints.Add(new Constraint(null,
                    null,
                    "cx-policy:" + policy.LeftOperand,
                    constraint.Operator.OperatorToJsonString(),
                    rightOperand
                ));
            }
        }

        var permission = new Permission(
            requestData.PolicyType.TypeToJsonString(),
            new Constraint(
                requestData.ConstraintOperand == ConstraintOperandId.And ? constraints : null,
                requestData.ConstraintOperand == ConstraintOperandId.Or ? constraints : null,
                null,
                null,
                null));
        var content = new PolicyFileContent(
            GetContext(),
            "Offer",
            "....",
            permission);

        return new PolicyResponse(content, additionalAttributes);
    }

    private static IEnumerable<object> GetContext() => new object[] { "https://www.w3.org/ns/odrl.jsonld", new { cx = "https://w3id.org/catenax/v0.0.1/ns/" } };
}
