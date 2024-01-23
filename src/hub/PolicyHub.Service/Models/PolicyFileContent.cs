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

using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Models;

public record PolicyResponse(
    [property: JsonPropertyName("content")] PolicyFileContent Content,
    [property: JsonPropertyName("attributes"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IEnumerable<AdditionalAttributes>? AdditionalAttributes
);

public record AdditionalAttributes(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("possibleValues")] IEnumerable<string> PossibleValues
);

public record PolicyFileContent
(
    [property: JsonPropertyName("@context")] IEnumerable<object> Context,
    [property: JsonPropertyName("@type")] string Type,
    [property: JsonPropertyName("@id")] string Id,
    [property: JsonPropertyName("permission")] Permission Permission
);

public record Permission
(
    [property: JsonPropertyName("action")] string Action,
    [property: JsonPropertyName("constraint")] Constraint Constraint
);

public record Constraint
(
    [property: JsonPropertyName("odrl:and"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IEnumerable<Constraint>? AndOperands,
    [property: JsonPropertyName("odrl:or"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IEnumerable<Constraint>? OrOperands,
    [property: JsonPropertyName("leftOperand"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? LeftOperand,
    [property: JsonPropertyName("operator"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Operator,
    [property: JsonPropertyName("rightOperand"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] object? RightOperandValue
);
