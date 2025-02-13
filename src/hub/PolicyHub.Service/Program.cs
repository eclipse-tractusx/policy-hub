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

using Microsoft.AspNetCore.Authentication;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.DependencyInjection;
using Org.Eclipse.TractusX.PolicyHub.Service.Authentication;
using Org.Eclipse.TractusX.PolicyHub.Service.Controllers;
using Org.Eclipse.TractusX.PolicyHub.Service.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Service;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Web;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Models.Extensions;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Web;
using System.Text.Json.Serialization;

var version = AssemblyExtension.GetApplicationVersion();

await WebApplicationBuildRunner
    .BuildAndRunWebApplicationAsync<Program>(args, "policy-hub", version, ".Hub",
        builder =>
        {
            builder.Services
                .AddTransient<GeneralHttpExceptionMiddleware>()
                .AddSingleton<IErrorMessageService, ErrorMessageService>()
                .AddSingleton<IErrorMessageContainer, ErrorMessageContainer>()
                .AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>()
                .AddEndpointsApiExplorer()
                .AddHubRepositories(builder.Configuration)
                .ConfigureHttpJsonOptions(options =>
                    {
                        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    })
                .Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
        },
        (app, _) =>
        {
            app.UseMiddleware<GeneralHttpExceptionMiddleware>();
            app.MapGroup("/api")
                .WithOpenApi()
                .MapPolicyHubApi();
        }).ConfigureAwait(ConfigureAwaitOptions.None);
