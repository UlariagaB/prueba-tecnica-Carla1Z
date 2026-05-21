using Microsoft.EntityFrameworkCore;
using Prueba_tecnica.Contexto;
using Prueba_tecnica.Entidades;
using Prueba_tecnica.Entidades.Dto;
using System;
using System.Globalization;
using System.Text.Json;

namespace Prueba_tecnica.Servicio
{
    public interface IRecepcionService
    {
        Task<Recepcion> ProcesarRecepcionAsync(RecepcionDto request);
    }
    public class RecepcionService : IRecepcionService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public RecepcionService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Recepcion> ProcesarRecepcionAsync(RecepcionDto request)
        {
            // TODO: 1. Validar existencia del Producto

            // TODO: 2. Validar que la capacidad de la Ubicación sea suficiente.

            // TODO: 3. Obtener cotización de moneda mediante API externa.

            // TODO: 4. Actualizar la ocupación de la Ubicación.

            // TODO: 5. Guardar el nuevo registro de Recepción en la base de datos y retornarlo.

            throw new NotImplementedException("Error");

        }
    }
}
