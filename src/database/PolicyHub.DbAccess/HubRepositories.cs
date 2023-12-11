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

using Org.Eclipse.TractusX.PolicyHub.DbAccess.Repositories;
using Org.Eclipse.TractusX.PolicyHub.Entities;
using System.Collections.Immutable;

namespace Org.Eclipse.TractusX.PolicyHub.DbAccess;

public class HubRepositories : IHubRepositories
{
    private readonly PolicyHubContext _dbContext;

    private static readonly IReadOnlyDictionary<Type, Func<PolicyHubContext, object>> Types = new Dictionary<Type, Func<PolicyHubContext, object>> {
        { typeof(IPolicyRepository), context => new PolicyRepository(context) }
    }.ToImmutableDictionary();

    public HubRepositories(PolicyHubContext policyHubContext)
    {
        _dbContext = policyHubContext;
    }

    public RepositoryType GetInstance<RepositoryType>()
    {
        object? repository = default;

        if (Types.TryGetValue(typeof(RepositoryType), out var createFunc))
        {
            repository = createFunc(_dbContext);
        }

        return (RepositoryType)(repository ?? throw new ArgumentException($"unexpected type {typeof(RepositoryType).Name}", nameof(RepositoryType)));
    }
}
