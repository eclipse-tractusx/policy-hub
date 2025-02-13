/********************************************************************************
 * Copyright (c) 2025 Contributors to the Eclipse Foundation
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

using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Service;
using System.Collections.Immutable;

namespace Org.Eclipse.TractusX.PolicyHub.Service.ErrorHandling;

public class ErrorMessageContainer : IErrorMessageContainer
{
    private static readonly IReadOnlyDictionary<int, string> Messages = ImmutableDictionary.CreateRange<int, string>([
        new((int)PolicyErrors.POLICY_NOT_EXIST, "Policy for type {type} and technicalKey {credential} does not exists"),
        new((int)PolicyErrors.NO_RIGHT_OPERAND_CONFIGURED, "There must be one configured rightOperand value"),
        new((int)PolicyErrors.INVALID_VALUES, "Invalid values [{value}] set for key {leftOperand}. Possible values [{possibleValues}]"),
        new((int)PolicyErrors.NO_VALUE_FOR_REGEX, "you must provide a value for the regex"),
        new((int)PolicyErrors.MULTIPLE_REGEX_DEFINED, "There should only be one regex pattern defined"),
        new((int)PolicyErrors.VALUE_DOES_NOT_MATCH_REGEX, "The provided value {value} does not match the regex pattern {values}"),
        new((int)PolicyErrors.OR_WITH_USAGE, "The support of OR constraintOperand for Usage constraints are not supported for now"),
        new((int)PolicyErrors.SINGLE_VALUE_BPNL_CONSTRAINT, "Only a single value BPNL is allowed with an AND constraint"),
        new((int)PolicyErrors.BPNL_WRONG_OPERATOR, "The operator for BPNLs should always be Equals"),
        new((int)PolicyErrors.USAGE_MULTIPLE_BPNL, "For usage policies only a single BPNL is allowed"),
        new((int)PolicyErrors.KEY_DEFINED_MULTIPLE_TIMES, "Keys {keys} have been defined multiple times"),
        new((int)PolicyErrors.POLICY_NOT_EXISTS_FOR_TECHNICAL_KEYS, "Policy for type {policyType} and requested technicalKeys does not exists. TechnicalKeys {technicalKeys} are allowed"),
        new((int)PolicyErrors.INVALID_VALUES_SET, "Invalid values set for {values}"),
        new((int)PolicyErrors.RIGHT_OPERAND_NOT_CONFIGURED, "There must be one configured rightOperand value")
    ]);

    public Type Type { get => typeof(PolicyErrors); }
    public IReadOnlyDictionary<int, string> MessageContainer { get => Messages; }
}

public enum PolicyErrors
{
    POLICY_NOT_EXIST,
    NO_RIGHT_OPERAND_CONFIGURED,
    INVALID_VALUES,
    NO_VALUE_FOR_REGEX,
    MULTIPLE_REGEX_DEFINED,
    VALUE_DOES_NOT_MATCH_REGEX,
    OR_WITH_USAGE,
    SINGLE_VALUE_BPNL_CONSTRAINT,
    BPNL_WRONG_OPERATOR,
    USAGE_MULTIPLE_BPNL,
    KEY_DEFINED_MULTIPLE_TIMES,
    POLICY_NOT_EXISTS_FOR_TECHNICAL_KEYS,
    INVALID_VALUES_SET,
    RIGHT_OPERAND_NOT_CONFIGURED
}
