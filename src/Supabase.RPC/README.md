# Supabase.RPC

Este módulo fornece suporte para chamadas de procedimento remoto (RPC) no Supabase usando C#.

## Instalação

```bash
dotnet add package supabase-dotnet-rpc
```

## Uso

### Configuração

Para usar o cliente RPC, você precisa registrá-lo no contêiner de injeção de dependência:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Supabase;
using Supabase.RPC.Extensions;

var services = new ServiceCollection();

services.AddSupabase("https://your-project-url.supabase.co", "your-api-key")
    .AddRpc();

var serviceProvider = services.BuildServiceProvider();
```

### Chamando procedimentos armazenados

Você pode chamar procedimentos armazenados usando o cliente RPC:

```csharp
using Supabase.RPC.Interfaces;

// Obtenha o cliente RPC do contêiner de injeção de dependência
var rpcClient = serviceProvider.GetRequiredService<IRpcClient>();

// Chame um procedimento armazenado com parâmetros e tipo de retorno
var result = await rpcClient.CallAsync<MyResultType>("procedure_name", new { param1 = "value1", param2 = 123 });

// Chame um procedimento armazenado com parâmetros sem tipo de retorno
await rpcClient.CallAsync("procedure_name", new { param1 = "value1", param2 = 123 });

// Chame um procedimento armazenado sem parâmetros
var result = await rpcClient.CallAsync<MyResultType>("procedure_name");
```

## Exemplos

### Exemplo 1: Chamando um procedimento que retorna um valor

```csharp
// Defina um tipo para o resultado
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Chame o procedimento
var user = await rpcClient.CallAsync<User>("get_user_by_id", new { id = 123 });
Console.WriteLine($"User: {user.Name}, Email: {user.Email}");
```

### Exemplo 2: Chamando um procedimento sem retorno

```csharp
// Chame o procedimento
await rpcClient.CallAsync("update_user_status", new { user_id = 123, status = "active" });
```

### Exemplo 3: Chamando um procedimento sem parâmetros

```csharp
// Defina um tipo para o resultado
public class SystemStatus
{
    public bool IsOnline { get; set; }
    public string Version { get; set; } = string.Empty;
}

// Chame o procedimento
var status = await rpcClient.CallAsync<SystemStatus>("get_system_status");
Console.WriteLine($"System is {(status.IsOnline ? "online" : "offline")}, Version: {status.Version}");
```

## Tratamento de erros

O cliente RPC lança exceções quando ocorrem erros. Você pode capturar essas exceções para tratar erros:

```csharp
try
{
    var result = await rpcClient.CallAsync<MyResultType>("procedure_name", new { param1 = "value1" });
    // Processe o resultado
}
catch (Exception ex)
{
    Console.WriteLine($"Error calling procedure: {ex.Message}");
}
``` 