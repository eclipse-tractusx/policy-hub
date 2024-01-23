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

using Org.Eclipse.TractusX.PolicyHub.DbAccess.Repositories;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Tests.Setup;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Xunit.Extensions.AssemblyFixture;

namespace Org.Eclipse.TractusX.PolicyHub.DbAccess.Tests;

/// <summary>
/// Tests the functionality of the <see cref="PolicyRepository"/>
/// </summary>
public class PolicyRepositoryTests : IAssemblyFixture<TestDbFixture>
{
    private readonly TestDbFixture _dbTestDbFixture;

    public PolicyRepositoryTests(TestDbFixture testDbFixture)
    {
        _dbTestDbFixture = testDbFixture;
    }

    #region GetPolicyTypes

    [Fact]
    public async Task GetAttributeKeys_ReturnsExpectedResult()
    {
        // Arrange
        var sut = await CreateSut().ConfigureAwait(false);

        // Act
        var result = await sut.GetAttributeKeys().ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().NotBeEmpty().And.HaveCount(5).And.Satisfy(
            x => x == "Regex",
            x => x == "Static",
            x => x == "DynamicValue",
            x => x == "Brands",
            x => x == "Version");
    }

    #endregion

    #region GetPolicyTypes

    [Fact]
    public async Task GetPolicyTypes_ReturnsExpectedResult()
    {
        // Arrange
        var sut = await CreateSut().ConfigureAwait(false);

        // Act
        var result = await sut.GetPolicyTypes(null, null).ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().NotBeEmpty().And.HaveCount(11).And.Satisfy(
            x => x.TechnicalKey == "BusinessPartnerNumber",
            x => x.TechnicalKey == "Membership",
            x => x.TechnicalKey == "FrameworkAgreement.traceability",
            x => x.TechnicalKey == "FrameworkAgreement.quality",
            x => x.TechnicalKey == "FrameworkAgreement.pcf",
            x => x.TechnicalKey == "FrameworkAgreement.behavioraltwin",
            x => x.TechnicalKey == "purpose.trace.v1.TraceBattery",
            x => x.TechnicalKey == "purpose.trace.v1.aspects",
            x => x.TechnicalKey == "companyRole.dismantler",
            x => x.TechnicalKey == "purpose.trace.v1.qualityanalysis",
            x => x.TechnicalKey == "purpose"
        );
    }

    [Fact]
    public async Task GetPolicyTypes_WithTypeFilter_ReturnsExpectedResult()
    {
        // Arrange
        var sut = await CreateSut().ConfigureAwait(false);

        // Act
        var result = await sut.GetPolicyTypes(PolicyTypeId.Access, null).ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().NotBeEmpty().And.HaveCount(3).And.Satisfy(
            x => x.TechnicalKey == "BusinessPartnerNumber" && x.Attribute.Count() == 1 && x.Type.Count() == 2 && x.UseCase.Count() == 5,
            x => x.TechnicalKey == "Membership" && x.Attribute.Count() == 1 && x.Type.Count() == 2 && x.UseCase.Count() == 5,
            x => x.TechnicalKey == "companyRole.dismantler" && x.Attribute.Count() == 3 && x.Type.Count() == 2 && x.UseCase.Count() == 5
        );
    }

    [Fact]
    public async Task GetPolicyTypes_WithUseCase_ReturnsExpectedResult()
    {
        // Arrange
        var sut = await CreateSut().ConfigureAwait(false);

        // Act
        var result = await sut.GetPolicyTypes(null, UseCaseId.Sustainability).ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().NotBeEmpty().And.HaveCount(3).And.Satisfy(
            x => x.TechnicalKey == "BusinessPartnerNumber" && x.Attribute.Count() == 1 && x.Type.Count() == 2 && x.UseCase.Count() == 5,
            x => x.TechnicalKey == "Membership" && x.Attribute.Count() == 1 && x.Type.Count() == 2 && x.UseCase.Count() == 5,
            x => x.TechnicalKey == "companyRole.dismantler" && x.Attribute.Count() == 3 && x.Type.Count() == 2 && x.UseCase.Count() == 5
        );
    }

    #endregion

    #region GetPolicyContentAsync

    [Fact]
    public async Task GetPolicyContentAsync_WithoutRightOperand_ReturnsExpectedResult()
    {
        // Arrange
        var sut = await CreateSut().ConfigureAwait(false);

        // Act
        var result = await sut.GetPolicyContentAsync(null, PolicyTypeId.Usage, "purpose.trace.v1.TraceBattery").ConfigureAwait(false);

        // Assert
        result.Exists.Should().BeTrue();
        result.Attributes.Key.Should().Be(AttributeKeyId.Static);
        result.Attributes.Values.Should().ContainSingle()
            .And.Satisfy(x => x == "purpose.trace.v1.TraceBattery");
        result.LeftOperand.Should().Be("purpose.trace.v1.TraceBattery");
        result.RightOperandValue.Should().BeNull();
    }

    [Fact]
    public async Task GetPolicyContentAsync_WithRightOperand_ReturnsExpectedResult()
    {
        // Arrange
        var sut = await CreateSut().ConfigureAwait(false);

        // Act
        var result = await sut.GetPolicyContentAsync(null, PolicyTypeId.Usage, "FrameworkAgreement.behavioraltwin").ConfigureAwait(false);

        // Assert
        result.Exists.Should().BeTrue();
        result.Attributes.Key.Should().Be(AttributeKeyId.Version);
        result.Attributes.Values.Should().ContainSingle()
            .And.Satisfy(x => x == "1.0");
        result.LeftOperand.Should().Be("FrameworkAgreement.behavioraltwin");
        result.RightOperandValue.Should().Be("active:{0}");
    }

    #endregion

    #region GetPolicyForOperandContent

    [Fact]
    public async Task GetPolicyForOperandContent__ReturnsExpectedResult()
    {
        // Arrange
        var sut = await CreateSut().ConfigureAwait(false);

        // Act
        var result = await sut.GetPolicyForOperandContent(PolicyTypeId.Usage, Enumerable.Repeat("purpose.trace.v1.TraceBattery", 1)).ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().ContainSingle()
            .And.Satisfy(
                x => x.TechnicalKey == "purpose.trace.v1.TraceBattery" &&
                     x.Attributes.Key == AttributeKeyId.Static &&
                     x.Attributes.Values.Single() == "purpose.trace.v1.TraceBattery" &&
                     x.LeftOperand == "purpose.trace.v1.TraceBattery" &&
                     x.RightOperandValue == null);
    }

    #endregion

    #region Setup

    private async Task<PolicyRepository> CreateSut()
    {
        var context = await _dbTestDbFixture.GetPolicyHubDbContext().ConfigureAwait(false);
        var sut = new PolicyRepository(context);
        return sut;
    }

    #endregion
}
