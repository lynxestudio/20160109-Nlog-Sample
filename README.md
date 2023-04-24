# Utilizando NLog con MonoDevelop en .NET

Para cualquier aplicación de software a nivel producción es indispensable tener un componente que escriba los eventos más significativos en una bitácora. Esta acción que se conoce como logging (escribir en la bitácora) se define técnicamente como:

“Una forma sistemática y controlada para obtener el estado de una aplicación en tiempo de ejecución.”

Pensando en esto, los diseñadores de .NET incorporaron un mecanismo de logging de forma predeterminada dentro del ensamblado System.Diagnostics, en clases como Trace, TraceListener, Swicth y todas sus clases derivadas. Aunque el empleo de estas clases es efectivo, no deja de ser rudimentario y carecer de muchas funcionalidades que terminan siendo un límite.

Teniendo en cuenta esto, en el ecosistema .NET han surgido a lo largo de los años una cantidad de componentes propietarios y opensource para el logging de aplicaciones. Dentro de ese conjunto hay un componente que se destacado y es del que trataré de resumir en este tutorial: Nlog.

¿Qué es NLog?
Nlog (http://nlog-project.org/) es un componente open source de logging para .NET, que entre sus características se encuentran:

Muy fácil de configurar.
Extremadamente personalizable con plantilla (layouts)
Altamente extensible
Hay tres características que se deben conocer antes de empezar su utilización:

Targets: Se utilizan para enviar los mensajes hacia otro destino, entiéndase aquí un archivo, una base de datos, un email, la consola, un webservice, etc.
Layouts: Con los comandos de layout podemos definir la estructura o el molde de como acomodar la información escrita en un determinado target.
Levels: Es una forma de asignar una prioridad al mensaje, los niveles permitidos son los siguientes:
Fatal: Sucedió algo que causo que el todo el sistema entero falle, se debe detener la ejecución.
Error: Un problema ha ocurrido pero no es fatal, el sistema puede seguir funcionando.
Warn: Un problema ha ocurrido, pero puede ser recuperable.
Info: Mensajes de información muy útiles como cambios de estado, login/logout, etc.
Debug: Se utiliza durante el desarrollo del sistema.
Trace: Para indicar el inicio y final de una rutina.
Como un primer acercamiento a su utilización, escribí una aplicación de consola en C# que solicita una cadena de conexión, con esta cadena trata de conectase a una base de datos PostgreSQL, si la cadena de conexión no es correcta se utiliza Nlog para enviar la excepción a consola, si logra conectarse solicita una consulta SELECT para ejecutar y mostrar los resultados en la consola. Aquí de nuevo si ocurre una excepción utiliza Nlog para notificarla.

1-. Ejecutar Monodevelop y seleccionar un proyecto de consola y nombrarlo como HelloNlog

Fig. 1 crear un proyecto llamado HelloNlog



2-. Dentro del Solution Explorer has click con el botón derecho y has click en Add Packages , entonces aparecerá la pantalla Add Packages, ya en esa pantalla usa el buscador para encontrar el paquete Nlog, y seleccionar los paquetes: Nlog, Nlog Configuration y Npgsql respectivamente, presionar el botón Add Packages para agregar los ensamblados al proyecto.

Fig. 2 seleccionar los paquetes Nlog y Nlog Configuration



3-. Ahora que ya se tiene una estructura en la solución como se muestra en la siguiente imagen:
Fig. 3 la estructura de la solución con los ensamblados.



4-. Agregar al proyecto una clase llamada NloggerWrapper
5-. Bien ahora hay que completar el código de la clase Program.
6-. Antes de ejecutar la solución es muy importante editar el archivo Nlog.conf para agregar targets (objetivos), layouts (disposición) y rules (reglas). Aquí el código del archivo Nlog.conf del proyecto.


Targets
En este archivo de configuración defino tres targets, el primero hacia un archivo, el segundo hacia una consola con salida de color y el último hacia una consola de salida normal.



Para ver completa la lista de targets consultar el enlace: https://github.com/NLog/NLog/wiki/Targets
Layouts
Por cierta comodidad y porque así es la manera predeterminada de ver la información, puse los layouts de la siguiente manera:



Se especifica un layout por cada uno de los targets Para más información de los layouts ver el siguiente enlace: https://github.com/NLog/NLog/wiki/Layout-Renderers

Rules
Ahora la configuración para las rules, aquí con el '*' le indico que ese nivel se use para todos los logs, únicamente en los niveles Error y Fatal escriban hacia el target llamado fileLog que es el target que escribe hacia un archivo de texto, en la segunda regla indico igual que para todos logs, los niveles Trace se escriban hacia el log llamado consoleLog que tiene la salida normal de consola y por último le indico que para los niveles Warn y Info escriban hacia el log de la consola con colores.



Para más información de las reglas ver el siguiente enlace: https://github.com/nlog/NLog/wiki/Configuration-file#rules

8-. Antes de ejecutar el programa, en el Solution Explorer haz click derecho sobre la solución después haz click en options, aparecerá la ventana Project Options ahí seleccionar las opciones run on external console y pause console output.
Fig. 4 opciones para ejecutar el proyecto.



Bien al ejecutar el programa este solicita una cadena de conexión desde el inicio:
Fig. 5 el programa solicita una cadena de conexión.



Errores como si la cadena de conexión no tiene un formato correcto, el servidor Postgresql esta abajo o no existe la base de datos, etc. Son encerrados dentro por un bloque try/catch y enviados al método LogException para que Nlog utilice el level correspondiente y lo mande hacia el target. Aquí el código del método LogException dentro de la clase NLoggerWrapper

    public static void LogException(Exception ex)
  {
   if (ex is ArgumentException)
    logger.Warn (ex.Message);
   else
   if (ex is NpgsqlException)
    logger.Error (ex.Message);
   else
    logger.Fatal (ex.Message);
  }
En este código dependiendo del tipo de excepción utiliza un nivel (level) de Nlog para escribir.
Fig. 6 el programa con Nlog muestra las excepciones en la consola con color..



Fig. 7 excepción atrapada por Nlog que se muestra en color.



Cuando uno de los target como en este ejemplo esta dirigido a escribir en archivo se puede revisar la creación del archivo y posteriormente su contenido.

Fig. 8 la creación de los archivos log con el target File.



Fig. 9 el formato de los archivos log.



Fig. 10 la ejecucción del programa sin errores

