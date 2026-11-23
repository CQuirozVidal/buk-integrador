# buk-integrador

Un mini integrador que estoy armando en C# y ASP.NET Core (.NET 10) para practicar cómo conversan los sistemas entre sí a través de APIs REST.

La idea: dos sistemas simulados que no se conocen entre ellos (empleados y remuneraciones) y, más adelante, un servicio que los conecta. Todo corre en local, sin nube y sin base de datos real: los datos viven en listas en memoria y son inventados.

## Cómo está armado

```mermaid
flowchart LR
    E["EmpleadosApi<br/>:5001"] --> I["Integrador<br/>(pendiente)"]
    I --> R["RemuneracionesApi<br/>:5002"]
    style I stroke-dasharray: 5 5
```

## Avance

- [x] **EmpleadosApi**: listar empleados y buscar uno por id
- [x] **RemuneracionesApi**: crear y consultar liquidaciones
- [ ] **Integrador** que llame a las dos APIs con `HttpClient`
- [ ] Qué pasa cuando algo falla: timeouts, errores 500 intermitentes, 429 y reintentos
- [ ] Colección de pruebas en Postman y cURL

## Endpoints

**EmpleadosApi** (`http://localhost:5001`)

| Método | Ruta | Respuesta |
|---|---|---|
| GET | `/empleados` | 200 con la lista |
| GET | `/empleados/{id}` | 200 con el empleado, o 404 si no existe |

**RemuneracionesApi** (`http://localhost:5002`)

| Método | Ruta | Respuesta |
|---|---|---|
| POST | `/liquidaciones` | 201 con la liquidación y su `Location`, o 400 si los datos no sirven |
| GET | `/liquidaciones/{id}` | 200 con la liquidación, o 404 si no existe |

Nota: el descuento de la liquidación es un 20 % fijo e inventado, solo para tener algo que calcular. No es un cálculo real de remuneraciones.

Para probar el POST desde PowerShell guardo el JSON en un archivo:

```powershell
Set-Content ok.json '{"empleadoId":1,"sueldoBase":1200000}'
curl.exe -i -X POST http://localhost:5002/liquidaciones -H "Content-Type: application/json" --data "@ok.json"
```

## Cómo correrlo

Necesitas el SDK de .NET 10. Cada API en su propia terminal:

```powershell
dotnet run --project EmpleadosApi
dotnet run --project RemuneracionesApi
```

## Códigos HTTP que uso

200 OK · 201 Created · 400 Bad Request · 404 Not Found · 405 Method Not Allowed