using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientesApi.Data; // Asegúrate de que la ruta sea correcta para tu contexto
using Microsoft.Data.SqlClient;
using System.Data;

//caso 2 - mediante Entity Framework Core (Linq). con paginacion
[ApiController]
[Route("api/[controller]")]
public class Clientes2Controller : ControllerBase
{
    private readonly AppDbContext _context;

    public Clientes2Controller(AppDbContext context)
    {
        _context = context;
    }

    // Método GET con paginación
    [HttpGet]
    public async Task<IActionResult> GetClientes2(int pageNumber = 1, int pageSize = 10)
    {
        // Validar que los parámetros sean positivos
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("El número de página y el tamaño de página deben ser mayores que 0.");
        }

        // Calcula el número de saltos (Skip) y la cantidad de resultados (Take)
        var clientes = await _context.Clientes
            //.Where(c => c.PaisOrigen == "Chile")  // Ejemplo de filtro
            .OrderBy(c => c.Nombre)  // Ejemplo de orden
            .Skip((pageNumber - 1) * pageSize)  // Salta los elementos anteriores
            .Take(pageSize)  // Toma solo los elementos de la página actual
            .ToListAsync();  // Ejecuta la consulta

        // Obtener el total de clientes para incluir en la respuesta
        var totalClientes = await _context.Clientes
            //.Where(c => c.PaisOrigen == "Chile")
            .CountAsync();

        // Crear el objeto de respuesta con paginación
        var paginacion = new
        {
            TotalItems = totalClientes,
            TotalPages = (int)Math.Ceiling(totalClientes / (double)pageSize),
            CurrentPage = pageNumber,
            PageSize = pageSize,
            Clientes = clientes
        };

        return Ok(paginacion);  // Devuelve los datos con información de paginación
    }
}
