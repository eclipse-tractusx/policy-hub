/********************************************************************************
 * Copyright (c) 2025 Contributors to the Eclipse Foundation
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

using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Filters;

public abstract class BaseQueryParametersFilter : IEndpointFilter
{
    protected readonly Dictionary<string, QueryParameterType> _queryParameters;

    protected BaseQueryParametersFilter(Dictionary<string, QueryParameterType> queryParameters)
    {
        _queryParameters = queryParameters;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var queryParams = context.HttpContext.Request.Query;

        // Check for required parameters
        EnsureRequiredParameters(queryParams);

        // Validate supported query parameters
        ValidateSupportedQueryParameters(queryParams);

        // Validate enum parameter values
        ValidateEnumParameters(queryParams);

        // Continue with the next filter or action
        return await next(context);
    }

    private void EnsureRequiredParameters(IQueryCollection queryParams)
    {
        var missingRequiredParameters = _queryParameters
            .Where(q => q.Value.IsRequired && GetArgument(queryParams, q.Key) == null)
            .Select(q => q.Key)
            .ToList();

        if (missingRequiredParameters.Count != 0)
        {
            throw new NotFoundException($"Missing required parameters: {string.Join(", ", missingRequiredParameters)}.");
        }
    }

    private void ValidateSupportedQueryParameters(IQueryCollection queryParams)
    {
        var invalidParameters = queryParams
            .Where(q => !_queryParameters.ContainsKey(q.Key))
            .Select(q => $"{q.Key}")
            .ToList();

        if (invalidParameters.Count != 0)
        {
            throw new ControllerArgumentException($"Invalid query parameters: {string.Join(", ", invalidParameters)}. Supported parameters are: {string.Join(", ", _queryParameters.Keys)}.");
        }
    }

    private void ValidateEnumParameters(IQueryCollection queryParams)
    {
        foreach (var param in _queryParameters)
        {
            var paramValue = GetArgument(queryParams, param.Key);
            if (paramValue != null && param.Value.EnumType != null)
            {
                ParamEnumValidator.ThrowIfInvalidEnumValue(param.Value.EnumType, paramValue, param.Key);
            }
        }
    }

    protected static string? GetArgument(IQueryCollection queryParams, string argumentName)
    {
        return queryParams.TryGetValue(argumentName, out var value) ? value.ToString() : null;
    }
}

