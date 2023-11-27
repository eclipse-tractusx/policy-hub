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

using Microsoft.AspNetCore.Mvc;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Models;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.BusinessLogic;
using Org.Eclipse.TractusX.PolicyHub.Service.Extensions;
using Org.Eclipse.TractusX.PolicyHub.Service.Models;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Library;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Controllers;

/// <summary>
/// Creates a new instance of <see cref="PolicyHubController"/>
/// </summary>
public static class PolicyHubController
{
    public static RouteGroupBuilder MapPolicyHubApi(this RouteGroupBuilder group)
    {
        var policyHub = group.MapGroup("/policy-hub");

        policyHub.MapGet("policy-attributes", (IPolicyHubBusinessLogic logic) => logic.GetAttributeKeys())
            .WithSwaggerDescription("Gets the keys for the attributes",
                "Example: GET: api/policy-hub/policy-attributes")
            // .RequireAuthorization()
            .WithDefaultResponses()
            .Produces(StatusCodes.Status200OK, typeof(string), Constants.JsonContentType);

        policyHub.MapGet("policy-types", (PolicyTypeId? type, UseCaseId? useCase, IPolicyHubBusinessLogic logic) => logic.GetPolicyTypes(type, useCase))
            .WithSwaggerDescription("Gets the policy types",
                "Example: GET: api/policy-hub/policy-types",
                "OPTIONAL: Type to filter the response",
                "OPTIONAL: UseCase to filter the response")
            // .RequireAuthorization()
            .WithDefaultResponses()
            .Produces(StatusCodes.Status200OK, typeof(PolicyTypeResponse), Constants.JsonContentType);

        policyHub.MapGet("policy-content",
                (UseCaseId? useCase,
                PolicyTypeId type,
                string credential,
                OperatorId operatorId,
                string? value,
                IPolicyHubBusinessLogic logic) => logic.GetPolicyContentWithFiltersAsync(useCase, type, credential, operatorId, value))
            .WithSwaggerDescription("Gets the content for a specific policy type",
                "Example: GET: api/policy-hub/policy-content",
                "OPTIONAL: The use case",
                "Type of the policy to get the content for",
                "The technical key of the policy",
                "The operator of the left and right operand",
                "OPTIONAL: Value for dynamic or regex operands")
            // .RequireAuthorization()
            .WithDefaultResponses()
            .Produces(StatusCodes.Status200OK, typeof(PolicyResponse), Constants.JsonContentType)
            .Produces(StatusCodes.Status404NotFound, typeof(ErrorResponse), Constants.JsonContentType);

        policyHub.MapPost("policy-content", ([FromBody] PolicyContentRequest requestData, IPolicyHubBusinessLogic logic) => logic.GetPolicyContentAsync(requestData))
            .WithSwaggerDescription("Gets the content for a specific policy type",
                "Example: POST: api/policy-hub/policy-content",
                "Request data with the configuration of the constraints")
            // .RequireAuthorization()
            .WithDefaultResponses()
            .Produces(StatusCodes.Status200OK, typeof(PolicyResponse), Constants.JsonContentType)
            .Produces(StatusCodes.Status404NotFound, typeof(ErrorResponse), Constants.JsonContentType);

        return group;
    }
}
