using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using FacturasAxoft.Repositorio;
using FacturasAxoft.Validaciones;
using System.Globalization;
using System.Xml;

namespace FacturasAxoft
{
    public class FacturasAxoft
    {
        private readonly string connectionString;
        private readonly RepositorioDatos repositorio;
        /// <summary>
        /// Instancia un FacturasAxoft que usaremos como fachada de la aplicación.
        /// </summary>
        /// <param name="conectionString">Datos necesarios para conectarse a la base de datos</param>
        /// <exception>Debe tirar una excepción con mensaje de error correspondiente en caso de no poder conectar a la base de datos</exception>
        public FacturasAxoft(string connectionString)
        {
            this.repositorio = new RepositorioDatos(connectionString);
        }

        /// <summary>
        /// Lee las facturas desde el archivo XML y las graba en la base de datos.
        /// Da de alta los clientes o los artículos que lleguen en el xml y no estén en la base de datos.
        /// </summary>
        /// <param name="path">Ubicación del archivo xml que contiene las facturas</param>
        /// <exception>Si no se puede acceder al archivo, no es un xml válido, o no cumple con las reglas de negocio, 
        /// devuelve una excepción con el mensaje de error correspondiente</exception>/// 
        public void CargarFacturas(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNodeList facturasList = xmlDoc.SelectNodes("//factura");

            foreach (XmlNode facturaElement in facturasList)
            {

                Factura factura = new Factura();

                factura.Numero = Convert.ToInt32(facturaElement.SelectSingleNode("numero").InnerText);
                factura.Fecha = Convert.ToDateTime(facturaElement.SelectSingleNode("fecha").InnerText);
                factura.Cliente = new Cliente
                {
                    Cuil = facturaElement.SelectSingleNode("cliente/CUIL").InnerText,
                    Nombre = facturaElement.SelectSingleNode("cliente/nombre").InnerText,
                    Direccion = facturaElement.SelectSingleNode("cliente/direccion").InnerText
                };
                var culture = new CultureInfo("es-CO");
                XmlNodeList renglones = facturaElement.SelectNodes("renglones/renglon");
                foreach (XmlNode renglonNode in renglones)
                {
                    string PrecioUnitario = renglonNode.SelectSingleNode("precioUnitario").InnerText.Trim();
                    PrecioUnitario = PrecioUnitario.Replace(".", ",");
                    string Total = renglonNode.SelectSingleNode("total").InnerText.Trim();
                    Total = Total.Replace(".", ",");

                    RenglonFactura renglon = new RenglonFactura
                    {
                        Articulo = new Articulo
                        {
                            CodigoArticulo = renglonNode.SelectSingleNode("codigoArticulo").InnerText,
                            Descripcion = renglonNode.SelectSingleNode("descripcion").InnerText,
                            PrecioUnitario = Convert.ToDecimal(PrecioUnitario)

                        },
                        Cantidad = Convert.ToInt32(renglonNode.SelectSingleNode("cantidad").InnerText),
                        Total = Convert.ToDecimal(Total)
                    };

                    factura.Renglones.Add(renglon);
                }


                string totalSinImpuestosStr = facturaElement.SelectSingleNode("totalSinImpuestos").InnerText.Trim();
                string ivaStr = facturaElement.SelectSingleNode("iva").InnerText.Trim();
                string importeIvaStr = facturaElement.SelectSingleNode("importeIva").InnerText.Trim();
                string totalConImpuestosStr = facturaElement.SelectSingleNode("totalConImpuestos").InnerText.Trim();




                totalSinImpuestosStr = totalSinImpuestosStr.Replace(".", ",");
                ivaStr = ivaStr.Replace(".", ",");
                importeIvaStr = importeIvaStr.Replace(".", ",");
                totalConImpuestosStr = totalConImpuestosStr.Replace(".", ",");


                factura.TotalSinImpuestos = Convert.ToDecimal(totalSinImpuestosStr);
                factura.IVA = decimal.Parse(ivaStr);
                factura.ImporteIVA = decimal.Parse(importeIvaStr);
                factura.TotalConImpuestos = decimal.Parse(totalConImpuestosStr);




                List<Cliente> clientes = repositorio.ObtenerClientes();
                List<Articulo> articulos = repositorio.ObtenerArticulos();
                List<Factura> facturas = repositorio.ObtenerFacturas();


                ValidadorFacturasAxoft validador = new ValidadorFacturasAxoft(clientes, articulos, facturas);

                validador.ValidarNuevaFactura(factura);

                repositorio.GuardarClienteSiNoExiste(factura.Cliente);
                factura.Cliente = repositorio.ObtenerClientePorCUIL(factura.Cliente.Cuil);
                foreach (var renglon in factura.Renglones)
                {
                    repositorio.GuardarArticuloSiNoExiste(renglon.Articulo);
                    renglon.Articulo = repositorio.ObtenerArticuloPorCodigo(renglon.Articulo.CodigoArticulo);
                }

                int facturaId = repositorio.GrabarFactura(factura);
                factura.Id = facturaId;
                repositorio.GuardarRenglonesFactura(factura);
            }
        }

