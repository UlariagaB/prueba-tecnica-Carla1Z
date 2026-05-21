# Desafío Técnico: WMS Backend

Bienvenido/a al desafio tecnico. Para respetar tu tiempo, hemos preparado este proyecto, no necesitas configurar bases de datos ni armar la arquitectura desde cero.

##  Contexto
Estamos desarrollando un módulo para un **Sistema de Gestión de Almacenes (WMS)**.
Necesitamos habilitar el registro de Recepción de Mercadería.
El sistema ya cuenta con un proyecto .NET 8 configurado con una base de datos en memoria (Entity Framework Core) que se autocompleta con Productos y Ubicaciones al iniciar.

##  Tu Misión
Debes implementar la lógica dentro del archivo `Services/IRecepcionService.cs` para que el endpoint de creación funcione correctamente. Existen reglas de negocio **obligatorias y excluyentes** que debes cumplir:

### Regla 1: Validacion o Creacion del Producto
El payload recibe un `SKU` (string) y una `Cantidad` (int). 
* **Validación:** Debes validar a nivel de código los atributos del producto que vienen del payload.
* **Creación:** Si el `SKU` ingresado ya existe en la base de datos, asocia la recepción a ese producto. **Si no existe, debes instanciarlo y guardarlo** en la misma transacción.


### Regla 2: Validación de Capacidad Física
Antes de guardar la recepción en la base de datos, debes validar que la `Cantidad` ingresada no supere el espacio disponible en la ubicación seleccionada. 
* Si hay espacio, debes guardar la recepción y actualizar la `OcupacionActual` de la ubicación.

### Regla 3: Trazabilidad Financiera (Redundancia Obligatoria)
La mercadería ingresa con un valor en USD, pero necesitamos guardarlo convertido a la moneda local en tiempo real. 
* **Intento Primario:** Debes consumir una API pública de cotizaciones mediante `HttpClient` (ej. `https://api.exchangerate-api.com/v4/latest/USD`).
* **Contingencia:** Los sistemas logísticos no pueden detenerse. Si la primera API falla debes consumir automáticamente una segunda API de respaldo (por ejemplo, la API pública de Binance o DolarHoy).

## Instrucciones de Ejecución
1. Clona el repositorio y abre la solución en tu IDE de preferencia Visual Studio.
2. Ejecuta el proyecto. Se abrirá automáticamente **Swagger** en tu navegador.
3. El proyecto usa una base de datos en memoria (`.UseInMemoryDatabase`) que se reinicia en cada ejecución.
4. Implementa el código en `RecepcionService.cs`. Evita modificar la estructura de las entidades a menos que lo consideres estrictamente necesario para tu solución.
5. Prueba tu lógica utilizando el endpoint `POST /api/recepciones` desde Swagger.
6. Puedes usar el endpoint `GET /api/recepciones/datos-prueba` para ver los IDs de productos y ubicaciones pre-cargados.


**Tiempo estiamdo:** 1 hora 
¡Éxitos!