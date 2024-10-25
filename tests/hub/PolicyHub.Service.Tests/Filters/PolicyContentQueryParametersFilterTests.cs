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

namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Filters;

using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.Filters;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class PolicyContentQueryParametersFilterTests
{
    [Fact]
    public async Task InvokeAsync_ValidParameters_ContinuesToNext()
    {
        // Arrange
        var queryParams = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "useCase", UseCaseId.Quality.ToString() },
            { "type", PolicyTypeId.Access.ToString() },
            { "policyName", "UsagePurpose" },
            { "operatorType", OperatorId.Equals.ToString() },
            { "value", "testCalue" }
        });

        var context = new DefaultHttpContext();
        context.Request.Query = queryParams;

        var endpointContext = A.Fake<EndpointFilterInvocationContext>();
        A.CallTo(() => endpointContext.HttpContext).Returns(context);

        var next = A.Fake<EndpointFilterDelegate>();
        A.CallTo(() => next(A<EndpointFilterInvocationContext>.Ignored)).Returns(ValueTask.FromResult<object?>(null));

        var filter = new PolicyContentQueryParametersFilter();

        // Act
        var result = await filter.InvokeAsync(endpointContext, next);

        // Assert
        A.CallTo(() => next(A<EndpointFilterInvocationContext>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task InvokeAsync_InvalidQueryParameter_ThrowsException()
    {
        // Arrange
        var queryParams = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "invalidParam", "InvalidValue" }
        });

        var context = new DefaultHttpContext();
        context.Request.Query = queryParams;

        var endpointContext = A.Fake<EndpointFilterInvocationContext>();
        A.CallTo(() => endpointContext.HttpContext).Returns(context);

        var next = A.Fake<EndpointFilterDelegate>();

        var filter = new PolicyContentQueryParametersFilter();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ControllerArgumentException>(() => filter.InvokeAsync(endpointContext, next).AsTask());
        Assert.Contains("Invalid query parameters", exception.Message);
    }

    [Fact]
    public async Task InvokeAsync_InvalidEnumParameter_ThrowsException()
    {
        // Arrange
        var queryParams = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "useCase", "InvalidUseCase" },
            { "type", "ValidType" },
            { "policyName", "ValidPolicyName" },
            { "operatorType", "ValidOperatorType" },
            { "value", "ValidValue" }
        });

        var context = new DefaultHttpContext();
        context.Request.Query = queryParams;

        var endpointContext = A.Fake<EndpointFilterInvocationContext>();
        A.CallTo(() => endpointContext.HttpContext).Returns(context);

        var next = A.Fake<EndpointFilterDelegate>();

        var filter = new PolicyContentQueryParametersFilter();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ControllerArgumentException>(() => filter.InvokeAsync(endpointContext, next).AsTask());
        Assert.Contains("Invalid parameter value: [useCase: 'InvalidUseCase']", exception.Message);
    }
}
