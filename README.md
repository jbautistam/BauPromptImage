**[Bau Image Prompt Generator](https://jbautistam.github.io/docs/prompt-image/prompt-image-generator/)** es una aplicación WPF 
para generación de imágenes con inteligencia artifical utilizando diferentes API.

Actualmente, utiliza la API de [Stable Horde](https://stablehorde.net/). **Stable Horde** utiliza un cluster distribuido para la generación
de imágenes, no es tan rápido como otras APIs pero proporciona una gran cantidad de modelos de forma gratuita siempre que no te importe
esperar.

Al abrir la aplicación se nos muestra un archivo vacío:

![Bau Image Prompt Generator](https://jbautistam.github.io/docs/prompt-image/images/bau-prompt-images-generator-1.png)

En la parte superior está la barra de herramientas donde podemos crear un archivo, abrir un archivo existente, grabar las modificaciones o
añadir / quitar versiones al archivo.

Al crear una versión de archivo, se copian todos los datos para permitirnos hacer modificaciones o probar con otros modelos.

En la sección de *Prompt* vemos las opciones para la creación de imágenes.

* **Generator:** es el generador que vamos a utilizar. La aplicación está pensada para utilizar diferentes generadores aunque por ahora sólo
existen dos: *Horde* para generación de imágenes utilizando **Stable Horde** y *Fake* que realmente es un generador que utilizo para pruebas
y que simplemente descarga un par de imágenes (es decir, no es un generador real).
* **Models:** es la lista de modelos que podemos utilizar en el generador seleccionado para crear nuestras imágenes.
* **Positive:** es el texto positivo que se va a utilizar para crear la imagen, es decir el texto que define nuestra imagen.
* **Negative:** es el texto negativo, es decir, lo que no queremos que aparezca en la imagen. Normalmente se utiliza para ajustar y evitar
errores en la imagen creada.

En la sección de *Parameters* se encuentran los parámetros de generación:

* **Widht y Height:** ancho y alto de las imágenes generadas.
* **Images to generate:** es el número de imágenes que deseamos crear al mismo tiempo.
* **Steps:** es el número de pasos que vamos a utilizar para generar la imagen.
* **Cfg scale:** es cómo queremos que la IA interprete nuestro prompt, cuánto más alto sea este valor, más se ajustará la imagen al prompt.
* **Denoising strenght:**
* **Sampler:** sampleador utilizado.
* **Postprocessing:** herramientas de postproceso a utilizar.
* **Karras:** indica si queremos añadir un ruido adicional a la generación de imágenes.
* **NSFW:** indica si permitimos que la IA genere imágenes "delicadas".

Una vez introducido el *prompt* y seleccionados los parámetros, podemos pulsar el botón `Generate` de la barra de herramientas para comenzar
el proceso.

Esta proceso puede tardar un tiempo, durante ese periodo, la API nos irá informando de la posición de la orden en la cola, del número de elementos
procesados y de los errores. Veremos este progreso en la parte inferior de la aplicación:

![Bau Image Prompt Generator](https://jbautistam.github.io/docs/prompt-image/images/bau-prompt-images-generator-2.png)

Una vez que la API indique que se han terminado de procesar las imágenes, la aplicación las descarga y nos la muestra
en una lista:

![Bau Image Prompt Generator](https://jbautistam.github.io/docs/prompt-image/images/bau-prompt-images-generator-3.png)

Si pulsamos dos veces sobre la imagen se nos abrirá esta imagen en una ventana adicional.

Las imágenes se descargan en el mismo lugar en el que se encuentra el archivo de versiones con un nombre secuencial que se corresponde con el nombre
de archivo y el código de versión.

La pestaña *Ideas* que se encuentra agrupada con la lista de imágenes, nos muestra un árbol de categorías con textos que 
podemos añadir a nuestro prompt:

![Bau Image Prompt Generator: ideas](https://jbautistam.github.io/docs/prompt-image/images/bau-prompt-images-generator-4.png)

incluye categorías como estilos de dibujos, nombres de artistas, nombres de cámaras y lentes o puntos de vista, etc...

## Proyectos

La solución se divide en varios proyectos:

![Bau Image Prompt Generator: proyectos](https://jbautistam.github.io/docs/prompt-image/images/bau-prompt-images-generator-5.png)

Contiene dos partes, en la raíz de la solución:

* **BauPromptImage.Application:** manejador de la aplicación
* **BauPromptImage.ViewModels:** view models de la aplicación
* **BauPromptImage.Desktop:** proyecto WPF con el ejecutable de la aplicación.

En la carpeta *Domain*:

* **LibAiImageGeneration:** librería de control de los diferentes generadores de imágenes. Es la
encargada de mandar las órdenes a la API del proveedor seleccionado, controlar el proceso de generación
y descargar las imágenes una vez generadas.
* **LibAiImageGeneration.Domain:** interfaces y clases de dominio que deben cumplir los proveedores de IA.
* **LibStableHorde.Api:** librería de control y manejo de la API de *Stable Horde*. Implementa las interfaces
definidas en el dominio de la librería anterior.
* **LibFakeAi.Api:** librería de control de una API *fake* que utilizo para hacer pruebas de interface. Implementa
las interfaces de `LibAiImagenGeneration.Domain` pero simplemente espera un tiempo antes de descargar imágenes
predefinidas (o lanzar un error).

## Uso de LibAiImageGeneration

Todo el trabajo de la aplicación realmente se basa en las llamadas a `LibAiImageGeneration`, realmente el resto del proyecto de escritorio
maneja esta librería y trata sus resultados.

Para poder utilizarla, primero debemos añadir los proveedores que serán nuestra implementaciones de `LibAiImageGeneration.Domain`:

```csharp
ImageGenerationManager = new Bau.Libraries.LibAiImageGeneration.ImageGenerationManager();

	// Añade los proveedores
	ImageGenerationManager.AddProvider("Horde", new Bau.Libraries.LibStableHorde.Api.StableHordeManager(new Uri("https://stablehorde.net"), "0000000000"));
	ImageGenerationManager.AddProvider("Fake", new Bau.Libraries.LibFakeAi.Api.FakeAiManager());
	// Conecta con los proveedores (descarga los modelos)
	await ImageGenerationManager.ConnectAsync(cancellationToken);
```

**Nota:** la inicialización del proveedor de *Stable Horde* utiliza la API Key `0000000000` que es la clave genérica de uso. En este momento, la aplicación
no admite configuración.

Una vez definido el *manager*, podemos agregar un manejador de eventos para que, una vez lancemos una tarea, nos avise del progreso de la misma:

```csharp
ImageGenerationManager.Progress += (sender, args) => UpdateProgress(args.Generation);
```

Para lanzar una nueva generación a un proveedor, debemos llamar al método `PromptAsync` del *manager*:

```csharp
PromptRequestModel prompt = new(provider);
PromptResultModel result = await ImageGenerationManager.AiProvidersManager.PromptAsync(prompt, cancellationToken);
		 
	if (!result.CanProcess || string.IsNullOrWhiteSpace(result.Id))
		MainViewModel.HostController.SystemController.ShowMessage($"Can't execute the prompt{Environment.NewLine}{result.Message}");
	else
		TasksListViewModel.Add(promptVersionViewModel, prompt.Positive, result.Id);
```

**Nota:** esta es una versión simplificada sacada del proyecto de ViewModels.

`PromptRequestModel` es la clase con los datos del prompt, incluye el proveedor, los textos positivos y negativos, el modelo seleccionado, etc...

El método `PromptAsync` llama al proveedor de IA adecuado para indicarle que inicie el proceso, nos devuelve un resultado que
incluye el `Id` del proceso en caso que la llamada sea correcta (el proveedor puede rechazarla porque la imagen sea demasiado grande, tenga demasiados
pasos o simplemente porque en ese momento no pueda ejecutarla).

A partir de ese momento, el *manager* se encarga de llamar cada cierto tiempo a la API y controlar el proceso. Cuando acaba, simplemente descarga las
imágenes generadas.

La llamada a `TasksListViewModel.Add(...)` que aparece en el código anterior, es para mostrar al usuario el progreso en la lista de tareas de la aplicación
de escritorio, realmente no es necesaria para el *manager* que simplemente lanza eventos según va avanzando en el proceso.

