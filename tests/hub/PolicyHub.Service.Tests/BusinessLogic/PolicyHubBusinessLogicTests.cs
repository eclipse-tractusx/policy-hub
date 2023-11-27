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

using Org.Eclipse.TractusX.PolicyHub.DbAccess;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Models;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Repositories;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Library;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.BusinessLogic;

public class PolicyHubBusinessLogicTests
{
    private readonly IFixture _fixture;
    private readonly IPolicyRepository _policyRepository;

    private readonly PolicyHubBusinessLogic _sut;

    public PolicyHubBusinessLogicTests()
    {
        _fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var hubRepositories = A.Fake<IHubRepositories>();
        _policyRepository = A.Fake<IPolicyRepository>();

        A.CallTo(() => hubRepositories.GetInstance<IPolicyRepository>()).Returns(_policyRepository);

        _sut = new PolicyHubBusinessLogic(hubRepositories);
    }

    #region GetAttributes

    [Fact]
    public async Task GetAttributeKeys_ReturnsExpected()
    {
        // Arrange
        Setup_GetAttributeKeys();

        // Act
        var result = await _sut.GetAttributeKeys().ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().HaveCount(5);
    }

    #endregion

    #region GetPolicyTypes

    [Fact]
    public async Task GetPolicyTypes_WithoutFilter_ReturnsExpected()
    {
        // Arrange
        Setup_GetPolicyTypes();

        // Act
        var result = await _sut.GetPolicyTypes(null, null).ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetPolicyTypes_WithPolicyTypeFilter_ReturnsExpected()
    {
        // Arrange
        Setup_GetPolicyTypes();

        // Act
        var result = await _sut.GetPolicyTypes(PolicyTypeId.Purpose, null).ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetPolicyTypes_WithUseCaseFilter_ReturnsExpected()
    {
        // Arrange
        Setup_GetPolicyTypes();

        // Act
        var result = await _sut.GetPolicyTypes(null, UseCaseId.Sustainability).ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region GetPolicyContentAsync

    [Fact]
    public async Task GetPolicyContentAsync_WithNotExistingInDatabase_ThrowsNotFoundException()
    {
        // Arrange
        const PolicyTypeId policyTypeId = PolicyTypeId.Access;
        A.CallTo(() => _policyRepository.GetPolicyContentAsync(null, policyTypeId, "membership"))
            .Returns(new ValueTuple<bool, string, (AttributeKeyId, IEnumerable<string>), string?>(false, null!, default, null!));
        async Task Act() => await _sut.GetPolicyContentWithFiltersAsync(null, policyTypeId, "membership", OperatorId.Equals, null);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act);

        // Assert
        ex.Message.Should().Be($"Policy for type {policyTypeId} and technicalKey membership does not exists");
    }

    #endregion

    #region Setup

    private void Setup_GetAttributeKeys()
    {
        A.CallTo(() => _policyRepository.GetAttributeKeys())
            .Returns(_fixture.CreateMany<string>(5).ToAsyncEnumerable());
    }

    private void Setup_GetPolicyTypes()
    {
        var susAccess = _fixture.Build<PolicyTypeResponse>()
            .With(x => x.UseCase, Enumerable.Repeat(UseCaseId.Sustainability, 1))
            .With(x => x.Type, Enumerable.Repeat(PolicyTypeId.Access, 1))
            .Create();
        var traceAccess = _fixture.Build<PolicyTypeResponse>()
            .With(x => x.UseCase, Enumerable.Repeat(UseCaseId.Traceability, 1))
            .With(x => x.Type, Enumerable.Repeat(PolicyTypeId.Access, 1))
            .Create();
        var susPurpose = _fixture.Build<PolicyTypeResponse>()
            .With(x => x.UseCase, Enumerable.Repeat(UseCaseId.Sustainability, 1))
            .With(x => x.Type, Enumerable.Repeat(PolicyTypeId.Purpose, 1))
            .Create();

        A.CallTo(() => _policyRepository.GetPolicyTypes(null, null))
            .Returns(new[] { susAccess, traceAccess, susPurpose }.ToAsyncEnumerable());
        A.CallTo(() => _policyRepository.GetPolicyTypes(PolicyTypeId.Purpose, null))
            .Returns(new[] { susPurpose }.ToAsyncEnumerable());
        A.CallTo(() => _policyRepository.GetPolicyTypes(null, UseCaseId.Sustainability))
            .Returns(new[] { susAccess, susPurpose }.ToAsyncEnumerable());
    }

    #endregion
}
