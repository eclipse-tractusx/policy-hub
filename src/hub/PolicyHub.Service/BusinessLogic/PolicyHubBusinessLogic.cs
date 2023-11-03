/********************************************************************************
 * Copyright (c) 2021, 2023 Contributors to the Eclipse Foundation
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
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.Extensions;
using Org.Eclipse.TractusX.PolicyHub.Service.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Library;
using System.Text.RegularExpressions;

namespace Org.Eclipse.TractusX.PolicyHub.Service.BusinessLogic;

public class PolicyHubBusinessLogic : IPolicyHubBusinessLogic
{
    private readonly IHubRepositories _hubRepositories;

    public PolicyHubBusinessLogic(IHubRepositories hubRepositories)
    {
        _hubRepositories = hubRepositories;
    }

    public IAsyncEnumerable<string> GetAttributeKeys() =>
        _hubRepositories.GetInstance<IPolicyRepository>().GetAttributeKeys();

    public IAsyncEnumerable<PolicyTypeResponse> GetPolicyTypes(PolicyTypeId? type, UseCaseId? useCase) =>
        _hubRepositories.GetInstance<IPolicyRepository>().GetPolicyTypes(type, useCase);

    public async Task<PolicyResponse> GetPolicyContentAsync(UseCaseId? useCase, PolicyTypeId type, string credential, OperatorId operatorId, string? value)
    {
        var (exists, leftOperand, attributes, rightOperandValue) = await _hubRepositories.GetInstance<IPolicyRepository>().GetPolicyContentAsync(useCase, type, credential).ConfigureAwait(false);
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
            GetRightOperand(operatorId, attributes, rightOperands, value, leftOperand) :
            (rightOperandValue!, null);

        return new PolicyResponse(CreateFileContent(type, operatorId, leftOperand, rightOperand), additionalAttribute == null ? null : Enumerable.Repeat(additionalAttribute, 1));
    }

    private static (object rightOperand, AdditionalAttributes? additionalAttribute) GetRightOperand(OperatorId operatorId, (AttributeKeyId? Key, IEnumerable<string> Values) attributes, IEnumerable<string> rightOperands, string? value, string leftOperand) =>
        attributes.Key switch
        {
            AttributeKeyId.DynamicValue => (value ?? "{dynamicValue}", null),
            AttributeKeyId.Regex => (GetRegexValue(attributes, value), null),
            _ => operatorId == OperatorId.Equals
                ? rightOperands.Count() > 1 ? ($"@{leftOperand}-{attributes.Key}", new AdditionalAttributes($"@{leftOperand}-{attributes.Key}", rightOperands)) : (rightOperands.Single(), null)
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

        if (!Regex.IsMatch(value, attributes.Values.Single()))
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
        var keyCounts = requestData.Constraints
            .GroupBy(pair => pair.Key)
            .ToDictionary(group => group.Key, group => group.Count());
        var multipleDefinedKey = keyCounts.Where(x => x.Value != 1);
        if (multipleDefinedKey.Any())
        {
            throw new ControllerArgumentException($"Keys {multipleDefinedKey.Select(x => x.Key)} have been defined multiple times");
        }

        var policies = await _hubRepositories.GetInstance<IPolicyRepository>().GetPolicyForOperandContent(requestData.PolicyType, requestData.Constraints.Select(x => x.Key)).ToListAsync().ConfigureAwait(false);
        if (policies.Count != requestData.Constraints.Count())
        {
            throw new NotFoundException($"Policy for type {requestData.PolicyType} and technicalKeys {requestData.Constraints.Select(x => x.Key).Except(policies.Select(x => x.TechnicalKey))} does not exists");
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

            var (rightOperand, additionalAttribute) = policy.Attributes.Key != null ?
                GetRightOperand(constraint.Operator, policy.Attributes, rightOperands, constraint.Value, policy.LeftOperand) :
                (policy.RightOperandValue!, null);
            if (additionalAttribute != null)
            {
                additionalAttributes ??= new List<AdditionalAttributes>();
                additionalAttributes.Add(additionalAttribute);
            }

            constraints.Add(new Constraint(null,
                null,
                policy.LeftOperand,
                constraint.Operator.OperatorToJsonString(),
                rightOperand
            ));
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

    private static object[] GetContext() => new object[] { "https://www.w3.org/ns/odrl.jsonld", new { cx = "https://w3id.org/catenax/v0.0.1/ns/" } };
}
