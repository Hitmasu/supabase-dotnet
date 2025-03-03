# Supabase.RPC Integration Tests

This project contains integration tests for the Supabase.RPC module, which verify the functionality of the RPC client with a real Supabase server.

## Prerequisites

To run the integration tests, you need:

1. A running Supabase server (local or remote)
2. Administrative access to the Supabase PostgreSQL database
3. Environment variables configured (or use default values for local development)

## Environment Setup

### Environment Variables

Configure the following environment variables:

```bash
# Supabase server URL
export SUPABASE_URL=http://localhost:54321

# Supabase API key (anon key for tests)
export SUPABASE_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZS1kZW1vIiwicm9sZSI6ImFub24iLCJleHAiOjE5ODM4MTI5OTZ9.CRXP1A7WOeoJeXxjNni43kdQwgnWNReilDMblYTn_I0
```

If not configured, the tests will use default values for a local Supabase server.

### Database Configuration

Before running the tests, you need to create the necessary PostgreSQL functions. The SQL scripts are in the `Scripts` folder:

1. `setup_functions.sql`: Basic functions for general tests
2. `setup_auth_functions.sql`: Functions that require authentication
3. `setup_error_functions.sql`: Functions for testing error handling
4. `setup_complex_types_functions.sql`: Functions for testing complex types

Execute the scripts in the Supabase database:

```bash
# Connect to the Supabase PostgreSQL database
psql -h localhost -p 5432 -d postgres -U postgres

# At the psql prompt, execute the scripts
\i Scripts/setup_functions.sql
\i Scripts/setup_auth_functions.sql
\i Scripts/setup_error_functions.sql
\i Scripts/setup_complex_types_functions.sql
```

Or use the Supabase Studio SQL interface to execute the scripts.

## Running the Tests

To run all tests:

```bash
dotnet test
```

To run a specific test:

```bash
dotnet test --filter "FullyQualifiedName=Supabase.RPC.Tests.Clients.RpcClientIntegrationTests.CallAsync_PostgresScalarFunction_ReturnsExpectedResult"
```

## Test Structure

The tests are organized into several classes:

1. `RpcClientIntegrationTests`: Basic integration tests
2. `RpcClientAuthIntegrationTests`: Integration tests with authentication
3. `RpcClientErrorHandlingTests`: Error handling tests
4. `RpcClientComplexTypesTests`: Tests with complex data types

## Implementation Notes

The RPC client implementation follows the Supabase JavaScript RPC reference documentation:
- Supports calling Postgres functions with parameters
- Handles scalar, object, and array return types
- Provides proper error handling
- Supports authentication for RPC calls
- Handles complex nested objects and arrays

## Notes

- Authentication tests (`RpcClientAuthIntegrationTests`) require the Supabase.Authentication module. If not available, these tests will be skipped.
- Some tests may fail if the PostgreSQL functions are not properly configured.
- The tests are designed to run in a local development environment but can also be run in a CI/CD environment with appropriate configurations. 