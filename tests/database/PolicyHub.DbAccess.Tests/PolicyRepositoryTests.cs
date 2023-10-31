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
    private readonly IFixture _fixture;

    public PolicyRepositoryTests(TestDbFixture testDbFixture)
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
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
        result.Should().NotBeEmpty().And.HaveCount(3).And.Satisfy(
            x => x == "Version",
            x => x == "Brands",
            x => x == "Value");
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
        result.Should().NotBeEmpty().And.HaveCount(4).And.Satisfy(
            x => x.TechnicalKey == "framework.traceability" && x.Attribute.Count() == 2 && x.Type.Count() == 2 && x.UseCase.Count() == 1,
            x => x.TechnicalKey == "bpn" && x.Attribute.Count() == 0 && x.Type.Count() == 2 && x.UseCase.Count() == 2,
            x => x.TechnicalKey == "time" && x.Attribute.Count() == 2 && x.Type.Count() == 1 && x.UseCase.Count() == 2,
            x => x.TechnicalKey == "....dismantler" && x.Attribute.Count() == 2 && x.Type.Count() == 2 && x.UseCase.Count() == 1
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
            x => x.TechnicalKey == "framework.traceability" && x.Attribute.Count() == 2 && x.Type.Count() == 2 && x.UseCase.Count() == 1,
            x => x.TechnicalKey == "bpn" && !x.Attribute.Any() && x.Type.Count() == 2 && x.UseCase.Count() == 2,
            x => x.TechnicalKey == "....dismantler" && x.Attribute.Count() == 2 && x.Type.Count() == 2 && x.UseCase.Count() == 1
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
            x => x.TechnicalKey == "bpn" && !x.Attribute.Any() && x.Type.Count() == 2 && x.UseCase.Count() == 2,
            x => x.TechnicalKey == "time" && x.Attribute.Count() == 2 && x.Type.Count() == 1 && x.UseCase.Count() == 2,
            x => x.TechnicalKey == "....dismantler" && x.Attribute.Count() == 2 && x.Type.Count() == 2 && x.UseCase.Count() == 1
        );
    }

    #endregion

    #region Setup

    private async Task<PolicyRepository> CreateSut()
    {
        var context = await _dbTestDbFixture.GetPortalDbContext().ConfigureAwait(false);
        var sut = new PolicyRepository(context);
        return sut;
    }

    #endregion
}
