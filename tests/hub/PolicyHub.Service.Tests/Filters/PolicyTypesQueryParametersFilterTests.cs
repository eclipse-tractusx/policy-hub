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

namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Filters;

using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Org.Eclipse.TractusX.PolicyHub.Service.Filters;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class PolicyTypesQueryParametersFilterTests
{
    [Fact]
    public async Task InvokeAsync_ValidParameters_ContinuesToNext()
    {
        // Arrange
        var queryParams = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "useCase", "PCF" },
            { "type", "Usage" }
        });

        var context = new DefaultHttpContext();
        context.Request.Query = queryParams;

        var endpointContext = A.Fake<EndpointFilterInvocationContext>();
        A.CallTo(() => endpointContext.HttpContext).Returns(context);

        var next = A.Fake<EndpointFilterDelegate>();
        A.CallTo(() => next(A<EndpointFilterInvocationContext>.Ignored)).Returns(ValueTask.FromResult<object?>(null).AsTask());

        var filter = new PolicyTypesQueryParametersFilter();

        // Act
        await filter.InvokeAsync(endpointContext, next);

        // Assert
        A.CallTo(() => next(A<EndpointFilterInvocationContext>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task InvokeAsync_NoParametersProvided_ContinuesToNext()
    {
        // Arrange
        var queryParams = new QueryCollection(new Dictionary<string, StringValues>
        {
        });

        var context = new DefaultHttpContext();
        context.Request.Query = queryParams;

        var endpointContext = A.Fake<EndpointFilterInvocationContext>();
        A.CallTo(() => endpointContext.HttpContext).Returns(context);

        var next = A.Fake<EndpointFilterDelegate>();
        A.CallTo(() => next(A<EndpointFilterInvocationContext>.Ignored)).Returns(ValueTask.FromResult<object?>(null).AsTask());

        var filter = new PolicyTypesQueryParametersFilter();

        // Act
        await filter.InvokeAsync(endpointContext, next);

        // Assert
        A.CallTo(() => next(A<EndpointFilterInvocationContext>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task InvokeAsync_Missing_And_InvalidQueryParameter_ThrowsException()
    {
        // Arrange
        var queryParams = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "invalidParam", "InvalidValue" }
        });

        var context = new DefaultHttpContext();
        context.Request.Query = queryParams;

        var endpointContext = A.Fake<EndpointFilterInvocationContext>();
        A.CallTo(() => endpointContext.HttpContext).Returns(context);

        var next = A.Fake<EndpointFilterDelegate>();

        var filter = new PolicyTypesQueryParametersFilter();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ControllerArgumentException>(() => filter.InvokeAsync(endpointContext, next).AsTask());
        Assert.Equal("Invalid query parameters: invalidParam. Supported parameters are: useCase, type.", exception.Message);
    }

    [Fact]
    public async Task InvokeAsync_InvalidQueryParameters_ThrowsException()
    {
        // Arrange
        var queryParams = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "useCase", "PCF" },
            { "type", "Usage" },
            { "invalidParam1", "InvalidValue1" },
            { "invalidParam2", "InvalidValue2" }
        });

        var context = new DefaultHttpContext();
        context.Request.Query = queryParams;

        var endpointContext = A.Fake<EndpointFilterInvocationContext>();
        A.CallTo(() => endpointContext.HttpContext).Returns(context);

        var next = A.Fake<EndpointFilterDelegate>();

        var filter = new PolicyTypesQueryParametersFilter();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ControllerArgumentException>(() => filter.InvokeAsync(endpointContext, next).AsTask());
        Assert.Equal("Invalid query parameters: invalidParam1, invalidParam2. Supported parameters are: useCase, type.", exception.Message);
    }

    [Fact]
    public async Task InvokeAsync_InvalidEnumParameter_ThrowsException()
    {
        // Arrange
        var queryParams = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "useCase", "InvalidUseCase" },
            { "type", "Usage" }
        });

        var context = new DefaultHttpContext();
        context.Request.Query = queryParams;

        var endpointContext = A.Fake<EndpointFilterInvocationContext>();
        A.CallTo(() => endpointContext.HttpContext).Returns(context);

        var next = A.Fake<EndpointFilterDelegate>();

        var filter = new PolicyTypesQueryParametersFilter();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ControllerArgumentException>(() => filter.InvokeAsync(endpointContext, next).AsTask());
        Assert.Equal("Invalid value 'InvalidUseCase' for parameter 'useCase'. Accepted values are: Traceability, Quality, PCF, Behavioraltwin, Businesspartner, CircularEconomy, DemandCapacity.", exception.Message);
    }
}
