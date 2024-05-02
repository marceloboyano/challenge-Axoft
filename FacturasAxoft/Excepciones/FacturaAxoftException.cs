namespace FacturasAxoft.Excepciones
{
    public class FacturaAxoftException : Exception
    {
        public FacturaAxoftException(string message) : base(message)
        {
        }
    }

    public class FacturaConFechaInvalidaException : FacturaAxoftException
    {
        public FacturaConFechaInvalidaException() :
            base("La fecha de la factura es inválida. Existen facturas grabadas con fecha posterior a la ingresada.")
        {
        }
    }

    public class FacturaAnteriorNoGrabadaException : FacturaAxoftException
    {
        public FacturaAnteriorNoGrabadaException() :
            base("La factura anterior no ha sido grabada. No se puede grabar una factura nueva sin grabar la anterior.")
        {
        }
    }  

    public class FacturaConNumeroInvalidoException : FacturaAxoftException
    {
        public FacturaConNumeroInvalidoException() :
            base("La numeración de facturas comienza en 1.")
        {
        }
    }

    public class ClienteInvalidoException : FacturaAxoftException
    {
        public ClienteInvalidoException() :
            base("Un cliente siempre debe tener el mismo CUIL, nombre, dirección, y porcentaje de IVA.")
        {
        }
    }

    public class ArticulosInvalidosException : FacturaAxoftException
    {
        public ArticulosInvalidosException() :
            base("Un artículo siempre debe tener el mismo código, descripción, y precio unitario.")
        {
        }
    }
    
    public class CUILInvalidoException : FacturaAxoftException
    {
        public CUILInvalidoException() :
            base("El CUIL es incorrecto de tener 11 digitos exactos.")
        {
        }
    }

    public class TotalRenglonInvalidoException : FacturaAxoftException
    {
        public TotalRenglonInvalidoException() :
            base("Los totales de los renglones no son correctos.")
        {
        }
    }
    
    public class TotalSinImpuestosInvalidoException : FacturaAxoftException
    {
        public TotalSinImpuestosInvalidoException() :
            base("Los totales de los renglones no coinciden con el total sin impuestos de la factura.")
        {
        }
    } 
    
    public class TotalConImpuestosInvalidoException : FacturaAxoftException
    {
        public TotalConImpuestosInvalidoException() :
            base("Los totales de los renglones no coinciden con el total con impuestos de la factura.")
        {
        }
    }

    public class IVAInvalidoException : FacturaAxoftException
    {
        public IVAInvalidoException() :
            base("El porcentaje de IVA especificado en la factura es inválido o el importe del iva es incorrecto ")
        {
        }
    }
       

}
