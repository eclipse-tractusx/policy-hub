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

using Microsoft.EntityFrameworkCore;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Models;
using Org.Eclipse.TractusX.PolicyHub.Entities;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;

namespace Org.Eclipse.TractusX.PolicyHub.DbAccess.Repositories;

public class PolicyRepository(PolicyHubContext dbContext)
    : IPolicyRepository
{
    public IAsyncEnumerable<string> GetAttributeKeys() =>
        dbContext.AttributeKeys
            .Select(x => x.Label)
            .AsAsyncEnumerable();

    public IAsyncEnumerable<PolicyTypeResponse> GetPolicyTypes(PolicyTypeId? type, UseCaseId? useCase) =>
        dbContext.Policies
            .Where(p =>
                (type == null || p.Types.Any(x => x.Id == type)) &&
                (useCase == null || p.UseCases.Any(x => x.Id == useCase)))
            .Select(p => new PolicyTypeResponse(
                p.TechnicalKey,
                p.Types.Where(t => t.IsActive).Select(t => t.Id),
                p.Description,
                p.UseCases.Where(u => u.IsActive).Select(u => u.Id),
                p.Attributes.Where(a => a.IsActive).Select(a => new PolicyAttributeResponse(a.Key, a.AttributeValue)),
                p.PolicyKind!.TechnicalEnforced
            ))
            .AsAsyncEnumerable();

    public Task<(bool Exists, string LeftOperand, (AttributeKeyId? Key, IEnumerable<string> Values) Attributes, string? RightOperandValue)> GetPolicyContentAsync(UseCaseId? useCase, PolicyTypeId type, string credential) =>
        dbContext.Policies
            .Where(p =>
                p.Types.Any(t => t.IsActive && t.Id == type) &&
                (useCase == null || p.UseCases.Any(x => x.Id == useCase)) &&
                p.TechnicalKey == credential)
            .Select(p => new ValueTuple<bool, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>(
                true,
                p.LeftOperandValue ?? p.TechnicalKey,
                new ValueTuple<AttributeKeyId?, IEnumerable<string>>(p.AttributeKeyId, p.AttributeKey!.PolicyAttributes.Where(pa => pa.IsActive && pa.PolicyId == p.Id && (useCase == null || pa.PolicyAttributeAssignedUseCases.Any(x => x.UseCaseId == useCase))).Select(a => a.AttributeValue)),
                p.PolicyKind!.Configuration!.RightOperandValue
            ))
            .FirstOrDefaultAsync();

    public IAsyncEnumerable<(string TechnicalKey, string LeftOperand, (AttributeKeyId? Key, IEnumerable<string> Values) Attributes, string? RightOperandValue)> GetPolicyForOperandContent(PolicyTypeId type, IEnumerable<string> technicalKeys) =>
        dbContext.Policies
            .Where(p =>
                p.Types.Any(t => t.IsActive && t.Id == type) &&
                technicalKeys.Contains(p.TechnicalKey))
            .Select(p => new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>(
                    p.TechnicalKey,
                    p.LeftOperandValue ?? p.TechnicalKey,
                    new ValueTuple<AttributeKeyId?, IEnumerable<string>>(p.AttributeKeyId, p.AttributeKey!.PolicyAttributes.Where(pa => pa.IsActive && pa.PolicyId == p.Id).Select(a => a.AttributeValue)),
                    p.PolicyKind!.Configuration!.RightOperandValue
                ))
            .AsAsyncEnumerable();

    public Task<List<(string TechnicalKey, AttributeKeyId? AttributeKey, IEnumerable<string> Values)>> GetAttributeValuesForTechnicalKeys(PolicyTypeId type, IEnumerable<string> technicalKeys) =>
        dbContext.Policies
            .Where(p =>
                p.IsActive &&
                p.Types.Any(t => t.IsActive && t.Id == type) &&
                technicalKeys.Contains(p.TechnicalKey))
            .Select(x => new ValueTuple<string, AttributeKeyId?, IEnumerable<string>>(
                x.TechnicalKey,
                x.AttributeKeyId,
                x.Attributes.Select(a => a.AttributeValue)))
            .ToListAsync();
}
