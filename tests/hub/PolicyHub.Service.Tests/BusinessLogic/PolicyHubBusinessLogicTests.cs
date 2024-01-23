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
using Org.Eclipse.TractusX.PolicyHub.Service.Models;
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
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPolicyTypes_WithUseCaseFilter_ReturnsExpected()
    {
        // Arrange
        Setup_GetPolicyTypes();

        // Act
        var result = await _sut.GetPolicyTypes(null, UseCaseId.Sustainability).ToListAsync().ConfigureAwait(false);

        // Assert
        result.Should().ContainSingle();
    }

    #endregion

    #region GetPolicyContentWithFiltersAsync

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_WithNotExistingInDatabase_ThrowsNotFoundException()
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

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_AttributeAndRightOperandNull_ThrowsUnexpectedConditionException()
    {
        // Arrange
        const PolicyTypeId policyTypeId = PolicyTypeId.Access;
        A.CallTo(() => _policyRepository.GetPolicyContentAsync(null, policyTypeId, "membership"))
            .Returns(new ValueTuple<bool, string, (AttributeKeyId?, IEnumerable<string>), string?>(true, "test", (null, Enumerable.Empty<string>()), null!));
        async Task Act() => await _sut.GetPolicyContentWithFiltersAsync(null, policyTypeId, "membership", OperatorId.Equals, null);

        // Act
        var ex = await Assert.ThrowsAsync<UnexpectedConditionException>(Act);

        // Assert
        ex.Message.Should().Be("There must be one configured rightOperand value");
    }

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_WithDynamicValue_ThrowsUnexpectedConditionException()
    {
        // Arrange
        const PolicyTypeId policyTypeId = PolicyTypeId.Access;
        A.CallTo(() => _policyRepository.GetPolicyContentAsync(null, policyTypeId, "membership"))
            .Returns(new ValueTuple<bool, string, (AttributeKeyId?, IEnumerable<string>), string?>(true, "test", (AttributeKeyId.DynamicValue, Enumerable.Empty<string>()), "test:{0}"));

        // Act
        var result = await _sut.GetPolicyContentWithFiltersAsync(null, policyTypeId, "membership", OperatorId.Equals, "abc");

        // Assert
        result.Content.Permission.Constraint.RightOperandValue.Should().Be("abc");
    }

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_WithMultipleRegexValues_ThrowsUnexpectedConditionException()
    {
        // Arrange
        const PolicyTypeId policyTypeId = PolicyTypeId.Access;
        A.CallTo(() => _policyRepository.GetPolicyContentAsync(null, policyTypeId, "membership"))
            .Returns(new ValueTuple<bool, string, (AttributeKeyId?, IEnumerable<string>), string?>(true, "test", (AttributeKeyId.Regex, new[] { "test1", "test2" }), null));
        async Task Act() => await _sut.GetPolicyContentWithFiltersAsync(null, policyTypeId, "membership", OperatorId.Equals, "test");

        // Act
        var ex = await Assert.ThrowsAsync<UnexpectedConditionException>(Act).ConfigureAwait(false);

        // Assert
        ex.Message.Should().Be("There should only be one regex pattern defined");
    }

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_WithMultipleValues_ReturnsExpected()
    {
        // Arrange
        A.CallTo(() => _policyRepository.GetPolicyContentAsync(UseCaseId.Traceability, PolicyTypeId.Usage, "multipleAdditionalValues"))
            .Returns(new ValueTuple<bool, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>(true, "multipleAdditionalValues", new ValueTuple<AttributeKeyId, IEnumerable<string>>(AttributeKeyId.Static, new[] { "value1", "value2", "value3" }), null));

        // Act
        var result = await _sut.GetPolicyContentWithFiltersAsync(UseCaseId.Traceability, PolicyTypeId.Usage, "multipleAdditionalValues", OperatorId.Equals, "test").ConfigureAwait(false);

        // Assert
        result.Content.Id.Should().Be("....");
        result.Content.Permission.Action.Should().Be("use");
        result.AdditionalAttributes.Should().ContainSingle();
        result.AdditionalAttributes!.Single().PossibleValues.Should().HaveCount(3)
            .And.Satisfy(
                x => x == "value1",
                x => x == "value2",
                x => x == "value3"
            );
        result.Content.Permission.Constraint.RightOperandValue.Should().Be("@multipleAdditionalValues-Static");
        result.Content.Permission.Constraint.LeftOperand.Should().Be("multipleAdditionalValues");
        result.Content.Permission.Constraint.Operator.Should().Be("eq");
        result.Content.Permission.Constraint.AndOperands.Should().BeNull();
        result.Content.Permission.Constraint.OrOperands.Should().BeNull();
    }

    #endregion

    #region GetPolicyContentAsync

    [Fact]
    public async Task GetPolicyContentAsync_WithUnmatchingPoliciesAndConstraints_ThrowsNotFoundException()
    {
        // Arrange
        var data = new PolicyContentRequest(PolicyTypeId.Access, ConstraintOperandId.Or,
            new[]
            {
                new Constraints("test", OperatorId.In, null),
                new Constraints("abc", OperatorId.Equals, null)
            });
        A.CallTo(() => _policyRepository.GetPolicyForOperandContent(data.PolicyType, A<IEnumerable<string>>._))
            .Returns(Enumerable.Repeat(new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("test", "active", default, null), 1).ToAsyncEnumerable());
        async Task Act() => await _sut.GetPolicyContentAsync(data);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(Act).ConfigureAwait(false);

        // Assert
        ex.Message.Should().Be($"Policy for type {data.PolicyType} and technicalKeys abc does not exists");
    }

    [Fact]
    public async Task GetPolicyContentAsync_WithAttributeAndRightOperandNull_ThrowsUnexpectedConditionException()
    {
        // Arrange
        var data = new PolicyContentRequest(PolicyTypeId.Access, ConstraintOperandId.Or,
            new[]
            {
                new Constraints("test", OperatorId.In, null),
            });
        A.CallTo(() => _policyRepository.GetPolicyForOperandContent(data.PolicyType, A<IEnumerable<string>>._))
            .Returns(Enumerable.Repeat(new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("test", "active", (null, Enumerable.Empty<string>()), null), 1).ToAsyncEnumerable());
        async Task Act() => await _sut.GetPolicyContentAsync(data);

        // Act
        var ex = await Assert.ThrowsAsync<UnexpectedConditionException>(Act).ConfigureAwait(false);

        // Assert
        ex.Message.Should().Be("There must be one configured rightOperand value");
    }

    [Fact]
    public async Task GetPolicyContentAsync_WithRegexWithoutValue_ThrowsControllerArgumentException()
    {
        // Arrange
        var data = new PolicyContentRequest(PolicyTypeId.Access, ConstraintOperandId.Or,
            new[]
            {
                new Constraints("test", OperatorId.Equals, null),
            });
        A.CallTo(() => _policyRepository.GetPolicyForOperandContent(data.PolicyType, A<IEnumerable<string>>._))
            .Returns(Enumerable.Repeat(new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("test", "active", (AttributeKeyId.Regex, Enumerable.Repeat(@"^BPNL[\w|\d]{12}$", 1)), null), 1).ToAsyncEnumerable());
        async Task Act() => await _sut.GetPolicyContentAsync(data);

        // Act
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act).ConfigureAwait(false);

        // Assert
        ex.ParamName.Should().Be("value");
        ex.Message.Should().Be("you must provide a value for the regex (Parameter 'value')");
    }

    [Fact]
    public async Task GetPolicyContentAsync_WithRegexWithoutMatchingRegexPattern_ThrowsControllerArgumentException()
    {
        // Arrange
        var data = new PolicyContentRequest(PolicyTypeId.Access, ConstraintOperandId.Or,
            new[]
            {
                new Constraints("test", OperatorId.Equals, "testRegValue"),
            });
        A.CallTo(() => _policyRepository.GetPolicyForOperandContent(data.PolicyType, A<IEnumerable<string>>._))
            .Returns(Enumerable.Repeat(new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("test", "active", (AttributeKeyId.Regex, Enumerable.Repeat(@"^BPNL[\w|\d]{12}$", 1)), null), 1).ToAsyncEnumerable());
        async Task Act() => await _sut.GetPolicyContentAsync(data);

        // Act
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act).ConfigureAwait(false);

        // Assert
        ex.ParamName.Should().Be("value");
        ex.Message.Should().Be(@"The provided value testRegValue does not match the regex pattern ^BPNL[\w|\d]{12}$ (Parameter 'value')");
    }

    [Fact]
    public async Task GetPolicyContentAsync_WithMultipleDefinedKeys_ThrowsNotFoundException()
    {
        // Arrange
        var data = new PolicyContentRequest(PolicyTypeId.Access, ConstraintOperandId.Or,
            new[]
            {
                new Constraints("test", OperatorId.In, null),
                new Constraints("test", OperatorId.Equals, null)
            });
        A.CallTo(() => _policyRepository.GetPolicyForOperandContent(data.PolicyType, A<IEnumerable<string>>._))
            .Returns(new[]
            {
                new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("test", "active", default, null)
            }.ToAsyncEnumerable());
        async Task Act() => await _sut.GetPolicyContentAsync(data);

        // Act
        var ex = await Assert.ThrowsAsync<ControllerArgumentException>(Act).ConfigureAwait(false);

        // Assert
        ex.Message.Should().Be("Keys test have been defined multiple times");
    }

    [Fact]
    public async Task GetPolicyContentAsync_WithValid_ReturnsExpected()
    {
        // Arrange
        var data = new PolicyContentRequest(PolicyTypeId.Access, ConstraintOperandId.Or,
            new[]
            {
                new Constraints("inValues", OperatorId.In, null),
                new Constraints("regValue", OperatorId.Equals, "BPNL00000001TEST"),
                new Constraints("dynamicWithoutValue", OperatorId.Equals, null),
                new Constraints("dynamicWithValue", OperatorId.Equals, "test")
            });
        A.CallTo(() => _policyRepository.GetPolicyForOperandContent(data.PolicyType, A<IEnumerable<string>>._))
            .Returns(new[]
            {
                new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("inValues", "active", new (AttributeKeyId.Brands, new[] { "BMW", "Mercedes" }), null),
                new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("regValue", "active", new (AttributeKeyId.Regex, Enumerable.Repeat(@"^BPNL[\w|\d]{12}$", 1)), null),
                new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("dynamicWithoutValue", "active", new (AttributeKeyId.DynamicValue, Enumerable.Repeat("active:{0}", 1)), "rightOperandValueTest"),
                new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("dynamicWithValue", "active", new (AttributeKeyId.DynamicValue, Enumerable.Repeat("active:{0}", 1)), "rightOperandValueTest")
            }.ToAsyncEnumerable());

        // Act
        var result = await _sut.GetPolicyContentAsync(data).ConfigureAwait(false);

        // Assert
        result.Content.Id.Should().Be("....");
        result.Content.Permission.Action.Should().Be("access");
        result.Content.Permission.Constraint.RightOperandValue.Should().BeNull();
        result.Content.Permission.Constraint.LeftOperand.Should().BeNull();
        result.Content.Permission.Constraint.Operator.Should().BeNull();
        result.Content.Permission.Constraint.AndOperands.Should().BeNull();
        result.Content.Permission.Constraint.OrOperands.Should().HaveCount(4)
            .And.Satisfy(
                x => x.LeftOperand == "active" && x.Operator == "in" && ((x.RightOperandValue as IEnumerable<string>)!).Count() == 2,
                x => x.LeftOperand == "active" && x.Operator == "eq" && x.RightOperandValue as string == "BPNL00000001TEST",
                x => x.LeftOperand == "active" && x.Operator == "eq" && x.RightOperandValue as string == "{dynamicValue}",
                x => x.LeftOperand == "active" && x.Operator == "eq" && x.RightOperandValue as string == "test");
    }

    [Fact]
    public async Task GetPolicyContentAsync_WithMultipleValues_ReturnsExpected()
    {
        var data = new PolicyContentRequest(PolicyTypeId.Usage, ConstraintOperandId.And,
            new[]
            {
                new Constraints("multipleAdditionalValues", OperatorId.Equals, null),
                new Constraints("test", OperatorId.In, null)
            });
        A.CallTo(() => _policyRepository.GetPolicyForOperandContent(data.PolicyType, A<IEnumerable<string>>._))
            .Returns(new[]
            {
                new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("multipleAdditionalValues", "multipleAdditionalValues", new(AttributeKeyId.Static, new[] { "value1", "value2", "value3" }), null),
                new ValueTuple<string, string, ValueTuple<AttributeKeyId?, IEnumerable<string>>, string?>("test", "test", new(AttributeKeyId.Version, new[] { "1.0", "1.1" }), null)
            }.ToAsyncEnumerable());

        // Act
        var result = await _sut.GetPolicyContentAsync(data).ConfigureAwait(false);

        // Assert
        result.Content.Id.Should().Be("....");
        result.Content.Permission.Action.Should().Be("use");
        result.Content.Permission.Constraint.RightOperandValue.Should().BeNull();
        result.Content.Permission.Constraint.LeftOperand.Should().BeNull();
        result.Content.Permission.Constraint.Operator.Should().BeNull();
        result.Content.Permission.Constraint.OrOperands.Should().BeNull();
        result.Content.Permission.Constraint.AndOperands.Should().HaveCount(2)
            .And.Satisfy(
                x => x.LeftOperand == "multipleAdditionalValues" && x.Operator == "eq" && x.RightOperandValue as string == "@multipleAdditionalValues-Static",
                x => x.LeftOperand == "test" && x.Operator == "in" && (x.RightOperandValue as IEnumerable<string>)!.Count() == 2);
        result.AdditionalAttributes.Should().ContainSingle()
            .And.Satisfy(x => x.Key == "@multipleAdditionalValues-Static");
        result.AdditionalAttributes!.Single().PossibleValues.Should().HaveCount(3)
            .And.Satisfy(
                x => x == "value1",
                x => x == "value2",
                x => x == "value3");
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

        A.CallTo(() => _policyRepository.GetPolicyTypes(null, null))
            .Returns(new[] { susAccess, traceAccess }.ToAsyncEnumerable());
        A.CallTo(() => _policyRepository.GetPolicyTypes(null, UseCaseId.Sustainability))
            .Returns(new[] { susAccess }.ToAsyncEnumerable());
    }

    #endregion
}
