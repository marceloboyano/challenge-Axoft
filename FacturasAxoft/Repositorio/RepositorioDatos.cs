using FacturasAxoft.Clases;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturasAxoft.Repositorio
{
    public class RepositorioDatos
    {
        private readonly string connectionString;

        public RepositorioDatos(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// trae todos los clientes de la base de datos
        /// </summary>
        /// <returns></returns>
        public List<Cliente> ObtenerClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

           
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Cliente";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Cliente cliente = new Cliente
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Cuil = reader["cuil"].ToString(),
                        Nombre = reader["nombre"].ToString(),
                        Direccion = reader["direccion"].ToString()
                    };

                    clientes.Add(cliente);
                }

                reader.Close();
            }

            return clientes;
        }

        /// <summary>
        /// trae todos los articulos de la base de datos
        /// </summary>
        /// <returns></returns>
        public List<Articulo> ObtenerArticulos()
        {
        List<Articulo> articulos = new List<Articulo>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Articulo";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Articulo articulo = new Articulo
                {
                    Id = Convert.ToInt32(reader["id"]),
                    CodigoArticulo = reader["codigoArticulo"].ToString(),
                    Descripcion = reader["descripcion"].ToString(),
                    PrecioUnitario = Convert.ToDecimal(reader["precioUnitario"])
                };

                articulos.Add(articulo);
            }

            reader.Close();
        }

        return articulos;
    }
        /// <summary>
        /// Trae todas las facturas de la base de datos
        /// </summary>
        /// <returns></returns>
        public List<Factura> ObtenerFacturas()
        {
            List<Factura> facturas = new List<Factura>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Factura f inner join Cliente c on f.id_cliente = c.id";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Factura factura = new Factura
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Numero = Convert.ToInt32(reader["numero"]),
                            Fecha = Convert.ToDateTime(reader["fecha"]),
                            Cliente = new Cliente
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Cuil = reader["CUIL"].ToString(), 
                                Nombre = reader["nombre"].ToString(),
                                Direccion = reader["direccion"].ToString()
                            },
                            TotalSinImpuestos = Convert.ToDecimal(reader["totalSinImpuestos"]),
                            IVA = Convert.ToDecimal(reader["IVA"]),
                            ImporteIVA = Convert.ToDecimal(reader["importeIVA"]),
                            TotalConImpuestos = Convert.ToDecimal(reader["totalConImpuestos"])
                        };

                        facturas.Add(factura);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron facturas.");
                }

                reader.Close();
            }

            return facturas;
        }
        /// <summary>
        /// Devuelve el Total del importe IVA  en un rango de fechas
        /// </summary>
        /// <param name="fechaDesde"></param>
        /// <param name="fechaHasta"></param>
        /// <returns></returns>
        public decimal TotalIva(DateTime fechaDesde, DateTime fechaHasta)
        {
            decimal totalIva = 0;

            string fechaDesdeFormatted = fechaDesde.ToString("yyyy-MM-dd HH:mm:ss");
            string fechaHastaFormatted = fechaHasta.ToString("yyyy-MM-dd HH:mm:ss");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT SUM(importeIVA) AS total_importe_iva
                    FROM Factura
                    WHERE fecha BETWEEN @fechaDesde AND @fechaHasta";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
                command.Parameters.AddWithValue("@fechaHasta", fechaHasta);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        totalIva = reader.GetDecimal(reader.GetOrdinal("total_importe_iva"));
                    }
                }

                reader.Close();
            }

            return totalIva;
        }

        /// <summary>
        /// Los 3 clientes que mas compraron un articulo especifico
        /// </summary>
        /// <param name="codigoArticulo"></param>
        /// <returns></returns>
        public List<ClienteMasComprador> Top3ClientesDeArticulo(string codigoArticulo)
        {
            List<ClienteMasComprador> topClientes = new List<ClienteMasComprador>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT TOP 3 c.id AS id_cliente, c.nombre AS nombre_cliente, SUM(r.cantidad) AS total_comprado
            FROM Factura f
            INNER JOIN Cliente c ON f.id_cliente = c.id
            INNER JOIN RenglonFactura r ON f.id = r.id_factura
            INNER JOIN Articulo a ON r.id_articulo = a.id
            WHERE a.codigoArticulo = @codigoArticulo
            GROUP BY c.id, c.nombre
            ORDER BY total_comprado DESC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@codigoArticulo", codigoArticulo);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ClienteMasComprador cliente = new ClienteMasComprador
                    {
                        IdCliente = Convert.ToInt32(reader["id_cliente"]),
                        NombreCliente = reader["nombre_cliente"].ToString(),
                        TotalComprado = Convert.ToInt32(reader["total_comprado"])
                    };
                    topClientes.Add(cliente);
                }
                reader.Close();
            }

            return topClientes;
        }

        /// <summary>
        /// Total y promedio facturado por fecha
        /// </summary>
        /// <returns></returns>
        public (decimal totalFacturado, decimal promedioImportes) TotalYPromedioFacturadoPorFecha(DateTime fecha)
        {
            decimal totalFacturado = 0;
            decimal totalImportes = 0;
            int totalFacturas = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryTotalFacturado = @"
            SELECT SUM(totalConImpuestos) AS total_facturado
            FROM Factura
            WHERE CAST(fecha AS DATE) = @fecha";

                string queryTotalImportes = @"
            SELECT SUM(totalConImpuestos) AS total_importes, COUNT(*) AS total_facturas
            FROM Factura
            WHERE CAST(fecha AS DATE) = @fecha";

                using (SqlCommand commandTotalFacturado = new SqlCommand(queryTotalFacturado, connection))
                using (SqlCommand commandTotalImportes = new SqlCommand(queryTotalImportes, connection))
                {
                    commandTotalFacturado.Parameters.AddWithValue("@fecha", fecha);
                    commandTotalImportes.Parameters.AddWithValue("@fecha", fecha);

                    try
                    {
                        connection.Open();

                        totalFacturado = Convert.ToDecimal(commandTotalFacturado.ExecuteScalar());

                        SqlDataReader readerTotalImportes = commandTotalImportes.ExecuteReader();
                        if (readerTotalImportes.HasRows)
                        {
                            while (readerTotalImportes.Read())
                            {
                                if (!readerTotalImportes.IsDBNull(readerTotalImportes.GetOrdinal("total_importes")))
                                {
                                    totalImportes = readerTotalImportes.GetDecimal(readerTotalImportes.GetOrdinal("total_importes"));
                                }
                                if (!readerTotalImportes.IsDBNull(readerTotalImportes.GetOrdinal("total_facturas")))
                                {
                                    totalFacturas = readerTotalImportes.GetInt32(readerTotalImportes.GetOrdinal("total_facturas"));
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        
                        Console.WriteLine($"Algo ha salido mal!!: {ex.Message}");
                        throw;
                    }
                }
            }


            decimal promedioImportes = totalFacturas > 0 ? totalImportes / totalFacturas : 0;

            return (totalFacturado, promedioImportes);
        }

        /// <summary>
        /// obtiene  el articulo mas comprado por cliente
        /// </summary>
        /// <returns></returns>
        public (int idArticulo, string nombreArticulo, int totalComprado) ArticuloMasCompradoPorCliente(string cuil)
        {
            int idArticuloMasComprado = 0;
            string nombreArticuloMasComprado = "";
            int totalCompradoArticuloMasComprado = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT TOP 1 r.id_articulo, a.descripcion AS nombre_articulo, SUM(r.cantidad) AS total_comprado
            FROM Factura f
            INNER JOIN Cliente c ON f.id_cliente = c.id
            INNER JOIN RenglonFactura r ON f.id = r.id_factura
            INNER JOIN Articulo a ON r.id_articulo = a.id
            WHERE c.CUIL = @cuil
            GROUP BY r.id_articulo, a.descripcion
            ORDER BY total_comprado DESC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@cuil", cuil);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        idArticuloMasComprado = Convert.ToInt32(reader["id_articulo"]);
                        nombreArticuloMasComprado = reader["nombre_articulo"].ToString();
                        totalCompradoArticuloMasComprado = Convert.ToInt32(reader["total_comprado"]);
                        break; 
                    }
                }

                reader.Close();
            }

            return (idArticuloMasComprado, nombreArticuloMasComprado, totalCompradoArticuloMasComprado);
        }




        /// <summary>
        /// obtiene  el promedio de compras por cliente
        /// </summary>
        /// <returns></returns>
        public decimal PromedioComprasPorCliente(string cuil)
        {

            decimal promedio = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT CAST(SUM(r.cantidad) AS DECIMAL(10, 2)) / COUNT(f.id) AS promedio_compras
            FROM Factura f
            INNER JOIN Cliente c ON f.id_cliente = c.id
            INNER JOIN RenglonFactura r ON f.id = r.id_factura
            WHERE c.CUIL = @cuil
                ";

                SqlCommand command = new SqlCommand(query, connection);
               
                command.Parameters.AddWithValue("@cuil", cuil); 

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        promedio = Convert.ToDecimal(reader["promedio_compras"]);
                    }
                }
               
                reader.Close();
            }

            return promedio;
        }


        /// <summary>
        /// obtiene los 3 clientes que mas compraron
        /// </summary>
        /// <returns></returns>
        public List<ClienteMasComprador> ObtenerClientesQueMasCompraron()
        {
            List<ClienteMasComprador> clientesMasCompradores = new List<ClienteMasComprador>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 3 c.id AS id_cliente, c.nombre AS nombre_cliente, SUM(r.cantidad) AS total_comprado
                        FROM Factura f
                        INNER JOIN Cliente c ON f.id_cliente = c.id
                        INNER JOIN RenglonFactura r ON f.id = r.id_factura
                        GROUP BY c.id, c.nombre
                        ORDER BY total_comprado DESC";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ClienteMasComprador cliente = new ClienteMasComprador
                        {
                            IdCliente = Convert.ToInt32(reader["id_cliente"]),
                            NombreCliente = reader["nombre_cliente"].ToString(),
                            TotalComprado = Convert.ToInt32(reader["total_comprado"])
                        };

                        clientesMasCompradores.Add(cliente);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron clientes que más compran.");
                }

                reader.Close();
            }

            return clientesMasCompradores;
        }
        /// <summary>
        /// Obtiene los 3 articulos mas vendidos
        /// </summary>
        /// <returns></returns>
        public List<ArticuloMasVendido> ObtenerArticulosMasVendidos()
        {
            List<ArticuloMasVendido> articulosMasVendidos = new List<ArticuloMasVendido>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @$"SELECT TOP 3 a.codigoArticulo, SUM(r.cantidad) AS totalVendido 
                               FROM Factura f 
                               INNER JOIN RenglonFactura r ON f.id = r.id_factura
                               INNER JOIN Articulo a ON r.id_articulo = a.id 
                               GROUP BY a.codigoArticulo 
                               ORDER BY totalVendido DESC";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ArticuloMasVendido articulo = new ArticuloMasVendido
                        {
                            CodigoArticulo = reader["codigoArticulo"].ToString(),
                            CantidadVendida = Convert.ToInt32(reader["totalVendido"])
                        };

                        articulosMasVendidos.Add(articulo);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron artículos vendidos.");
                }

                reader.Close();
            }

            return articulosMasVendidos;
        }

        /// <summary>
        /// Verifica si ya existe el cliente para guardarlo
        /// </summary>
        /// <param name="cliente"></param>
        public void GuardarClienteSiNoExiste(Cliente cliente)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Cliente WHERE Cuil = @Cuil";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Cuil", cliente.Cuil);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                if (count == 0)
                {
                    query = "INSERT INTO Cliente (Cuil, Nombre, Direccion) VALUES (@Cuil, @Nombre, @Direccion)";
                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Cuil", cliente.Cuil);
                    command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// verifica si el articulo ya existe
        /// </summary>
        /// <param name="articulo"></param>
        public void GuardarArticuloSiNoExiste(Articulo articulo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Articulo WHERE CodigoArticulo = @CodigoArticulo";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CodigoArticulo", articulo.CodigoArticulo);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                if (count == 0)
                {
                    query = "INSERT into Articulo (CodigoArticulo, Descripcion, PrecioUnitario) VALUES (@CodigoArticulo, @Descripcion, @PrecioUnitario)";
                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CodigoArticulo", articulo.CodigoArticulo);
                    command.Parameters.AddWithValue("@Descripcion", articulo.Descripcion);
                    command.Parameters.AddWithValue("@PrecioUnitario", articulo.PrecioUnitario);
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// obtiene cliente por cuil 
        /// </summary>
        /// <param name="cuil"></param>
        /// <returns></returns>
        public Cliente ObtenerClientePorCUIL(string cuil)
        {
            Cliente cliente = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select * FROM Cliente WHERE Cuil = @Cuil";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Cuil", cuil);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    cliente = new Cliente
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Cuil = reader["cuil"].ToString(),
                        Nombre = reader["nombre"].ToString(),
                        Direccion = reader["direccion"].ToString()
                    };
                }

                reader.Close();
            }

            return cliente;
        }
        /// <summary>
        /// obtiene un articulo por su codigo
        /// </summary>
        /// <param name="codigoArticulo"></param>
        /// <returns></returns>
        public Articulo ObtenerArticuloPorCodigo(string codigoArticulo)
        {
            Articulo articulo = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "Select * From Articulo WHERE CodigoArticulo = @CodigoArticulo";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CodigoArticulo", codigoArticulo);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    articulo = new Articulo
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        CodigoArticulo = reader["codigoArticulo"].ToString(),
                        Descripcion = reader["descripcion"].ToString(),
                        PrecioUnitario = Convert.ToDecimal(reader["precioUnitario"])
                    };
                }

                reader.Close();
            }

            return articulo;
        }

        /// <summary>
        /// Guarda los renglones de la factura
        /// </summary>
        /// <param name="factura"></param>
        public void GuardarRenglonesFactura(Factura factura)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var renglon in factura.Renglones)
                {
                    string query = @"Insert into RenglonFactura (id_factura, id_articulo, cantidad, total) 
                             VALUES (@IdFactura, @IdArticulo, @Cantidad, @Total)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IdFactura", factura.Id);                 
                    command.Parameters.AddWithValue("@IdArticulo", renglon.Articulo.Id); 
                    command.Parameters.AddWithValue("@Cantidad", renglon.Cantidad);
                    command.Parameters.AddWithValue("@Total", renglon.Total);

                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Graba la factura
        /// </summary>
        /// <param name="factura"></param>
        /// <returns></returns>
        public int GrabarFactura(Factura factura)
        {
            int facturaId = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"Insert into Factura (numero, fecha, id_Cliente, totalSinImpuestos, IVA, importeIVA, totalConImpuestos) 
                 VALUES (@Numero, @Fecha, @Id_Cliente, @TotalSinImpuestos, @IVA, @ImporteIVA, @TotalConImpuestos);
                 SELECT SCOPE_IDENTITY();"; 

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Numero", factura.Numero);
                command.Parameters.AddWithValue("@Fecha", factura.Fecha);
                command.Parameters.AddWithValue("@Id_Cliente", factura.Cliente.Id);
                command.Parameters.AddWithValue("@TotalSinImpuestos", factura.TotalSinImpuestos);
                command.Parameters.AddWithValue("@IVA", factura.IVA);
                command.Parameters.AddWithValue("@ImporteIVA", factura.ImporteIVA);
                command.Parameters.AddWithValue("@TotalConImpuestos", factura.TotalConImpuestos);

                connection.Open();
                facturaId = Convert.ToInt32(command.ExecuteScalar()); 
            }

            return facturaId; 
        }
    }
}
