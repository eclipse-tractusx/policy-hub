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

using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.Extensions;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Extensions;

public class JsonGenerationExtensionsTests
{
    [Theory]
    [InlineData(PolicyTypeId.Access, "access")]
    [InlineData(PolicyTypeId.Usage, "use")]
    public void TypeToJsonString_WithValidData_ReturnsExpected(PolicyTypeId policyTypeId, string result)
    {
        // Act
        var jsonString = policyTypeId.TypeToJsonString();

        // Assert
        jsonString.Should().Be(result);
    }

    [Fact]
    public void TypeToJsonString_WithInvalidData_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => ((PolicyTypeId)0).TypeToJsonString());

        // Assert
        ex.Message.Should().Be($"0 is not a valid value (Parameter 'type'){Environment.NewLine}Actual value was 0.");
    }

    [Theory]
    [InlineData(OperatorId.Equals, "eq")]
    [InlineData(OperatorId.In, "in")]
    public void OperatorToJsonString_WithValidData_ReturnsExpected(OperatorId operatorId, string result)
    {
        // Act
        var jsonString = operatorId.OperatorToJsonString();

        // Assert
        jsonString.Should().Be(result);
    }

    [Fact]
    public void OperatorToJsonString_WithInvalidData_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => ((OperatorId)0).OperatorToJsonString());

        // Assert
        ex.Message.Should().Be($"0 is not a valid value (Parameter 'type'){Environment.NewLine}Actual value was 0.");
    }
}
