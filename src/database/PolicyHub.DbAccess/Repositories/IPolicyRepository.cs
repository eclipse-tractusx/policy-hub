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

using Org.Eclipse.TractusX.PolicyHub.DbAccess.Models;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;

namespace Org.Eclipse.TractusX.PolicyHub.DbAccess.Repositories;

public interface IPolicyRepository
{
    IAsyncEnumerable<string> GetAttributeKeys();
    IAsyncEnumerable<PolicyTypeResponse> GetPolicyTypes(PolicyTypeId? type, UseCaseId? useCase);
    Task<(bool Exists, string LeftOperand, (AttributeKeyId? Key, IEnumerable<string> Values) Attributes, string? RightOperandValue)> GetPolicyContentAsync(UseCaseId? useCase, PolicyTypeId type, string credential);
    IAsyncEnumerable<(string TechnicalKey, string LeftOperand, (AttributeKeyId? Key, IEnumerable<string> Values) Attributes, string? RightOperandValue)> GetPolicyForOperandContent(PolicyTypeId type, IEnumerable<string> technicalKeys);
    Task<List<(string TechnicalKey, AttributeKeyId? AttributeKey, IEnumerable<string> Values)>> GetAttributeValuesForTechnicalKeys(PolicyTypeId type, IEnumerable<string> technicalKeys);
}
