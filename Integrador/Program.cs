var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("empleados", c => 
{
    c.BaseAddress = new Uri("http://localhost:5001");
    c.DefaultRequestHeaders.Add("Authorization", "Bearer token-demo");
});

builder.Services.AddHttpClient("remuneraciones", c =>
{
    c.BaseAddress = new Uri("http://localhost:5002");
    c.DefaultRequestHeaders.Add("Authorization", "Bearer token-demo");
});

var app = builder.Build();

app.MapPost("/sincronizar", async (IHttpClientFactory factory) => 
{
    var clienteEmpleados = factory.CreateClient("empleados");
    var clienteRemuneraciones = factory.CreateClient("remuneraciones");

    var empleados = await clienteEmpleados.GetFromJsonAsync<List<EmpleadoDto>>("/empleados");

    var resultados = new List<ResultadoDto>();

    foreach (var e in empleados!) 
    {
        var respuesta = await clienteRemuneraciones.PostAsJsonAsync("/liquidaciones", new { empleadoId = e.Id, sueldoBase = e.SueldoBase });

        resultados.Add(new ResultadoDto(e.Id, e.Nombre, (int)respuesta.StatusCode));
    }

    return Results.Ok(resultados);
});

app.Run("http://localhost:5003");

record EmpleadoDto(int Id, string Nombre, string Area, int SueldoBase);

record ResultadoDto(int EmpleadoId, string Nombre, int Estado);

