using System.ComponentModel.DataAnnotations;

namespace Prueba_tecnica.Entidades
{
    public class Producto
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string SKU { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;
    }
}
