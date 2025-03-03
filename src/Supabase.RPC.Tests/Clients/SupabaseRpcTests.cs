using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Supabase.RPC.Rpc;
using Xunit;

namespace Supabase.RPC.Tests.Clients;

/// <summary>
/// Basic tests for the RPC client
/// </summary>
public class SupabaseRpcTests : IClassFixture<TestFixture>
{
    private readonly ISupabaseRpc _supabaseRpc;
    private readonly Bogus.Faker _faker;

    public SupabaseRpcTests(TestFixture fixture)
    {
        var serviceProvider = fixture.ServiceProvider;
        _supabaseRpc = serviceProvider.GetRequiredService<ISupabaseRpc>();
        _faker = new Bogus.Faker();
    }

    #region Scalar Functions

    /// <summary>
    /// Test that CallAsync with parameters returns the expected result
    /// </summary>
    /// <remarks>
    /// Scenario: Call an RPC procedure with parameters
    /// 
    /// Expected behavior:
    /// 1. The RPC procedure is called with the correct parameters
    /// 2. The result is correctly returned
    /// 
    /// Verifications:
    /// - The returned result is of the expected type
    /// - The returned value matches the expected value
    /// </remarks>
    [Fact]
    public async Task CallAsync_WithParameters_ReturnsExpectedResult()
    {
        // Arrange
        var a = _faker.Random.Int(1, 100);
        var b = _faker.Random.Int(1, 100);
        var expectedSum = a + b;

        // Act
        var result = await _supabaseRpc.CallAsync<int>("add_numbers", new { a, b });

        // Assert
        result.Should().Be(expectedSum);
    }

    /// <summary>
    /// Test that CallAsync without parameters calls the procedure with empty parameters
    /// </summary>
    /// <remarks>
    /// Scenario: Call an RPC procedure without parameters
    /// 
    /// Expected behavior:
    /// 1. The RPC procedure is called with empty parameters
    /// 2. The result is correctly returned
    /// 
    /// Verifications:
    /// - The returned result is of the expected type
    /// - The returned value is valid
    /// </remarks>
    [Fact]
    public async Task CallAsync_WithoutParameters_ReturnsExpectedResult()
    {
        // Act
        var result = await _supabaseRpc.CallAsync<DateTime>("get_current_timestamp");

        // Assert
        result.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }

    #endregion

    #region Void Functions

    /// <summary>
    /// Test that CallAsync without return type calls the procedure correctly
    /// </summary>
    /// <remarks>
    /// Scenario: Call an RPC procedure without a return type
    /// 
    /// Expected behavior:
    /// 1. The RPC procedure is called with the correct parameters
    /// 2. The call completes without errors
    /// 
    /// Verifications:
    /// - The call doesn't throw an exception
    /// </remarks>
    [Fact]
    public async Task CallAsync_WithoutReturnType_CompletesSuccessfully()
    {
        // Arrange
        var message = _faker.Lorem.Sentence();

        // Act
        Func<Task> action = () => _supabaseRpc.CallAsync("log_message", new { message });

        // Assert
        await action.Should().NotThrowAsync();
    }

    #endregion

    #region Non-Existent Functions

    /// <summary>
    /// Test that CallAsync throws exception when calling a non-existent function
    /// </summary>
    /// <remarks>
    /// Scenario: Call a non-existent Postgres function
    /// 
    /// Expected behavior:
    /// 1. The call to the non-existent function fails
    /// 2. An exception is thrown
    /// 
    /// Verifications:
    /// - An exception is thrown with the appropriate error message
    /// </remarks>
    [Fact]
    public async Task CallAsync_NonExistentFunction_ThrowsException()
    {
        // Arrange
        var nonExistentFunction = $"non_existent_function_{_faker.Random.AlphaNumeric(10)}";

        // Act
        var action = () => _supabaseRpc.CallAsync<object>(nonExistentFunction, new { });

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .Where(ex => ex.Message.Contains("PGRST202"));
    }

    #endregion
} 