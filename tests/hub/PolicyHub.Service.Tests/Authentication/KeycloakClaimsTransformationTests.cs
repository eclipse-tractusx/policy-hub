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

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.Eclipse.TractusX.PolicyHub.Service.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Authentication;

public class KeycloakClaimsTransformationTests
{
    private readonly KeycloakClaimsTransformation _sut;

    public KeycloakClaimsTransformationTests()
    {
        var options = Options.Create(new JwtBearerOptions
        {
            TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = "validAudience"
            }
        });
        _sut = new KeycloakClaimsTransformation(options);
    }

    [Fact]
    public async Task TransformAsync_WithoutRoles_ReturnsExpected()
    {
        // Arrange
        var identity = new ClaimsIdentity(Enumerable.Repeat(new Claim(ClaimTypes.Email, "test@mail.com"), 1));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await _sut.TransformAsync(principal).ConfigureAwait(false);

        // Assert
        result.Claims.Should().ContainSingle()
            .And.Satisfy(x => x.Type == ClaimTypes.Email && x.Value == "test@mail.com");
    }

    [Fact]
    public async Task TransformAsync_WithRoles_ReturnsExpected()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new { validAudience = new { roles = Enumerable.Repeat("testRole", 1) } });
        var identity = new ClaimsIdentity(Enumerable.Repeat(new Claim(KeycloakClaimsTransformation.ResourceAccess, json, "JSON"), 1));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await _sut.TransformAsync(principal).ConfigureAwait(false);

        // Assert
        result.Claims.Should().HaveCount(2).And.Satisfy(
            x => x.Type == KeycloakClaimsTransformation.ResourceAccess && x.Value == json,
            x => x.Type == ClaimTypes.Role && x.Value == "testRole");
    }

    [Fact]
    public async Task TransformAsync_WithIntRole_ReturnsExpected()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new { validAudience = new { roles = Enumerable.Repeat(1, 1) } });
        var identity = new ClaimsIdentity(Enumerable.Repeat(new Claim(KeycloakClaimsTransformation.ResourceAccess, json, "JSON"), 1));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await _sut.TransformAsync(principal).ConfigureAwait(false);

        // Assert
        result.Claims.Should().ContainSingle()
            .And.Satisfy(x => x.Type == KeycloakClaimsTransformation.ResourceAccess && x.Value == json);
    }
}
