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

using Framework.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.DependencyInjection;
using Org.Eclipse.TractusX.PolicyHub.Service.Authentication;
using Org.Eclipse.TractusX.PolicyHub.Service.HealthCheck;
using Org.Eclipse.TractusX.Portal.Backend.Framework.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Extensions;

public static class StartupServiceExtensions
{
    public static IServiceCollection AddDefaultServices<TProgram>(this IServiceCollection services, IConfigurationRoot configuration, string version)
    {
        services.AddCors(options => options.SetupCors(configuration));

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Name = ".Portal";
            options.IdleTimeout = TimeSpan.FromMinutes(10);
        });

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        });

        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c => SwaggerGenConfiguration.SetupSwaggerGen<TProgram>(c, version));

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            configuration.Bind("JwtBearerOptions", options);
            if (!options.RequireHttpsMetadata)
            {
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
            }
        });

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>()
            .AddOptions<JwtBearerOptions>()
            .Bind(configuration.GetSection("JwtBearerOptions"))
            .ValidateOnStart();

        services.AddHealthChecks()
            .AddCheck<JwtBearerConfigurationHealthCheck>("JwtBearerConfiguration", tags: new[] { "keycloak" });

        services.AddHttpContextAccessor();

        services.AddHubRepositories(configuration);
        services.AutoRegister();
        return services;
    }
}
