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

using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;

namespace Org.Eclipse.TractusX.PolicyHub.Entities.Entities;

public class PolicyAttribute
{
    private PolicyAttribute()
    {
        AttributeValue = null!;
    }

    public PolicyAttribute(Guid policyId, AttributeKeyId key, string attributeValue)
        : this()
    {
        PolicyId = policyId;
        Key = key;
        AttributeValue = attributeValue;
    }

    public Guid PolicyId { get; private set; }

    public AttributeKeyId Key { get; private set; }

    public string AttributeValue { get; private set; }

    public bool IsActive { get; set; }

    public virtual Policy? Policy { get; private set; }

    public virtual AttributeKey? AttributeKey { get; private set; }
}
