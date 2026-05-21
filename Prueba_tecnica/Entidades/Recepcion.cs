namespace Prueba_tecnica.Entidades
{
    public class Recepcion
    {
        public Producto Producto { get; set; }
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int UbicacionId { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;
        public decimal ValorDeclaradoUSD { get; set; }
        public decimal ValorMonedaLocal { get; set; }
    }
}
