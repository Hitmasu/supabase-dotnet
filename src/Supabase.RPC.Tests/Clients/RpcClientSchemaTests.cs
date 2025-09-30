using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Supabase.RPC.Rpc;
using Xunit;

namespace Supabase.RPC.Tests.Clients;

/// <summary>
/// Tests for schema-specific operations in the RPC client
/// </summary>
public class RpcClientSchemaTests : IClassFixture<TestFixture>
{
    private readonly ISupabaseRpc _rpcClient;
    private readonly Bogus.Faker _faker;

    public RpcClientSchemaTests(TestFixture fixture)
    {
        var serviceProvider = fixture.ServiceProvider;
        _rpcClient = serviceProvider.GetRequiredService<ISupabaseRpc>();
        _faker = new Bogus.Faker();
    }

    /// <summary>
    /// Test that CallAsync with schema parameter calls a function in a specific schema
    /// </summary>
    /// <remarks>
    /// Scenario: Call an RPC procedure with schema specification
    /// 
    /// Expected behavior:
    /// 1. The RPC procedure in the specified schema is called with the correct parameters
    /// 2. The result is correctly returned
    /// 
    /// Verifications:
    /// - The returned result is of the expected type
    /// - The returned value matches the expected value
    /// </remarks>
    [Fact]
    public async Task CallAsync_WithSchema_ReturnsExpectedResult()
    {
        // Arrange
        var a = _faker.Random.Int(1, 100);
        var b = _faker.Random.Int(1, 100);
        var expectedSum = a + b;

        // Act
        var result = await _rpcClient.CallAsync<int>("add_numbers", new { a, b }, "public");

        // Assert
        result.Should().Be(expectedSum);
    }

    /// <summary>
    /// Test that CallAsync with schema parameter but without result calls a function in a specific schema
    /// </summary>
    /// <remarks>
    /// Scenario: Call an RPC procedure with schema specification but without a return value
    /// 
    /// Expected behavior:
    /// 1. The RPC procedure in the specified schema is called with the correct parameters
    /// 2. The call completes without errors
    /// 
    /// Verifications:
    /// - The call doesn't throw an exception
    /// </remarks>
    [Fact]
    public async Task CallAsync_WithSchema_WithoutResult_CompletesSuccessfully()
    {
        // Arrange
        var message = _faker.Lorem.Sentence();

        // Act
        var action = async () => await _rpcClient.CallAsync("log_message", new { message }, "public");

        // Assert
        await action.Should().NotThrowAsync();
    }

    /// <summary>
    /// Test that ForSchema creates a schema-specific client that works correctly
    /// </summary>
    /// <remarks>
    /// Scenario: Create a schema-specific client and use it to call a function
    /// 
    /// Expected behavior:
    /// 1. The ForSchema method creates a valid schema-specific client
    /// 2. The schema-specific client calls the function in the correct schema
    /// 3. The result is correctly returned
    /// 
    /// Verifications:
    /// - The schema-specific client is not null
    /// - The returned result from the schema-specific client matches the expected value
    /// </remarks>
    [Fact]
    public async Task ForSchema_CreatesValidSchemaClient_ThatWorksCorrectly()
    {
        // Arrange
        var a = _faker.Random.Int(1, 100);
        var b = _faker.Random.Int(1, 100);
        var expectedSum = a + b;

        // Create schema-specific client
        var publicSchemaClient = ((SupabaseRpc)_rpcClient).ForSchema("public");

        // Act
        var result = await publicSchemaClient.CallAsync<int>("add_numbers", new { a, b });

        // Assert
        publicSchemaClient.Should().NotBeNull();
        result.Should().Be(expectedSum);
    }

    /// <summary>
    /// Test that schema-specific functions are correctly called with the Content-Profile header
    /// </summary>
    /// <remarks>
    /// Scenario: Call a function in the auth schema
    /// 
    /// Expected behavior:
    /// 1. The function is called with the correct Accept-Profile header
    /// 2. The function in the auth schema is found and called correctly
    /// 
    /// Verifications:
    /// - The call doesn't throw an exception that the function wasn't found
    /// </remarks>
    [Fact]
    public async Task CallAsync_WithAuthSchema_UsesCorrectHeader()
    {
        // Act
        var action = async () => await _rpcClient.CallAsync<object>("get_auth_user", new { }, "auth");

        // Assert
        // If the Accept-Profile header is working correctly, this should not throw a function not found error
        // The test might fail for other reasons (like auth.get_user doesn't exist), but not because of schema prefixing
        await action.Should().NotThrowAsync<Exception>(because: "the function should be found in the auth schema");
    }
}