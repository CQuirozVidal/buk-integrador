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
    c.Timeout = TimeSpan.FromSeconds(2); // Configura un timeout de 2 segundos  
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
        try 
        { 
            var respuesta = await clienteRemuneraciones.PostAsJsonAsync("/liquidaciones", new { EmpleadoId = e.Id, SueldoBase = e.SueldoBase });
            resultados.Add(new ResultadoDto(e.Id, e.Nombre, (int)respuesta.StatusCode));
        }
        catch (TaskCanceledException)
        {
            resultados.Add(new ResultadoDto(e.Id, e.Nombre, 504)); // En caso de error, se registra un estado 504   
        }
        catch (HttpRequestException)
        {
            resultados.Add(new ResultadoDto(e.Id, e.Nombre, 503)); // En caso de error, se registra un estado 503
        }
    }

    return Results.Ok(new {
        leidos = empleados.Count, 
        creadas = resultados.Count(r => r.Estado == 201),
        fallidas = resultados.Count(r => r.Estado != 201),
        resultados
    });

});

app.Run("http://localhost:5003");

record EmpleadoDto(int Id, string Nombre, string Area, int SueldoBase);

record ResultadoDto(int EmpleadoId, string Nombre, int Estado);

