using System.ComponentModel.DataAnnotations;

namespace Prueba_tecnica.Entidades
{
    public class Ubicacion
    {
        public int Id { get; set; }
        public string CodigoUbicacion { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public int OcupacionActual { get; set; }
    }
}

