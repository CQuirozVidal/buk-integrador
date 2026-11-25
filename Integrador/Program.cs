var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("empleados", c => 
{
    c.BaseAddress = new Uri("http://localhost:5001");
    c.DefaultRequestHeaders.Add("Authorization", "Bearer token-demo");
});

var app = builder.Build();

app.MapPost("/sincronizar", async (IHttpClientFactory factory) => 
{ 
    var client = factory.CreateClient("empleados");
    var empleados = await client.GetFromJsonAsync<List<EmpleadoDto>>("/empleados");
    return Results.Ok(new { leido = empleados!.Count, empleados });
});

app.Run("http://localhost:5003");

record EmpleadoDto(int Id, string Nombre, string Area, int SueldoBase);

