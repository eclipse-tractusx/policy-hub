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
using Org.Eclipse.TractusX.PolicyHub.Entities.Extensions;

namespace Org.Eclipse.TractusX.PolicyHub.Entities.Tests;

public class PolicyKindExtensionsTests
{
    [Theory]
    [InlineData(PolicyKindId.BusinessPartnerNumber, true)]
    [InlineData(PolicyKindId.Membership, true)]
    [InlineData(PolicyKindId.Framework, true)]
    [InlineData(PolicyKindId.Purpose, false)]
    [InlineData(PolicyKindId.Dismantler, true)]
    public void IsTechnicalEnforced_WithValidData_ReturnsExpected(PolicyKindId policyKindId, bool expectedResult)
    {
        // Act
        var result = policyKindId.IsTechnicalEnforced();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void IsTechnicalEnforced_WithInvalidData_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => ((PolicyKindId)0).IsTechnicalEnforced());

        // Assert
        ex.Message.Should().Be($"PolicyKindId 0 is not supported (Parameter 'policyKindId'){Environment.NewLine}Actual value was 0.");
    }
}
