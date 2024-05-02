using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using FacturasAxoft.Repositorio;

namespace FacturasAxoft.Validaciones
{
    /// <summary>
    /// En esta clase implementarán todas las validaciones.
    /// Se da una validación ya implementada a modo de ejemplo.
    /// </summary>
    public class ValidadorFacturasAxoft
    {
        private readonly List<Cliente> clientes;
        private readonly List<Articulo> articulos;
        private readonly List<Factura> facturas;


        /// <summary>
        /// Instancia un Validador facturas
        /// </summary>
        /// <param name="clientes">Clientes preexistentes, ya grabados en la base de datos</param>
        /// <param name="articulos">Artículos preexistentes, ya grabados en la base de datos</param>
        /// <param name="facturas">Facturas preexistentes, ya grabadas en la base de datos</param></param>
        public ValidadorFacturasAxoft(List<Cliente> clientes, List<Articulo> articulos, List<Factura> facturas)
        {
            this.clientes = clientes;
            this.articulos = articulos;
            this.facturas = facturas;
        }

        /// <summary>
        /// Valida la factura pasada por parámetro según lo lógica de negocios requerida.
        /// </summary>
        /// <param name="factura">Factura a validar</param>
        /// <exception>En caso de que la factura no cumpla con las reglas de negocio requeridas
        /// debe lanzar una excepción con el mensaje de error correspondiente</exception>/// 
        public void ValidarNuevaFactura(Factura factura)
        {

            decimal totalRenglonesCalculado = 0;
            if (factura.Numero <= 0)
            {
                throw new FacturaConNumeroInvalidoException();
            }

          

            if (facturas.Any(f => f.Fecha > factura.Fecha))
            {
                throw new FacturaConFechaInvalidaException();
            }

            if (factura.Cliente.Cuil.Length != 11)
            {
                throw new CUILInvalidoException();
            }

            var facturasExistente = facturas.FirstOrDefault(c => c.Cliente.Cuil == factura.Cliente.Cuil);


            if (facturasExistente != null)
            {

                if (facturasExistente.Cliente.Nombre != factura.Cliente.Nombre ||
                    facturasExistente.Cliente.Direccion != factura.Cliente.Direccion ||
                    Convert.ToInt32(facturasExistente.IVA) != factura.IVA)
                {
                    throw new ClienteInvalidoException();
                }
            }
       

            foreach (var renglon in factura.Renglones)
            {

                var articulosExistente = articulos.FirstOrDefault(c => c.CodigoArticulo == renglon.Articulo.CodigoArticulo);

             

                if (articulosExistente != null)
                {
                    if (articulosExistente.Descripcion != renglon.Articulo.Descripcion ||
                     articulosExistente.PrecioUnitario != renglon.Articulo.PrecioUnitario)
                    {

                        throw new ArticulosInvalidosException();
                    }
                }

                totalRenglonesCalculado += renglon.Articulo.PrecioUnitario * renglon.Cantidad;

                if (renglon.Articulo.PrecioUnitario * renglon.Cantidad != renglon.Total)
                {
                    throw new TotalRenglonInvalidoException();
                }

            }

            if (totalRenglonesCalculado != factura.TotalSinImpuestos)
            {
                throw new TotalSinImpuestosInvalidoException();
            }


            if (totalRenglonesCalculado + (totalRenglonesCalculado * (factura.IVA / 100)) != factura.TotalConImpuestos)
            {
                throw new TotalConImpuestosInvalidoException();
            }

            if (!(new List<decimal> { 0, 10.5m, 21, 27 }).Contains(factura.IVA))
            {
                throw new IVAInvalidoException();
            }

            if (totalRenglonesCalculado * (factura.IVA / 100) != factura.ImporteIVA)
            {
                throw new IVAInvalidoException();
            }
        }


    }
}
