using Microsoft.EntityFrameworkCore;
using Prueba_tecnica.Contexto;
using Prueba_tecnica.Entidades;
using Prueba_tecnica.Entidades.Dto;
using System;
using System.Globalization;
using System.Text.Json;
using System.IO;

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
            Log($"Iniciando recepción para SKU: {request?.SKU}");
            // TODO: 1. Validar existencia del Producto
            if (request == null)
                throw new InvalidOperationException("La recepción no puede estar vacía.");

            // Validaciones básicas de los campos, antes de intentar guardar o consultar

            // obligatorio porque es la referencia principal para identificar el producto
            if (string.IsNullOrWhiteSpace(request.SKU))
                throw new InvalidOperationException("El SKU es obligatorio.");

            // Esta validacion proviene de la anotación [MaxLength(20)] en la entidad Producto, se valida antes de intentar guardar
            if (request.SKU.Length > 20)
                throw new InvalidOperationException("¡Atención! El SKU NO puede superar los 20 caracteres.");

            if (string.IsNullOrWhiteSpace(request.NombreProducto))
                throw new InvalidOperationException("El nombre del producto es obligatorio.");

            // Esta validacion proviene de la anotación [MaxLength(50)] en la entidad Producto, se valida antes de intentar guardar
            if (request.NombreProducto.Length > 50)
                throw new InvalidOperationException("¡Atención! El nombre del producto NO puede superar los 50 caracteres.");

            if (request.Cantidad <= 0)
                throw new InvalidOperationException("La cantidad debe ser mayor a cero.");

            if (request.ValorDeclaradoUSD <= 0)
                throw new InvalidOperationException("El valor declarado en USD debe ser mayor a cero.");

            // busco si el producto ya existe por su SKU, si no existe lo creo
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.SKU == request.SKU);

            if (producto != null)
            {
                Log($"Producto existente encontrado. SKU: {producto.SKU}");
            }

            if (producto == null)
            {
                producto = new Producto
                {
                    SKU = request.SKU.Trim(),
                    Nombre = request.NombreProducto.Trim()
                };

                _context.Productos.Add(producto);
                Log($"Producto nuevo creado. SKU: {producto.SKU}");
            }

            // TODO: 2. Validar que la capacidad de la Ubicación sea suficiente.

            // validar que la ubicacion exista, para luego validar su capacidad
            // si no existe la ubicacion no se debe validar su capacidad
            var ubicacion = await _context.Ubicaciones.FirstOrDefaultAsync(u => u.Id == request.UbicacionId);

            if (ubicacion == null)
                throw new InvalidOperationException("La ubicación indicada no existe.");

            Log($"Validando capacidad para ubicación: {ubicacion.CodigoUbicacion}");
            var espacioDisponible = ubicacion.CapacidadMaxima - ubicacion.OcupacionActual;

            if (request.Cantidad > espacioDisponible)
            {
                Log($"Capacidad insuficiente en ubicación {ubicacion.CodigoUbicacion}. Espacio disponible: {espacioDisponible}");

                throw new InvalidOperationException("La cantidad ingresada supera el espacio disponible de la ubicación.");
            }


            // TODO: 3. Obtener cotización de moneda mediante API externa.
            // objego la cotizacion en tiempo real, primero con una api principal, en caso de que falle uso una de respaldo
            var cotizacion = await ObtenerCotizacionDolarAsync();

            var valorMonedaLocal = request.ValorDeclaradoUSD * cotizacion;

            // TODO: 4. Actualizar la ocupación de la Ubicación.
            ubicacion.OcupacionActual += request.Cantidad;
            Log($"Ocupación actualizada para ubicación {ubicacion.CodigoUbicacion}. Nueva ocupación: {ubicacion.OcupacionActual}");

            // TODO: 5. Guardar el nuevo registro de Recepción en la base de datos y retornarlo.
            //throw new NotImplementedException("Error");
            var recepcion = new Recepcion
            {
                Producto = producto,
                UbicacionId = request.UbicacionId,
                Cantidad = request.Cantidad,
                ValorDeclaradoUSD = request.ValorDeclaradoUSD,
                ValorMonedaLocal = valorMonedaLocal,
                FechaRecepcion = DateTime.UtcNow
            };

            _context.Recepciones.Add(recepcion);

            await _context.SaveChangesAsync();
            Log($"Recepción registrada correctamente. SKU: {producto.SKU} | Cantidad: {request.Cantidad}");

            return recepcion;
        }

        #region OBTENER COTIZACION DOLAR
        private async Task<decimal> ObtenerCotizacionDolarAsync()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                // Intento Primario
                Log("Consultando cotización desde API principal.");
                var response = await client.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    using var document = JsonDocument.Parse(json);

                    var cotizacion = document.RootElement
                        .GetProperty("rates")
                        .GetProperty("ARS")
                        .GetDecimal();

                    return cotizacion;
                    Log($"Cotización obtenida desde API principal: {cotizacion}");
                }
            }
            catch (Exception ex)
            {
                Log($"Error API principal: {ex.Message}");
            }

            try
            {
                // Segundo intento: Contingencia
                Log("API principal falló. Consultando API de respaldo.");
                var response = await client.GetAsync("https://api.binance.com/api/v3/ticker/price?symbol=USDTARS");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    using var document = JsonDocument.Parse(json);

                    var precio = document.RootElement
                        .GetProperty("price")
                        .GetString();

                    // Binance devuelve el precio como string, por eso se parsea
                    if (decimal.TryParse(precio, NumberStyles.Any, CultureInfo.InvariantCulture, out var cotizacion))
                    {
                        Log($"Cotización obtenida desde API respaldo: {cotizacion}");

                        return cotizacion;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error API respaldo: {ex.Message}");
            }

            throw new InvalidOperationException("No se pudo obtener la cotización del dólar desde las APIs configuradas.");
        }
        #endregion

        #region LOG
        private void Log(string msg)
        {
            try
            {
                // Creo una carpeta simple para ir guardando logs diarios
                string folderName = "Logs";

                string routeFolder = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    folderName
                );

                if (!Directory.Exists(routeFolder))
                {
                    Directory.CreateDirectory(routeFolder);
                }

                string fileName = $"Logs_{DateTime.Now:yyyy-MM-dd}.txt";

                string routeFile = Path.Combine(routeFolder, fileName);

                File.AppendAllText(
                    routeFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {msg}{Environment.NewLine}"
                );
            }
            catch (Exception)
            {
                // Si falla el log no quiero romper el flujo principal
            }
        }
        #endregion
    }
}
