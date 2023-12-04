using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;
using Org.Eclipse.TractusX.PolicyHub.Service.Extensions;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Extensions;

public class JsonGenerationExtensionsTests
{
    [Theory]
    [InlineData(PolicyTypeId.Access, "access")]
    [InlineData(PolicyTypeId.Usage, "use")]
    [InlineData(PolicyTypeId.Purpose, "use")]
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
        ex.Message.Should().Be("0 is not a valid value (Parameter 'type')\nActual value was 0.");
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
        ex.Message.Should().Be("0 is not a valid value (Parameter 'type')\nActual value was 0.");
    }
}
