// See https://aka.ms/new-console-template for more information
using System.Globalization;

Console.WriteLine("Inicio: Facturas Axoft");

string connectionString = "Server=MSB;Database=FacturasAxoft;Integrated Security=True;";
string metodo = "CargarFacturas";
string path = @"C:\FacturasAxoft\XML\Ejemplo.xml";
string cuil = "20345678901";
string fecha = "2020-12-31";
string codigoArticulo = "AR003";
string fechaDesdeStr = "2020-12-31";
string fechaHastaStr = "2021-01-01";

Console.WriteLine($"connectionString: {connectionString}");
FacturasAxoft.FacturasAxoft facturasAxoft = new(connectionString);

//Console.WriteLine($"metodo: {metodo}");
string result = "OK";
switch (metodo)
{
	case "CargarFacturas":
  //      string path= args[2];
        Console.WriteLine($"path: {path}");
        facturasAxoft.CargarFacturas(path);
        break;
    case "Get3ArticulosMasVendidos":
        result = facturasAxoft.Get3ArticulosMasVendidos();
        break;
    case "Get3Compradores":
        result = facturasAxoft.Get3Compradores();
        break;
    case "GetPromedioYArticuloMasCompradoDeCliente":
     //   string cuil = args[2];
        Console.WriteLine($"cuil: {cuil}");
        result = facturasAxoft.GetPromedioYArticuloMasCompradoDeCliente(cuil);
        break;
    case "GetTotalYPromedioFacturadoPorFecha":
      //  string fecha = args[2];
        Console.WriteLine($"fecha: {fecha}");
        facturasAxoft.GetTotalYPromedioFacturadoPorFecha(Convert.ToDateTime(fecha));
        break;
    case "GetTop3ClientesDeArticulo":
        //string codigoArticulo = args[2];
        Console.WriteLine($"codigoArticulofecha: {codigoArticulo}");
        result = facturasAxoft.GetTop3ClientesDeArticulo(codigoArticulo);
        break;
    case "GetTotalIva":
        // string fechaDesde = args[2];
        // string fechaHasta = args[3];

        DateTime fechaDesde = DateTime.ParseExact(fechaDesdeStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime fechaHasta = DateTime.ParseExact(fechaHastaStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        fechaDesde = fechaDesde.AddHours(0).AddMinutes(0).AddSeconds(0);
        fechaHasta = fechaHasta.AddHours(23).AddMinutes(59).AddSeconds(59);


        Console.WriteLine($"fechaDesde: {fechaDesde}");
        Console.WriteLine($"fechaHasta: {fechaHasta}");
        result = facturasAxoft.GetTotalIva(fechaDesde, fechaHasta);
        break;
    default:
		break;
}
Console.WriteLine($"result: {result}");
Console.WriteLine("Fin: Facturas Axoft");
