namespace FacturasAxoft.Clases
{
    /// <summary>
    /// Clase que representa a una factura.
    /// Puede que sea necesario modificarla para hacer las implementaciones requeridas.
    /// </summary>
    public class Factura
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public Cliente Cliente { get; set; }
        public List<RenglonFactura> Renglones { get; set; } = new List<RenglonFactura>();
        public decimal TotalSinImpuestos { get; set; }
        public decimal IVA { get; set; }
        public decimal ImporteIVA { get; set; }
        public decimal TotalConImpuestos { get; set; }

    }

    /// <summary>
    /// Clase que representa el renglon de una factura.
    /// Puede que sea necesario modificarla para hacer las implementaciones requeridas.
    /// </summary>
    public class RenglonFactura
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public Articulo Articulo { get; set; }
    }
}
