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

using Org.Eclipse.TractusX.PolicyHub.DbAccess.DependencyInjection;
using Org.Eclipse.TractusX.PolicyHub.Service.Authentication;
using Org.Eclipse.TractusX.PolicyHub.Service.Controllers;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Web;

const string Version = "v2";

WebApplicationBuildRunner
    .BuildAndRunWebApplication<Program, KeycloakClaimsTransformation>(args, "policyHub", Version, ".Hub",
        builder =>
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHubRepositories(builder.Configuration);
        },
        (app, _) =>
        {
            app.MapGroup("/api")
                .WithOpenApi()
                .MapPolicyHubApi();
        },
        null);