        /// <summary>
        /// Obtiene los 3 artículos mas vendidos
        /// </summary>
        /// <returns>JSON con los 3 artículos mas vendidos</returns>
        /// <exception>Nunca devuelve excepción, en caso de no existir 3 artículos vendidos devolver los que existan, en caso de
        /// tener artículos con la misma cantidad de ventas devolver cualquiera</exception>
        public string Get3ArticulosMasVendidos()
        {
            var articulosMasVendidos = repositorio.ObtenerArticulosMasVendidos();
            string jsonResult = "{ \"articulos_mas_vendidos\": [";
            foreach (var articulo in articulosMasVendidos)
            {
                jsonResult += $"{{ \"codigo_articulo\": \"{articulo.CodigoArticulo}\", \"cantidad_vendida\": {articulo.CantidadVendida} }}, ";
            }
            jsonResult = jsonResult.TrimEnd(',', ' ') + "] }";

            return jsonResult;
        }

        /// <summary>
        /// Obtiene los 3 clientes que mas compraron
        /// </summary>
        /// <returns>JSON con los 3 clientes que mas compraron</returns>
        /// <exception>Mismo criterio que para artículos</exception>
        public string Get3Compradores()
        {
            var clientesMasCompradores = repositorio.ObtenerClientesQueMasCompraron();
            string jsonResult = "{ \"clientes_mas_compradores\": [";
            foreach (var cliente in clientesMasCompradores)
            {
                jsonResult += $"{{ \"id_cliente\": \"{cliente.IdCliente}\", \"nombre_cliente\": \"{cliente.NombreCliente}\", \"total_comprado\": {cliente.TotalComprado} }}, ";
            }
            jsonResult = jsonResult.TrimEnd(',', ' ') + "] }";

            return jsonResult;
        }

        /// <summary>
        /// Devuelve el promedio de facturas y el artículo que mas compro.
        /// </summary>
        /// <param name="cuil"></param>
        /// <returns>JSON con los datos requeridos</returns>
        /// <exception>Si no existe el cliente, o si no tiene compras devolver un mensaje de error con el mensaje correspondiente</exception>
        public string GetPromedioYArticuloMasCompradoDeCliente(string cuil)
        {
            var promedioComprasPorCliente = repositorio.PromedioComprasPorCliente(cuil);
            var articuloMasCompradoPorCliente = repositorio.ArticuloMasCompradoPorCliente(cuil);

            string jsonResult = $"{{ \"cuil\": \"{cuil}\", \"promedio_compras\": {promedioComprasPorCliente}, \"articulo_mas_comprado\": {{ \"id_articulo\": \"{articuloMasCompradoPorCliente.idArticulo}\", \"nombre_articulo\": \"{articuloMasCompradoPorCliente.nombreArticulo}\", \"total_comprado\": {articuloMasCompradoPorCliente.totalComprado} }} }}";

            return jsonResult;
        }


        /// <summary>
        /// Devuelve el total y promedio facturado para una fecha.
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns>JSON con los datos requeridos</returns>
        /// <exception>Si el dato de fecha ingresado no es válido, o si no existen facturas para la fecha dada,
        /// mostrar el mensaje de error correspondiente</exception>
        public string GetTotalYPromedioFacturadoPorFecha(DateTime fecha)
        {
            var totalYPromedioFacturadoPorFecha = repositorio.TotalYPromedioFacturadoPorFecha(fecha);

            string jsonResult = $"{{ \"total_facturado\": {totalYPromedioFacturadoPorFecha.totalFacturado},\"promedio_importes\": {totalYPromedioFacturadoPorFecha.promedioImportes} }} }}";

            return jsonResult;
        }


        /// <summary>
        /// Devuelve los 3 clientes que mas compraron el artículo
        /// </summary>
        /// <param name="codigoArticulo"></param>
        /// <returns>JSON con los datos pedidos</returns>
        /// <exception>Si el artículo no existe, o no fue comprado por al menos 3 clientes devolver un mensaje de error correspondiente</exception>
        public string GetTop3ClientesDeArticulo(string codigoArticulo)
        {
            var top3ClientesDeArticulo = repositorio.Top3ClientesDeArticulo(codigoArticulo);

            string jsonResult = "{ \"top_3_clientes_de_articulo\": [";

            foreach (var cliente in top3ClientesDeArticulo)
            {
                jsonResult += $"{{ \"id_cliente\": \"{cliente.IdCliente}\", \"nombre_cliente\": \"{cliente.NombreCliente}\", \"total_comprado\": {cliente.TotalComprado} }}, ";
            }

            jsonResult = jsonResult.TrimEnd(',', ' ') + "] }";

            return jsonResult;
        }

        /// <summary>
        /// Devuelve el total de IVA de las facturas generadas desde la fechaDesde hasta la fechaHasta, ambas inclusive.
        /// </summary>
        /// <returns>JSON con el dato requerido</returns>
        /// <exception>Si no existen facturas para las fechas ingresadas mostrar un mensaje de error</exception>
        public string GetTotalIva(DateTime fechaDesde, DateTime fechaHasta)
        {
            var totalIva = repositorio.TotalIva(fechaDesde, fechaHasta);

            string jsonResult = $"{{ \"total_importe_iva\": {totalIva} }}";

            return jsonResult;
        }
    }
}
