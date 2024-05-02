# FacturasAxoft

## Challenge Axoft

### Objetivos
Te damos la bienvenida al Challenge de Axoft, el objetivo es que puedas demostrar tus conocimientos. Recordá que tenés 48hs para poder completar el desafio. 

### Aclaraciones
  El proyecto está hecho con VisualStudio, compila y ejecuta los unittests. Se sugiere usar VisualStudio o VisualStudioCode.

#### Uso de GIT
  Para resolver este ejercicio hacer fork en un repositorio privado y dar permisos al usuario AxoftRRHH para poder visualizarlo.
  
  Es deseable poder observar un uso ordenado de GIT. En particular queremos no ver sólo un commit con toda la solución ya implementada, sino que quede trazabilidad del trabajo realizado. De esta forma tener al menos un commit por cada ítem realizado, o mejor aún si hay intermedios.

  Ningún camino está libre de errores, lo esperable es que siempre en algún momento tengamos que dar algún paso atrás, si hay que revertir un commit se hace. Valoraremos de forma positiva dejar evidencia del trabajo realizado en commits parciales, no evaluaremos de forma negativa los errores en commits intermedios puesto que lo esperable es que existan.

#### Buenas prácticas en código
  Se valorará de forma positiva el uso de las buenas prácticas de código, modularización, nombres descriptivos, etc...

## Enunciado
  Se requiere la realización de un sistema que administre facturas.
  El ingreso de facturas se va a hacer desde un archivo xml, y las mismas deberán ser guardadas en una base de datos relacional SQL para luego hacer consultas.

  Se puede asumir que las facturas dentro del archivo xml están ordenadas por número de factura, no hay huecos ni repeticiones.
  Un mismo cliente siempre tiene el mismo CUIL, nombre, dirección y porcentaje de IVA en todas las facturas que se le hayan emitido.
  Un mismo artículo siempre tiene el mismo código, descripción y precio unitario, en todas las facturas en que aparezca.
  El único impuesto que aparece es el IVA.

### Diseño de base de datos
  Diseñar una base de datos que pueda complir con el requerimiento teniendo en cuenta el enunciado y los archivos xml que se dan a modo de ejemplo.
  La base de datos debe estar bien normalizada, con los índices y foreignKeys necesarios.
  Como única consideración extra, vamos a pedir que cada tabla tenga un campo id de tipo int.
  Cualquier consideración o decisión debe ser documentada.
  
  Se sugiere utilizar Microsoft SqlServer, de utilizar otro aclararlo.
  Los scripts necesarios para la creación de la base de datos con el modelo requerido deben estar en el archivo CreateDb.sql en la carpeta SQL. Al ejecutar el contenido sobre un master debe dejar generada la base de datos lista para ser utilizada por el sistema.
  
### Grabación de datos
  Implementar la grabación de datos en el método CargarFacturas de la clase FacturasAxoft.
  El método toma como parámetro la ubicación y nombre completo del archivo xml que se va a leer, y debe volcar toda la información en la base de datos.
  Para la conexión a la base de datos se cuenta con un connectionString que es parámetro del constructor de la clase FacturasAxoft.
  Se puede utilizar cualquier herramienta provista por c#, .net, o libraries de acceso público. En caso de hacer uso de paquetes específicos no olvidar subir la referencia.

   Se permite modificar cualquier de las clases provistas que se adecúen mejor a los datos que vienen en el archivo xml o al modelo de datos requerido por el sql.

### Validaciones
  Como en cualquier sistema, antes de grabar datos es necesario validarlos.
  Para esto se provee ya de un esquema de validación que cuenta con clase ValidadorFacturasAxoft que implementa el método ValidarNuevaFactura, y una clase de unittests que verifica que la clase ValidadorFacturasAxoft funciona de manera correcta. También se dan algunos unittests ya implementados, incluyendo un caso completo de validación incluyendo la lógica que valida, la excepción, y su unittest.
  Asegurarse de utilizar el esquema de validación en el método CargarFacturas para evitar que se graben facturas que no sean válidas.
  Las validaciones deben asegurar que se cumplan las siguientes reglas:
  1. La numeración de facturas comienza en 1.
  2. Al grabar una nueva factura debe ya estar grabada la anterior. Por ejemplo, para grabar la factura 3 es requisito que ya esté grabada la 2.
  3. Las facturas están emitidas en órden cronológico, por lo que si la factura 1 tiene fecha 5 de enero, la factura 2 no puede tener fecha 4 de enero.
  4. El CUIL debe ser válido.
  5. Un mismo cliente siempre tiene el mismo CUIL, nombre, dirección, y porcentaje de IVA.
  6. Un mismo artículo siempre tiene el mismo código, descripción, y precio unitario.
  7. Los totales de los renglones son correctos.
  8. Los totales sin impuestos son correctos.
  9. Los porcentajes de IVA posibles son 0, 10.5, 21 y 27.
  10. El importe de IVA es correcto.
  11. El total con impuestos es correcto.

### Consultas
  Se requiere que el sistema resuelva algunas consultas. Todos los métodos están en FacturasAxoft listos para implementar con su summary.
  1. Top 3 de artículos mas vendidos.
  2. Top 3 de clientes que mas compraron.
  3. Promedio de compras y artículo mas comprado de un cliente.
  4. Total facturado y promedio de importes de factura para una fecha.
  5. Top 3 de clientes que mas compraron un artículo.
  6. Total de IVA para un rango de fechas.
  
