using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prueba_tecnica.Contexto;
using Prueba_tecnica.Entidades.Dto;
using Prueba_tecnica.Servicio;
using System;

namespace Prueba_tecnica.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RecepcionesController : ControllerBase
    {
        private readonly IRecepcionService _recepcionService;
        private readonly AppDbContext _context;

        public RecepcionesController(IRecepcionService recepcionService, AppDbContext context)
        {
            _recepcionService = recepcionService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarRecepcion([FromBody] RecepcionDto request)
        {
            try
            {
                var result = await _recepcionService.ProcesarRecepcionAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }


        [HttpGet("datos-prueba")]
        public async Task<IActionResult> GetDatosPrueba()
        {
            var productos = await _context.Productos.ToListAsync();
            var ubicaciones = await _context.Ubicaciones.ToListAsync();
            return Ok(new { Productos = productos, Ubicaciones = ubicaciones });
        }
    }

}
