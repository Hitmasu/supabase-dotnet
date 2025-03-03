**Supabase-dotnet**
================

A .NET client for Supabase.

**Getting Started**
-----------------

To use Supabase-dotnet, install package in project:

```
dotnet add package supabase-dotnet
```

Then register the dependency in your ServiceCollection

```c#
services.AddSupabase("https://yourapiurlsupabase.co", "apikey");
```

**Modules**
------------

| Module            | Interface      | Status           |
| ----------------- | -------------- | ---------------- |
| Auth - JWT GoTrue | ISupabaseAuth  | ✅               |
| RPC               | ISupabaseRpc   | ✅               |
| Storage           | -              | 🏗️ In development |
| Realtime          | -              | 🏗️ In development |
| Postgrest         | -              | 🏗️ In development |
| Functions         | -              | 🏗️ In development |

**Authentication**
-----------------

To work with Authentication on Supabase, simply inject the `ISupabaseAuth` interface in your constructor:

```c#
public class MyService {
  private readonly ISupabaseAuth _supabaseAuth;
  
  public MyService(ISupabaseAuth supabaseAuth){
    _supabaseAuth = supabaseAuth;
  }
  
  public async Task SignInAsync(string email, string password){
    var authenticated = await _supabaseAuth.SignIn(email, password);
  }
}
```

All endpoints are mapped from here: https://github.com/supabase/auth#endpoints

**Remote Procedure Calls (RPC)**
-----------------

To work with RPC on Supabase, inject the `ISupabaseRpc` interface in your constructor:

```c#
public class MyService {
  private readonly ISupabaseRpc _rpcClient;
  
  public MyService(ISupabaseRpc rpcClient){
    _rpcClient = rpcClient;
  }
  
  // Call a function with parameters
  public async Task<int> AddNumbersAsync(int a, int b){
    return await _rpcClient.CallAsync<int>("add_numbers", new { a, b });
  }
  
  // Call a function in a specific schema
  public async Task<UserInfo> GetUserInfoAsync(string userId){
    return await _rpcClient.CallAsync<UserInfo>("get_user_info", new { user_id = userId }, "auth");
  }
  
  // Use a schema-specific client
  public async Task ProcessUserDataAsync(UserData data){
    var authClient = _rpcClient.ForSchema("auth");
    await authClient.CallAsync("process_user_data", data);
  }
}
```

The RPC client supports:
- Calling Postgres functions with parameters
- Working with specific database schemas via the `schema` parameter or `ForSchema()` method
- Handling scalar, object, and array return types
- Proper error handling and validation

**Token Resolver**
-------------------

Supabase-dotnet provides a Token Resolver, which is called before assigning an authentication token to supabase requests. This allows you to easily and portably resolve tokens for all .NET environments.

Here's an example of a simple token resolver:

```c#
//services.AddScoped<ITokenResolver,MyTokenResolver>();

public class MyTokenResolver : ITokenResolver
{
    public ValueTask<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var token = "my.jwt.token";
        return ValueTask.FromResult(token);
    }
}
```

Sometimes you may want to re-pass the token from the request (Server-Side, API, etc.). Here's an example of how to do that:

```c#
//services.AddScoped<ITokenResolver,MyTokenResolver>();
//services.AddHttpContextAccessor();

public class MyTokenResolver : ITokenResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
  
  	public MyTokenResolver(IHttpContextAccessor httpContextAcessor){
      _httpContextAccessor = httpContextAcessor;
    }

    public ValueTask<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var token = request.Headers.Authorization.Replace("Bearer ", "");
        return ValueTask.FromResult(token);
    }
}
```

**Custom Data**
----------------

All methods have overloads for non-custom data and custom data. If you don't want to attach values to the UserMetadata, you can always call the method without passing any custom type, for example:

```c#
_supabaseAuth.GetUserAsync(email);
```

However, if you want to work with custom data (the key 'data' in JSON), you need to create a type for your custom data:

```c#
class CustomUserData : UserMetadataBase{
  public int Age {get;set;}
}
```

And when you do an operation that requires (and can) retrieve that metadata, you can just pass `CustomUserData` as a generic parameter:

#### Registering a user:

```c#
private readonly ISupabaseAuth _supabaseAuth;

//...

var email = "myemail@company.com"
var password = "strongpassword"
var customData = new CustomUserData {
	Age = 50
}

var registeredUser = await _supabaseAuth.SignUpAsync(email,password,customData);
var customMetadata = registeredUser.User.UserMetadata;
var age = customMetadata.Age;
```

#### Getting a user:

```c#
private readonly ISupabaseAuth _supabaseAuth;

//...

var user = await _supabaseAuth.GetUserAsync<CustomUserData>(email);

var customMetadata = user.UserMetadata;
var age = customMetadata.Age;
```
