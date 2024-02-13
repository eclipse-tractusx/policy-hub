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

using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Service;
using System.Diagnostics.CodeAnalysis;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Extensions;

[ExcludeFromCodeCoverage]
public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithSwaggerDescription(this RouteHandlerBuilder builder, string summary, string description, params string[] parameterDescriptions) =>
        builder.WithOpenApi(op =>
        {
            op.Summary = summary;
            op.Description = description;
            for (var i = 0; i < parameterDescriptions.Length; i++)
            {
                if (i < op.Parameters.Count)
                {
                    op.Parameters[i].Description = parameterDescriptions[i];
                }
            }

            return op;
        });

    public static RouteHandlerBuilder WithDefaultResponses(this RouteHandlerBuilder builder) =>
        builder
            .Produces(StatusCodes.Status401Unauthorized, typeof(ErrorResponse), contentType: "application/json")
            .Produces(StatusCodes.Status500InternalServerError, typeof(ErrorResponse), "application/json");
}
