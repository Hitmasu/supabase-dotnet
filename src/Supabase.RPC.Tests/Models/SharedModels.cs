using System.Text.Json.Serialization;

namespace Supabase.RPC.Tests.Models;

/// <summary>
/// Shared models for RPC tests
/// </summary>
public static class SharedModels
{
    public class Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class Person
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public Address Address { get; set; } = new();
    }

    public class PersonInfo
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsAdult { get; set; }
        public string Location { get; set; } = string.Empty;
    }

    public class ArrayStats
    {
        public int Sum { get; set; }
        public double Average { get; set; }
        public int Count { get; set; }
    }

    public class UserInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Ensure that the CreatedAt field is treated as UTC
        private DateTime _createdAt;
        public DateTime CreatedAt 
        { 
            get => _createdAt;
            set => _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }

    public class ComplexObject
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("meta")]
        public Dictionary<string, object> Metadata { get; set; } = new();
        
        public List<ComplexItem> Items { get; set; } = new();
    }

    public class ComplexItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
    }
} 