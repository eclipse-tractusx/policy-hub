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

public class PolicyAssignedUseCases
{
    private PolicyAssignedUseCases()
    {
        Policy = null!;
        UseCase = null!;
    }

    public PolicyAssignedUseCases(Guid policyId, UseCaseId useCaseId)
        : this()
    {
        PolicyId = policyId;
        UseCaseId = useCaseId;
    }

    public Guid PolicyId { get; set; }

    public UseCaseId UseCaseId { get; set; }

    public virtual Policy? Policy { get; private set; }
    public virtual UseCase? UseCase { get; private set; }
}
