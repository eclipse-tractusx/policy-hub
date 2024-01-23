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

using Org.Eclipse.TractusX.PolicyHub.DbAccess.Models;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.Models;
using Org.Eclipse.TractusX.PolicyHub.Service.Tests.Setup;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Library;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Controllers;

[Trait("Category", "PolicyHubEndToEnd")]
[Collection("PolicyHub")]
public class PolicyHubControllerTests : IClassFixture<IntegrationTestFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };

    private const string BaseUrl = "/api/policy-hub";
    private readonly HttpClient _client;

    public PolicyHubControllerTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Policy Attributes

    [Fact]
    public async Task GetAttributes()
    {
        // Act
        var attributes = await _client.GetFromJsonAsync<IEnumerable<string>>($"{BaseUrl}/policy-attributes").ConfigureAwait(false);

        // Assert
        attributes.Should().NotBeNull().And.HaveCount(5).And.Satisfy(
            x => x == "Regex",
            x => x == "Static",
            x => x == "DynamicValue",
            x => x == "Brands",
            x => x == "Version"
        );
    }

    #endregion

    #region Policy Types

    [Fact]
    public async Task GetPolicyTypes_WithoutFilter_ReturnsExpected()
    {
        // Act
        var policies = await _client.GetFromJsonAsync<IEnumerable<PolicyTypeResponse>>($"{BaseUrl}/policy-types", JsonOptions).ConfigureAwait(false);

        // Assert
        policies.Should().NotBeNull()
            .And.HaveCount(11).And.Satisfy(
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
    public async Task GetPolicyTypes_WithTypeFilter_ReturnsExpected()
    {
        // Act
        var policies = await _client.GetFromJsonAsync<IEnumerable<PolicyTypeResponse>>($"{BaseUrl}/policy-types?type={PolicyTypeId.Access.ToString()}", JsonOptions).ConfigureAwait(false);

        // Assert
        policies.Should().NotBeNull()
            .And.HaveCount(3).And.Satisfy(
                x => x.TechnicalKey == "BusinessPartnerNumber",
                x => x.TechnicalKey == "Membership",
                x => x.TechnicalKey == "companyRole.dismantler"
            );
    }

    [Fact]
    public async Task GetPolicyTypes_WithUseCaseFilter_ReturnsExpected()
    {
        // Act
        var policies = await _client.GetFromJsonAsync<IEnumerable<PolicyTypeResponse>>($"{BaseUrl}/policy-types?useCase={UseCaseId.Traceability.ToString()}", JsonOptions).ConfigureAwait(false);

        // Assert
        policies.Should().NotBeNull()
            .And.HaveCount(8).And.Satisfy(
                x => x.TechnicalKey == "BusinessPartnerNumber",
                x => x.TechnicalKey == "Membership",
                x => x.TechnicalKey == "FrameworkAgreement.traceability",
                x => x.TechnicalKey == "purpose.trace.v1.TraceBattery",
                x => x.TechnicalKey == "purpose.trace.v1.aspects",
                x => x.TechnicalKey == "companyRole.dismantler",
                x => x.TechnicalKey == "purpose.trace.v1.qualityanalysis",
                x => x.TechnicalKey == "purpose"
            );
    }

    #endregion

    #region Policy Content

    [Fact]
    public async Task GetPolicyContent_WithRegexWithIncorrectValue_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Access}&credential=BusinessPartnerNumber&operatorId={OperatorId.Equals}&value=notmatching").ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions).ConfigureAwait(false);
        error!.Errors.Should().ContainSingle().And.Satisfy(
            x => x.Value.Single() == @"The provided value notmatching does not match the regex pattern ^BPNL[\w|\d]{12}$ (Parameter 'value')");
    }

    [Fact]
    public async Task GetPolicyContent_WithRegexWithoutValue_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Access}&credential=BusinessPartnerNumber&operatorId={OperatorId.Equals}").ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions).ConfigureAwait(false);
        error!.Errors.Should().ContainSingle().And.Satisfy(
            x => x.Value.Single() == "you must provide a value for the regex (Parameter 'value')");
    }

    [Fact]
    public async Task GetPolicyContent_BpnWithValue_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Usage}&credential=BusinessPartnerNumber&operatorId={OperatorId.Equals}&value=BPNL00000003CRHK").ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync().ConfigureAwait(false))
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"leftOperand\":\"BusinessPartnerNumber\",\"operator\":\"eq\",\"rightOperand\":\"BPNL00000003CRHK\"}}}}");
    }

    [Fact]
    public async Task GetPolicyContent_UsageFrameworkEquals_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Usage}&credential=FrameworkAgreement.traceability&operatorId={OperatorId.Equals}").ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync().ConfigureAwait(false))
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"leftOperand\":\"FrameworkAgreement.traceability\",\"operator\":\"eq\",\"rightOperand\":\"@FrameworkAgreement.traceability-Version\"}}},\"attributes\":[{\"key\":\"@FrameworkAgreement.traceability-Version\",\"possibleValues\":[\"active:1.0\",\"active:1.1\",\"active:1.2\"]}]}");
    }

    [Fact]
    public async Task GetPolicyContent_UsageDismantlerIn_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Usage}&credential=companyRole.dismantler&operatorId={OperatorId.In}").ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync().ConfigureAwait(false))
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"leftOperand\":\"Dismantler.activityType\",\"operator\":\"in\",\"rightOperand\":[\"Audi\",\"BMW\",\"VW\"]}}}}");
    }

    [Fact]
    public async Task GetPolicyContent_TraceabilityUsagePurposeEquals_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?useCase={UseCaseId.Traceability}&type={PolicyTypeId.Usage}&credential=purpose.trace.v1.TraceBattery&operatorId={OperatorId.Equals}").ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync().ConfigureAwait(false))
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"leftOperand\":\"purpose.trace.v1.TraceBattery\",\"operator\":\"eq\",\"rightOperand\":\"purpose.trace.v1.TraceBattery\"}}}}");
    }

    #endregion

    #region Policy Content with Filters

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_TwoEqualsConstraintsAndOperand_ReturnsExpected()
    {
        // Arrange
        var data = new PolicyContentRequest(
            PolicyTypeId.Usage,
            ConstraintOperandId.And,
            new[]
            {
                new Constraints("FrameworkAgreement.traceability", OperatorId.Equals, null),
                new Constraints("companyRole.dismantler", OperatorId.In, null)
            });

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/policy-content", data, JsonOptions).ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync().ConfigureAwait(false))
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"odrl:and\":[{\"leftOperand\":\"FrameworkAgreement.traceability\",\"operator\":\"eq\",\"rightOperand\":\"@FrameworkAgreement.traceability-Version\"},{\"leftOperand\":\"Dismantler.activityType\",\"operator\":\"in\",\"rightOperand\":[\"Audi\",\"BMW\",\"VW\"]}]}}},\"attributes\":[{\"key\":\"@FrameworkAgreement.traceability-Version\",\"possibleValues\":[\"active:1.0\",\"active:1.1\",\"active:1.2\"]}]}");
    }

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_MultipleConstraintsEqualsAndOperand_ReturnsExpected()
    {
        // Arrange
        var data = new PolicyContentRequest(
            PolicyTypeId.Usage,
            ConstraintOperandId.And,
            new[]
            {
                new Constraints("FrameworkAgreement.traceability", OperatorId.Equals, null),
                new Constraints("companyRole.dismantler", OperatorId.In, null),
                new Constraints("BusinessPartnerNumber", OperatorId.Equals, "BPNL00000003CRHK")
            });

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/policy-content", data, JsonOptions).ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync().ConfigureAwait(false))
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"odrl:and\":[{\"leftOperand\":\"BusinessPartnerNumber\",\"operator\":\"eq\",\"rightOperand\":\"BPNL00000003CRHK\"},{\"leftOperand\":\"FrameworkAgreement.traceability\",\"operator\":\"eq\",\"rightOperand\":\"@FrameworkAgreement.traceability-Version\"},{\"leftOperand\":\"Dismantler.activityType\",\"operator\":\"in\",\"rightOperand\":[\"Audi\",\"BMW\",\"VW\"]}]}}},\"attributes\":[{\"key\":\"@FrameworkAgreement.traceability-Version\",\"possibleValues\":[\"active:1.0\",\"active:1.1\",\"active:1.2\"]}]}");
    }

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_MultipleConstraintsEqualsOrOperand_ReturnsExpected()
    {
        // Arrange
        var data = new PolicyContentRequest(
            PolicyTypeId.Usage,
            ConstraintOperandId.Or,
            new[]
            {
                new Constraints("FrameworkAgreement.traceability", OperatorId.Equals, null),
                new Constraints("companyRole.dismantler", OperatorId.In, null),
            });

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/policy-content", data, JsonOptions).ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync().ConfigureAwait(false))
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"odrl:or\":[{\"leftOperand\":\"FrameworkAgreement.traceability\",\"operator\":\"eq\",\"rightOperand\":\"@FrameworkAgreement.traceability-Version\"},{\"leftOperand\":\"Dismantler.activityType\",\"operator\":\"in\",\"rightOperand\":[\"Audi\",\"BMW\",\"VW\"]}]}}},\"attributes\":[{\"key\":\"@FrameworkAgreement.traceability-Version\",\"possibleValues\":[\"active:1.0\",\"active:1.1\",\"active:1.2\"]}]}");
    }

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_WithSameConstraintKeys_ReturnsError()
    {
        // Arrange
        var data = new PolicyContentRequest(
            PolicyTypeId.Usage,
            ConstraintOperandId.Or,
            new[]
            {
                new Constraints("FrameworkAgreement.traceability", OperatorId.Equals, null),
                new Constraints("FrameworkAgreement.traceability", OperatorId.Equals, null),
            });

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/policy-content", data, JsonOptions).ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions).ConfigureAwait(false);
        error!.Errors.Should().ContainSingle().And.Satisfy(
            x => x.Value.Single() == "Keys FrameworkAgreement.traceability have been defined multiple times");
    }

    #endregion

    #region Swagger

    [Fact]
    public async Task CheckSwagger_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/swagger/v2/swagger.json").ConfigureAwait(false);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
