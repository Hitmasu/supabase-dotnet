using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Supabase.Common.TokenResolver;
using Supabase.RPC.Rpc;
using Xunit;

namespace Supabase.RPC.Tests.Clients;

/// <summary>
/// Tests for error handling in the RPC client
/// </summary>
public class RpcClientErrorHandlingTests : IClassFixture<TestFixture>
{
    private readonly ISupabaseRpc _supabaseRpc;
    private readonly TokenResolver _tokenResolver;
    private readonly Bogus.Faker _faker;

    public RpcClientErrorHandlingTests(TestFixture fixture)
    {
        var serviceProvider = fixture.ServiceProvider;
        _supabaseRpc = serviceProvider.GetRequiredService<ISupabaseRpc>();
        _tokenResolver = (TokenResolver)serviceProvider.GetRequiredService<ITokenResolver>();
        _faker = new Bogus.Faker();
    }

    /// <summary>
    /// Test that CallAsync handles validation errors correctly
    /// </summary>
    /// <remarks>
    /// Scenario: Call a function with invalid parameters that cause a validation error
    /// 
    /// Expected behavior:
    /// 1. The function is called with invalid parameters
    /// 2. The function returns a validation error
    /// 3. The error is propagated as an exception
    /// 
    /// Verifications:
    /// - An exception is thrown with the appropriate error message
    /// </remarks>
    [Fact]
    public async Task CallAsync_ValidationError_ThrowsException()
    {
        // Arrange
        var invalidValue = -1; // Invalid value for the parameter

        // Act
        var func = async () => await _supabaseRpc.CallAsync<int>("validate_positive_number", new { value = invalidValue });

        // Assert
        await func.Should().ThrowAsync<Exception>()
            .Where(ex => ex.Message.Contains("validation") || ex.Message.Contains("positive"));
    }

    /// <summary>
    /// Test that CallAsync handles database errors correctly
    /// </summary>
    /// <remarks>
    /// Scenario: Call a function that causes a database error
    /// 
    /// Expected behavior:
    /// 1. The function is called and causes a database error
    /// 2. The error is propagated as an exception
    /// 
    /// Verifications:
    /// - An exception is thrown with the appropriate error message
    /// </remarks>
    [Fact]
    public async Task CallAsync_DatabaseError_ThrowsException()
    {
        // Arrange
        var tableName = _faker.Random.AlphaNumeric(10); // Non-existent table

        // Act
        var func = async () => await _supabaseRpc.CallAsync<object>("query_table", new { table_name = tableName });

        // Assert
        await func.Should().ThrowAsync<Exception>()
            .Where(ex => ex.Message.Contains("relation") || ex.Message.Contains("does not exist"));
    }

    /// <summary>
    /// Test that CallAsync handles type conversion errors correctly
    /// </summary>
    /// <remarks>
    /// Scenario: Call a function whose result cannot be converted to the expected type
    /// 
    /// Expected behavior:
    /// 1. The function is called and returns a result
    /// 2. The result cannot be converted to the expected type
    /// 3. A conversion error is thrown
    /// 
    /// Verifications:
    /// - An exception is thrown with the appropriate error message
    /// </remarks>
    [Fact]
    public async Task CallAsync_TypeConversionError_ThrowsException()
    {
        // Arrange
        var text = _faker.Lorem.Sentence();

        // Act
        var func = async () => await _supabaseRpc.CallAsync<int>("return_text", new { text });

        // Assert
        await func.Should().ThrowAsync<Exception>();
    }

    /// <summary>
    /// Test that CallAsync handles custom business logic errors correctly
    /// </summary>
    /// <remarks>
    /// Scenario: Call a function that throws a custom business error
    /// 
    /// Expected behavior:
    /// 1. The function is called and throws a custom business error
    /// 2. The error is propagated as an exception
    /// 
    /// Verifications:
    /// - An exception is thrown with the custom error message
    /// </remarks>
    [Fact]
    public async Task CallAsync_BusinessLogicError_ThrowsException()
    {
        // Arrange
        var errorMessage = _faker.Lorem.Sentence();

        // Act
        var func = async () => await _supabaseRpc.CallAsync<object>("throw_business_error", new { error_message = errorMessage });

        // Assert
        await func.Should().ThrowAsync<Exception>()
            .Where(ex => ex.Message.Contains(errorMessage));
    }

    /// <summary>
    /// Test that CallAsync throws exception when procedure throws exception
    /// </summary>
    /// <remarks>
    /// Scenario: Call an RPC procedure that throws an exception
    /// 
    /// Expected behavior:
    /// 1. The RPC procedure throws an exception
    /// 2. The exception is propagated to the caller
    /// 
    /// Verifications:
    /// - An exception is thrown with the appropriate error message
    /// </remarks>
    [Fact]
    public async Task CallAsync_WhenProcedureThrowsException_ThrowsException()
    {
        // Arrange
        var errorMessage = _faker.Lorem.Sentence();

        // Act
        Func<Task> action = () => _supabaseRpc.CallAsync<object>("throw_business_error", new { error_message = errorMessage });

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .Where(ex => ex.Message.Contains(errorMessage));
    }
} 