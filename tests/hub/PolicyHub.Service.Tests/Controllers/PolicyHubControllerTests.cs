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

using Org.Eclipse.TractusX.PolicyHub.DbAccess.Models;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.Models;
using Org.Eclipse.TractusX.PolicyHub.Service.Tests.Setup;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling.Web;
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
        var attributes = await _client.GetFromJsonAsync<IEnumerable<string>>($"{BaseUrl}/policy-attributes");

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
        var policies = await _client.GetFromJsonAsync<IEnumerable<PolicyTypeResponse>>($"{BaseUrl}/policy-types", JsonOptions);

        // Assert
        policies.Should().NotBeNull()
            .And.HaveCount(7).And.Satisfy(
                x => x.TechnicalKey == "BusinessPartnerNumber",
                x => x.TechnicalKey == "Membership",
                x => x.TechnicalKey == "FrameworkAgreement",
                x => x.TechnicalKey == "Dismantler.allowedBrands",
                x => x.TechnicalKey == "UsagePurpose",
                x => x.TechnicalKey == "Dismantler",
                x => x.TechnicalKey == "ContractReference"
            );
    }

    [Fact]
    public async Task GetPolicyTypes_WithTypeFilter_ReturnsExpected()
    {
        // Act
        var policies = await _client.GetFromJsonAsync<IEnumerable<PolicyTypeResponse>>($"{BaseUrl}/policy-types?type={PolicyTypeId.Access}", JsonOptions);

        // Assert
        policies.Should().NotBeNull()
            .And.HaveCount(4).And.Satisfy(
                x => x.TechnicalKey == "BusinessPartnerNumber",
                x => x.TechnicalKey == "Membership",
                x => x.TechnicalKey == "Dismantler.allowedBrands",
                x => x.TechnicalKey == "Dismantler"
            );
    }

    [Fact]
    public async Task GetPolicyTypes_WithUseCaseFilter_ReturnsExpected()
    {
        // Act
        var policies = await _client.GetFromJsonAsync<IEnumerable<PolicyTypeResponse>>($"{BaseUrl}/policy-types?useCase={UseCaseId.Traceability}", JsonOptions);

        // Assert
        policies.Should().NotBeNull()
            .And.HaveCount(6).And.Satisfy(
                x => x.TechnicalKey == "BusinessPartnerNumber",
                x => x.TechnicalKey == "Membership",
                x => x.TechnicalKey == "FrameworkAgreement",
                x => x.TechnicalKey == "Dismantler.allowedBrands",
                x => x.TechnicalKey == "UsagePurpose",
                x => x.TechnicalKey == "Dismantler"
            );
    }

    #endregion

    #region Policy Content

    [Fact]
    public async Task GetPolicyContent_WithRegexWithIncorrectValue_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Access}&policyName=BusinessPartnerNumber&operatorType={OperatorId.Equals}&value=notmatching");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions);
        error!.Errors.Should().ContainSingle().And.Satisfy(
            x => x.Value.Single() == @"The provided value notmatching does not match the regex pattern ^BPNL[\w|\d]{12}$ (Parameter 'value')");
    }

    [Fact]
    public async Task GetPolicyContent_WithRegexWithoutValue_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Access}&policyName=BusinessPartnerNumber&operatorType={OperatorId.Equals}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions);
        error!.Errors.Should().ContainSingle().And.Satisfy(
            x => x.Value.Single() == "you must provide a value for the regex (Parameter 'value')");
    }

    [Fact]
    public async Task GetPolicyContent_BpnWithValue_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Usage}&policyName=BusinessPartnerNumber&operatorType={OperatorId.Equals}&value=BPNL00000003CRHK");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync())
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"leftOperand\":\"cx-policy:BusinessPartnerNumber\",\"operator\":\"eq\",\"rightOperand\":\"BPNL00000003CRHK\"}}}}");
    }

    [Fact]
    public async Task GetPolicyContent_UsageFrameworkEquals_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?useCase=Traceability&type={PolicyTypeId.Usage}&policyName=FrameworkAgreement&operatorType={OperatorId.Equals}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync())
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"leftOperand\":\"cx-policy:FrameworkAgreement\",\"operator\":\"eq\",\"rightOperand\":\"DataExchangeGovernance:1.0\"}}}}");
    }

    [Fact]
    public async Task GetPolicyContent_UsageDismantlerIn_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?type={PolicyTypeId.Usage}&policyName=Dismantler.allowedBrands&operatorType={OperatorId.In}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync())
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"leftOperand\":\"cx-policy:Dismantler.allowedBrands\",\"operator\":\"in\",\"rightOperand\":[\"BMW\",\"Audi\",\"VW\"]}}}}");
    }

    [Fact]
    public async Task GetPolicyContent_TraceabilityUsagePurposeEquals_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/policy-content?useCase={UseCaseId.Traceability}&type={PolicyTypeId.Usage}&policyName=Membership&operatorType={OperatorId.Equals}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync())
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"leftOperand\":\"cx-policy:Membership\",\"operator\":\"eq\",\"rightOperand\":\"active\"}}}}");
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
                new Constraints("FrameworkAgreement", OperatorId.Equals, "DataExchangeGovernance:1.0"),
                new Constraints("Dismantler.allowedBrands", OperatorId.In, "Audi")
            });

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/policy-content", data, JsonOptions);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync())
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"odrl:and\":[{\"leftOperand\":\"cx-policy:Dismantler.allowedBrands\",\"operator\":\"in\",\"rightOperand\":[\"BMW\",\"Audi\",\"VW\"]},{\"leftOperand\":\"cx-policy:FrameworkAgreement\",\"operator\":\"eq\",\"rightOperand\":\"DataExchangeGovernance:1.0\"}]}}}}");
    }

    [Fact]
    public async Task GetPolicyContentWithFiltersAsync_WithWrongValue_ReturnsBadRequest()
    {
        // Arrange
        var data = new PolicyContentRequest(
            PolicyTypeId.Usage,
            ConstraintOperandId.And,
            new[]
            {
                new Constraints("FrameworkAgreement.traceability", OperatorId.Equals, "1.0"),
                new Constraints("purpose", OperatorId.In, "By accepting this policy you have to pay 1K BC")
            });

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/policy-content", data, JsonOptions);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
                new Constraints("FrameworkAgreement", OperatorId.Equals, "DataExchangeGovernance:1.0"),
                new Constraints("Dismantler.allowedBrands", OperatorId.In, "Audi"),
                new Constraints("BusinessPartnerNumber", OperatorId.Equals, "BPNL00000003CRHK")
            });

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/policy-content", data, JsonOptions);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync())
            .Should()
            .Be("{\"content\":{\"@context\":[\"https://www.w3.org/ns/odrl.jsonld\",{\"cx\":\"https://w3id.org/catenax/v0.0.1/ns/\"}],\"@type\":\"Offer\",\"@id\":\"....\",\"permission\":{\"action\":\"use\",\"constraint\":{\"odrl:and\":[{\"leftOperand\":\"cx-policy:BusinessPartnerNumber\",\"operator\":\"eq\",\"rightOperand\":\"BPNL00000003CRHK\"},{\"leftOperand\":\"cx-policy:Dismantler.allowedBrands\",\"operator\":\"in\",\"rightOperand\":[\"BMW\",\"Audi\",\"VW\"]},{\"leftOperand\":\"cx-policy:FrameworkAgreement\",\"operator\":\"eq\",\"rightOperand\":\"DataExchangeGovernance:1.0\"}]}}}}");
    }

    #endregion

    #region Swagger

    [Fact]
    public async Task CheckSwagger_ReturnsExpected()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/swagger/v2/swagger.json");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
