using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientesApi.Data; // Asegúrate de que la ruta sea correcta para tu contexto
using Microsoft.Data.SqlClient;
using System.Data;

//caso 1 - mediante llamado a sp
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ClientesController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

	[HttpGet]
	public async Task<IActionResult> GetClientes(int pageNumber = 1, int pageSize = 10)
	{
		var clientes = new List<Cliente>();

		using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
		{
			using (var command = new SqlCommand("sp_ObtenerClientes", connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				await connection.OpenAsync();
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						clientes.Add(new Cliente
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Email = reader.GetString(2),
							Telefono = reader.GetString(3),
							PaisOrigen = reader.GetString(4)
						});
					}
				}
			}
		}

		// Aplicar paginación en memoria
		var totalItems = clientes.Count;
		var items = clientes
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		var result = new
		{
			TotalItems = totalItems,
			totalPages = pageNumber,
			currentPage = 1,
			PageSize = pageSize,
			clientes = items
		};

		return Ok(result);
	}

}
