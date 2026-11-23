var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var liquidaciones = new List<Liquidacion>();

app.MapPost("/liquidaciones", (SolicitudLiquidacion s) => 
{
    if (s.EmpleadoId <= 0 || s.SueldoBase <= 0)
        return Results.BadRequest(new { error = "empleadoId y sueldoBase deben ser mayores que cero" });

    var descuentos = (int)(s.SueldoBase * 0.20);
    var liq = new Liquidacion(liquidaciones.Count + 1, s.EmpleadoId, s.SueldoBase, descuentos, s.SueldoBase - descuentos);
    liquidaciones.Add(liq);

    return Results.Created($"/liquidaciones/{liq.Id}", liq);
});

app.MapGet("/liquidaciones/{id:int}", (int id) => 
{
    var liq = liquidaciones.FirstOrDefault(l => l.Id == id);
    return liq is null
        ? Results.NotFound(new { error = $"La liquidación {id} no existe" })
        : Results.Ok(liq);
});

app.Run("http://localhost:5002");

record SolicitudLiquidacion(int EmpleadoId, int SueldoBase);
record Liquidacion(int Id, int EmpleadoId, int SueldoBase, int Descuentos, int Liquido);