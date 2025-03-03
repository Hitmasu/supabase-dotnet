using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Supabase.RPC.Rpc;
using Supabase.RPC.Tests.Models;
using Xunit;
using static Supabase.RPC.Tests.Models.SharedModels;

namespace Supabase.RPC.Tests.Clients;

/// <summary>
/// Tests for complex types in the RPC client
/// </summary>
public class RpcClientComplexTypesTests : IClassFixture<TestFixture>
{
    private readonly ISupabaseRpc _supabaseRpc;
    private readonly Bogus.Faker _faker;

    public RpcClientComplexTypesTests(TestFixture fixture)
    {
        var serviceProvider = fixture.ServiceProvider;
        _supabaseRpc = serviceProvider.GetRequiredService<ISupabaseRpc>();
        _faker = new Bogus.Faker();
    }

    #region Object Parameters Tests

    /// <summary>
    /// Test that CallAsync can handle nested objects in parameters
    /// </summary>
    /// <remarks>
    /// Scenario: Call a function with parameters containing nested objects
    /// 
    /// Expected behavior:
    /// 1. The function is called with parameters containing nested objects
    /// 2. The parameters are correctly serialized
    /// 3. The function returns the expected result
    /// 
    /// Verifications:
    /// - The returned result contains the data from the nested objects
    /// </remarks>
    [Fact]
    public async Task CallAsync_NestedObjectParameters_ReturnsExpectedResult()
    {
        // Arrange
        var address = new Address
        {
            Street = _faker.Address.StreetName(),
            City = _faker.Address.City(),
            State = _faker.Address.State(),
            ZipCode = _faker.Address.ZipCode(),
            Country = _faker.Address.Country()
        };

        var person = new SharedModels.Person
        {
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Age = _faker.Random.Int(18, 80),
            Address = address
        };

        // Act
        var result = await _supabaseRpc.CallAsync<PersonInfo>("process_person", new { person });

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be($"{person.FirstName} {person.LastName}");
        result.IsAdult.Should().Be(person.Age >= 18);
        result.Location.Should().Be($"{address.City}, {address.State}, {address.Country}");
    }

    /// <summary>
    /// Test that CallAsync with complex object parameters works correctly
    /// </summary>
    /// <remarks>
    /// Scenario: Call an RPC procedure with complex object parameters
    /// 
    /// Expected behavior:
    /// 1. The RPC procedure is called with the complex object parameters
    /// 2. The parameters are correctly serialized
    /// 3. The result is correctly returned
    /// 
    /// Verifications:
    /// - The returned result contains the expected data
    /// </remarks>
    [Fact]
    public async Task CallAsync_WithComplexObjectParameters_ReturnsExpectedResult()
    {
        // Arrange
        var person = new
        {
            firstName = _faker.Person.FirstName,
            lastName = _faker.Person.LastName,
            age = _faker.Random.Int(18, 80),
            address = new
            {
                street = _faker.Address.StreetName(),
                city = _faker.Address.City(),
                state = _faker.Address.State(),
                country = _faker.Address.Country()
            }
        };

        // Act
        var result = await _supabaseRpc.CallAsync<PersonInfo>("process_person", new { person });

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be($"{person.firstName} {person.lastName}");
        result.IsAdult.Should().Be(person.age >= 18);
        result.Location.Should().Be($"{person.address.city}, {person.address.state}, {person.address.country}");
    }

    #endregion

    #region Array Parameters Tests

    /// <summary>
    /// Test that CallAsync can handle array parameters
    /// </summary>
    /// <remarks>
    /// Scenario: Call a function with parameters containing arrays
    /// 
    /// Expected behavior:
    /// 1. The function is called with parameters containing arrays
    /// 2. The parameters are correctly serialized
    /// 3. The function returns the expected result
    /// 
    /// Verifications:
    /// - The returned result reflects the correct processing of the arrays
    /// </remarks>
    [Fact]
    public async Task CallAsync_ArrayParameters_ReturnsExpectedResult()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 5).Select(_ => _faker.Random.Int(1, 100)).ToArray();
        var expectedSum = numbers.Sum();
        var expectedAverage = numbers.Average();

        // Act
        var result = await _supabaseRpc.CallAsync<ArrayStats>("process_numbers", new { numbers });

        // Assert
        result.Should().NotBeNull();
        result.Sum.Should().Be(expectedSum);
        result.Average.Should().BeApproximately(expectedAverage, 0.001);
        result.Count.Should().Be(numbers.Length);
    }

    #endregion

    #region Complex Return Types Tests

    /// <summary>
    /// Test that CallAsync can handle complex return types
    /// </summary>
    /// <remarks>
    /// Scenario: Call a function that returns a complex type
    /// 
    /// Expected behavior:
    /// 1. The function is called and returns a complex type
    /// 2. The result is correctly deserialized
    /// 
    /// Verifications:
    /// - The returned complex object has all the expected properties
    /// - The nested properties are correctly deserialized
    /// </remarks>
    [Fact]
    public async Task CallAsync_ComplexReturnType_DeserializesCorrectly()
    {
        // Arrange
        var id = _faker.Random.Guid();

        // Act
        var result = await _supabaseRpc.CallAsync<ComplexObject>("get_complex_object", new { id });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        result.Metadata.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCountGreaterThan(0);
    }

    /// <summary>
    /// Test that CallAsync can call a Postgres function that returns a complex object
    /// </summary>
    /// <remarks>
    /// Scenario: Call a Postgres function that returns a complex object
    /// 
    /// Expected behavior:
    /// 1. The Postgres function is called with the correct parameters
    /// 2. The function result is returned and correctly deserialized
    /// 
    /// Verifications:
    /// - The returned result is of the expected type
    /// - The properties of the returned object have the expected values
    /// </remarks>
    [Fact]
    public async Task CallAsync_PostgresObjectFunction_ReturnsExpectedResult()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var email = _faker.Internet.Email();

        // Act
        var result = await _supabaseRpc.CallAsync<UserInfo>("create_user_info", new { name, email });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Email.Should().Be(email);
        
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// Test that CallAsync can call a Postgres function that returns an array
    /// </summary>
    /// <remarks>
    /// Scenario: Call a Postgres function that returns an array
    /// 
    /// Expected behavior:
    /// 1. The Postgres function is called with the correct parameters
    /// 2. The array returned by the function is correctly deserialized
    /// 
    /// Verifications:
    /// - The returned result is an array of the expected type
    /// - The array contains the expected elements
    /// </remarks>
    [Fact]
    public async Task CallAsync_PostgresArrayFunction_ReturnsExpectedResult()
    {
        // Arrange
        var count = _faker.Random.Int(3, 10);

        // Act
        var result = await _supabaseRpc.CallAsync<int[]>("generate_series", new { count });

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(count);
        for (int i = 0; i < count; i++)
        {
            result[i].Should().Be(i + 1);
        }
    }

    #endregion
} 