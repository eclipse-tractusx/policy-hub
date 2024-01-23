using Org.Eclipse.TractusX.PolicyHub.DbAccess.Repositories;
using Org.Eclipse.TractusX.PolicyHub.DbAccess.Tests.Setup;
using Xunit.Extensions.AssemblyFixture;

namespace Org.Eclipse.TractusX.PolicyHub.DbAccess.Tests;

public class HubRepositoriesTests : IAssemblyFixture<TestDbFixture>
{
    private readonly TestDbFixture _dbTestDbFixture;

    public HubRepositoriesTests(TestDbFixture testDbFixture)
    {
        _dbTestDbFixture = testDbFixture;
    }

    #region GetInstance

    [Fact]
    public async Task GetInstance_WithValid_ReturnsExpected()
    {
        var sut = await CreateSut().ConfigureAwait(false);

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
