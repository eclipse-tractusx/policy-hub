using Org.Eclipse.TractusX.PolicyHub.DbAccess.Repositories;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Tests.Setup;
using Xunit.Extensions.AssemblyFixture;

namespace Org.Eclipse.TractusX.PolicyHub.DbAccess.Tests;

public class HubRepositoriesTests : IAssemblyFixture<TestDbFixture>
{
    private readonly TestDbFixture _dbTestDbFixture;

#pragma warning disable xUnit1041 // Fixture arguments to test classes must have fixture sources
    public HubRepositoriesTests(TestDbFixture testDbFixture)
#pragma warning restore xUnit1041 // Fixture arguments to test classes must have fixture sources
    {
        _dbTestDbFixture = testDbFixture;
    }

    #region GetInstance

    [Fact]
    public async Task GetInstance_WithValid_ReturnsExpected()
    {
        var sut = await CreateSut();

        var repo = sut.GetInstance<IPolicyRepository>();

        repo.Should().BeOfType<PolicyRepository>();
    }

    #endregion

    #region Setup

    private async Task<HubRepositories> CreateSut()
    {
        var context = await _dbTestDbFixture.GetPolicyHubDbContext().ConfigureAwait(false);
        var sut = new HubRepositories(context);
        return sut;
    }

    #endregion
}
