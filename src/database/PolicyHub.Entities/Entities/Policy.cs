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

using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;

namespace Org.Eclipse.TractusX.PolicyHub.Entities.Entities;

public class Policy
{
    private Policy()
    {
        TechnicalKey = null!;
        Description = null!;
        Types = new HashSet<PolicyType>();
        Attributes = new HashSet<PolicyAttribute>();
        UseCases = new HashSet<UseCase>();
    }

    public Policy(Guid id, PolicyKindId kindId, string technicalKey, string description, bool isActive)
        : this()
    {
        Id = id;
        KindId = kindId;
        TechnicalKey = technicalKey;
        Description = description;
        IsActive = isActive;
    }

    public Guid Id { get; set; }

    public PolicyKindId KindId { get; set; }

    public string? LeftOperandValue { get; set; }

    public string TechnicalKey { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public AttributeKeyId? AttributeKeyId { get; set; }

    public virtual PolicyKind? PolicyKind { get; set; }

    public ICollection<PolicyType> Types { get; private set; }

    public ICollection<UseCase> UseCases { get; private set; }

    public virtual AttributeKey? AttributeKey { get; private set; }

    public ICollection<PolicyAttribute> Attributes { get; private set; }
}
