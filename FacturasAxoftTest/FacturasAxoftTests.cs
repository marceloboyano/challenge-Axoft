using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using FacturasAxoft.Validaciones;
using Xunit;
using Xunit.Sdk;

namespace FacturasAxoftTest
{
    [TestClass]
    public class FacturasAxoftTests
    {
        private readonly List<Cliente> clientes;
        private readonly List<Articulo> articulos;
        private readonly List<Factura> facturas;
        private readonly ValidadorFacturasAxoft validador;

        /// <summary>
        /// Copmletar con los tests necesarios para verificar toda la funcionalidad requerida.
        /// Puede ser necesario modificar los tests preexistentes para que corran corractamente al implementar el resto de la solución.
        /// </summary>
        public FacturasAxoftTests()
        {
            clientes = new List<Cliente>();
            articulos = new List<Articulo>();
            facturas = new List<Factura>();

            validador = new ValidadorFacturasAxoft(clientes, articulos, facturas);
        }

        /// <summary>
        /// La primer factura a ingresar, con número 1 es válida.
        /// </summary>
        [TestMethod]
        public void PimerFacturaEsValida()
        {
            // No tengo facturas preexistentes

            // La primer factura que voy a agregar tiene el número 1
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020,1,1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            CodigoArticulo  = "ART01",
                            Descripcion = "artículo cero uno",
                            PrecioUnitario  = 10
                        },
                        Cantidad = 2
                    }
                }                
            };

            // La factura es válida, no tiene que tirar la excepción.
            Exception exception = Record.Exception(() => validador.ValidarNuevaFactura(factura));
            Assert.IsNull(exception);
        }

        /// <summary>
        /// La primer factura a ingresar, con número 2 es válida.
        /// </summary>
        [TestMethod]
        public void SegundaFacturaEsValida()
        {
            // Tengo preexistente una factura número 1 con fecha uno de enero
            facturas.Add(new()
                {
                    Numero = 1,
                    Fecha = new DateTime(2020, 1, 1),
                    Cliente = new Cliente
                    {
                        Cuil = "20123456781",
                        Direccion = "Calle falsa 123",
                        Nombre = "Juan"
                    },
                    Renglones = new List<RenglonFactura>()
                    {
                        new RenglonFactura
                        {
                            Articulo = new Articulo()
                            {
                                CodigoArticulo    = "ART01",
                                Descripcion = "artículo cero uno",
                                PrecioUnitario = 10
                            },
                            Cantidad = 2
                        }
                    }
                }
            );

            // Tengo una nueva factura nro dos con fecha 1 de enero
            Factura factura = new()
            {
                Numero = 2,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            CodigoArticulo    = "ART01",
                            Descripcion = "artículo cero uno",
                            PrecioUnitario = 10
                        },
                        Cantidad = 2
                    }
                }
            };

            // La factura es válida, no tiene que tirar la excepción.
            Exception exception = Record.Exception(() => validador.ValidarNuevaFactura(factura));
            Assert.IsNull(exception);
        }

        /// <summary>
        /// Este test verifica si tengo una factura con número 1 y fecha 2 de enero no pueda ingregar la factura nro 2 con fecha
        /// 1 de enero porque no estaría respetando el órden cronológico.
        /// </summary>
        [TestMethod]
        public void FacturaConFechaInvalida()
        {
            // Tengo una factura número 1 con fecha dos de enero
            facturas.Add(new()
                {
                    Numero = 1,
                    Fecha = new DateTime(2020, 1, 2),
                    Cliente = new Cliente
                    {
                        Cuil = "20123456781",
                        Direccion = "Calle falsa 123",
                        Nombre = "Juan"
                    },
                    Renglones = new List<RenglonFactura>()
                    {
                        new RenglonFactura
                        {
                            Articulo = new Articulo()
                            {
                                CodigoArticulo    = "ART01",
                                Descripcion = "artículo cero uno",
                                PrecioUnitario = 10
                            },
                            Cantidad = 2
                        }
                    }
                }
            );

            // Voy a querer ageegar la factura número 2 con fecha 1 de enero
            Factura factura = new()
            {
                Numero = 2,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            CodigoArticulo  = "ART01",
                            Descripcion = "artículo cero uno",
                            PrecioUnitario = 10
                        },
                        Cantidad = 2
                    }
                }
            };

            // Al validar la nueva factura salta una excepción tipada, y con el mensaje de error correspondiente.
            Assert.ThrowsException<FacturaConFechaInvalidaException>( () => validador.ValidarNuevaFactura(factura),
                "La fecha de la factura es inválida. Existen facturas grabadas con número inferior y fecha posterior a la ingresada.");
        }
    }
}