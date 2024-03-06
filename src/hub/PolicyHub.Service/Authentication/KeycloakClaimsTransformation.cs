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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Json;
using System.Security.Claims;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Authentication;

public class KeycloakClaimsTransformation : IClaimsTransformation
{
    private readonly JwtBearerOptions _options;
    public const string ResourceAccess = "resource_access";

    public KeycloakClaimsTransformation(IOptions<JwtBearerOptions> options)
    {
        _options = options.Value;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = new ClaimsIdentity();
        if (AddRoles(principal, claimsIdentity))
        {
            principal.AddIdentity(claimsIdentity);
        }

        return Task.FromResult(principal);
    }

    private bool AddRoles(ClaimsPrincipal principal, ClaimsIdentity claimsIdentity)
    {
        var resourceAccess = principal.Claims
            .FirstOrDefault(claim => claim is { Type: ResourceAccess, ValueType: "JSON" })?.Value;
        if (resourceAccess == null ||
            !((JsonValue.Parse(resourceAccess) as JsonObject)?.TryGetValue(
                _options.TokenValidationParameters.ValidAudience,
                out var audience) ?? false) ||
            !((audience as JsonObject)?.TryGetValue("roles", out var roles) ?? false) ||
            roles is not JsonArray)
        {
            return false;
        }

        var rolesAdded = false;
        foreach (JsonValue role in roles)
        {
            if (role.JsonType != JsonType.String)
            {
                continue;
            }

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            rolesAdded = true;
        }

        return rolesAdded;
    }
}
