var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) => 
{
    var auth = context.Request.Headers.Authorization.ToString();
    if (auth != "Bearer token-demo") 
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { error = "Token faltante o inválido" });
        return;
    }
    await next();
});

var empleados = new List<Empleado>
{
    new(1, "Ana Pérez", "Contabilidad", 1200000),
    new(2, "Luis Soto", "Operaciones", 950000),
    new(3, "Marta Rojas", "TI", 1500000),
};

app.MapGet("/empleados", () => empleados);

app.MapGet("/empleados/{id:int}", (int id) =>
{
    var empleado = empleados.FirstOrDefault(e => e.Id == id);

    return empleado is null
        ? Results.NotFound(new { error = $"El empleado {id} no existe" })
        : Results.Ok(empleado);
});

app.Run("http://localhost:5001");

record Empleado(int Id, string Nombre, string Area, int SueldoBase);