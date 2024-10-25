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
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Filters;

public class PolicyContentQueryParametersFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var queryParams = context.HttpContext.Request.Query;
        var requiredParameters = new[] { "useCase", "type", "policyName", "operatorType", "value" };

        // Validate query parameters
        ValidateQueryParameters(queryParams, requiredParameters);

        // Validate enum parameters
        ValidateEnumParameter<UseCaseId>(queryParams, "useCase");
        ValidateEnumParameter<PolicyTypeId>(queryParams, "type");
        ValidateEnumParameter<OperatorId>(queryParams, "operatorType");

        // Continue with the next filter or action
        return await next(context);
    }

    private static void ValidateQueryParameters(IQueryCollection queryParams, string[] requiredParameters)
    {
        var invalidParameters = queryParams
            .Where(q => !requiredParameters.Contains(q.Key))
            .Select(q => q.Key)
            .ToList();

        if (invalidParameters.Any())
        {
            throw new ControllerArgumentException($"Invalid query parameters: [{string.Join(", ", invalidParameters)}]. Accepted query parameters: [{string.Join(", ", requiredParameters)}].");
        }
    }

    private static void ValidateEnumParameter<TEnum>(IQueryCollection queryParams, string paramName) where TEnum : struct, Enum
    {
        var paramValue = GetArgument(queryParams, paramName);
        if (paramValue != null && !Enum.TryParse<TEnum>(paramValue, true, out _))
        {
            ThrowIfInvalidEnumValue<TEnum>(paramValue, paramName);
        }
    }

    private static void ThrowIfInvalidEnumValue<TEnum>(string? value, string paramName) where TEnum : struct, Enum
    {
        var acceptedValues = string.Join(", ", Enum.GetNames<TEnum>());
        throw new ControllerArgumentException($"Invalid parameter value: [{paramName}: '{value}']. Accepted values: [{acceptedValues}].");
    }

    private static string? GetArgument(IQueryCollection queryParams, string argumentName)
    {
        return queryParams.TryGetValue(argumentName, out var value) ? value.ToString() : null;
    }
}
