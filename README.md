**[Bau Image Prompt Generator](https://jbautistam.github.io/docs/prompt-image/prompt-image-generator/)** es una aplicaci�n WPF 
para generaci�n de im�genes con inteligencia artifical utilizando diferentes API.

Actualmente, utiliza la API de [Stable Horde](https://stablehorde.net/). **Stable Horde** utiliza un cluster distribuido para la generaci�n
de im�genes, no es tan r�pido como otras APIs pero proporciona una gran cantidad de modelos de forma gratuita siempre que no te importe
esperar.

Al abrir la aplicaci�n se nos muestra un archivo vac�o:

![Bau Image Prompt Generator](/docs/prompt-image/images/bau-prompt-images-generator-1.png)

En la parte superior est� la barra de herramientas donde podemos crear un archivo, abrir un archivo existente, grabar las modificaciones o
a�adir / quitar versiones al archivo.

Al crear una versi�n de archivo, se copian todos los datos para permitirnos hacer modificaciones o probar con otros modelos.

En la secci�n de *Prompt* vemos las opciones para la creaci�n de im�genes.

* **Generator:** es el generador que vamos a utilizar. La aplicaci�n est� pensada para utilizar diferentes generadores aunque por ahora s�lo
existen dos: *Horde* para generaci�n de im�genes utilizando **Stable Horde** y *Fake* que realmente es un generador que utilizo para pruebas
y que simplemente descarga un par de im�genes (es decir, no es un generador real).
* **Models:** es la lista de modelos que podemos utilizar en el generador seleccionado para crear nuestras im�genes.
* **Positive:** es el texto positivo que se va a utilizar para crear la imagen, es decir el texto que define nuestra imagen.
* **Negative:** es el texto negativo, es decir, lo que no queremos que aparezca en la imagen. Normalmente se utiliza para ajustar y evitar
errores en la imagen creada.

En la secci�n de *Parameters* se encuentran los par�metros de generaci�n:

* **Widht y Height:** ancho y alto de las im�genes generadas.
* **Images to generate:** es el n�mero de im�genes que deseamos crear al mismo tiempo.
* **Steps:** es el n�mero de pasos que vamos a utilizar para generar la imagen.
* **Cfg scale:** es c�mo queremos que la IA interprete nuestro prompt, cu�nto m�s alto sea este valor, m�s se ajustar� la imagen al prompt.
* **Denoising strenght:**
* **Sampler:** sampleador utilizado.
* **Postprocessing:** herramientas de postproceso a utilizar.
* **Karras:** indica si queremos a�adir un ruido adicional a la generaci�n de im�genes.
* **NSFW:** indica si permitimos que la IA genere im�genes "delicadas".

Una vez introducido el *prompt* y seleccionados los par�metros, podemos pulsar el bot�n `Generate` de la barra de herramientas para comenzar
el proceso.

Esta proceso puede tardar un tiempo, durante ese periodo, la API nos ir� informando de la posici�n de la orden en la cola, del n�mero de elementos
procesados y de los errores. Veremos este progreso en la parte inferior de la aplicaci�n:

![Bau Image Prompt Generator](/docs/prompt-image/images/bau-prompt-images-generator-2.png)

Una vez que la API indique que se han terminado de procesar las im�genes, la aplicaci�n las descarga y nos la muestra
en una lista:

![Bau Image Prompt Generator](/docs/prompt-image/images/bau-prompt-images-generator-3.png)

Si pulsamos dos veces sobre la imagen se nos abrir� esta imagen en una ventana adicional.

Las im�genes se descargan en el mismo lugar en el que se encuentra el archivo de versiones con un nombre secuencial que se corresponde con el nombre
de archivo y el c�digo de versi�n.

La pesta�a *Ideas* que se encuentra agrupada con la lista de im�genes, nos muestra un �rbol de categor�as con textos que 
podemos a�adir a nuestro prompt:

![Bau Image Prompt Generator: ideas](/docs/prompt-image/images/bau-prompt-images-generator-4.png)

incluye categor�as como estilos de dibujos, nombres de artistas, nombres de c�maras y lentes o puntos de vista, etc...

## Proyectos

La soluci�n se divide en varios proyectos:

![Bau Image Prompt Generator: proyectos](/docs/prompt-image/images/bau-prompt-images-generator-5.png)

Contiene dos partes, en la ra�z de la soluci�n:

* **BauPromptImage.Application:** manejador de la aplicaci�n
* **BauPromptImage.ViewModels:** view models de la aplicaci�n
* **BauPromptImage.Desktop:** proyecto WPF con el ejecutable de la aplicaci�n.

En la carpeta *Domain*:

* **LibAiImageGeneration:** librer�a de control de los diferentes generadores de im�genes. Es la
encargada de mandar las �rdenes a la API del proveedor seleccionado, controlar el proceso de generaci�n
y descargar las im�genes una vez generadas.
* **LibAiImageGeneration.Domain:** interfaces y clases de dominio que deben cumplir los proveedores de IA.
* **LibStableHorde.Api:** librer�a de control y manejo de la API de *Stable Horde*. Implementa las interfaces
definidas en el dominio de la librer�a anterior.
* **LibFakeAi.Api:** librer�a de control de una API *fake* que utilizo para hacer pruebas de interface. Implementa
las interfaces de `LibAiImagenGeneration.Domain` pero simplemente espera un tiempo antes de descargar im�genes
predefinidas (o lanzar un error).

## Uso de LibAiImageGeneration

Todo el trabajo de la aplicaci�n realmente se basa en las llamadas a `LibAiImageGeneration`, realmente el resto del proyecto de escritorio
maneja esta librer�a y trata sus resultados.

Para poder utilizarla, primero debemos a�adir los proveedores que ser�n nuestra implementaciones de `LibAiImageGeneration.Domain`:

```csharp
ImageGenerationManager = new Bau.Libraries.LibAiImageGeneration.ImageGenerationManager();

	// A�ade los proveedores
	ImageGenerationManager.AddProvider("Horde", new Bau.Libraries.LibStableHorde.Api.StableHordeManager(new Uri("https://stablehorde.net"), "0000000000"));
	ImageGenerationManager.AddProvider("Fake", new Bau.Libraries.LibFakeAi.Api.FakeAiManager());
	// Conecta con los proveedores (descarga los modelos)
	await ImageGenerationManager.ConnectAsync(cancellationToken);
```

**Nota:** la inicializaci�n del proveedor de *Stable Horde* utiliza la API Key `0000000000` que es la clave gen�rica de uso. En este momento, la aplicaci�n
no admite configuraci�n.

Una vez definido el *manager*, podemos agregar un manejador de eventos para que, una vez lancemos una tarea, nos avise del progreso de la misma:

```csharp
ImageGenerationManager.Progress += (sender, args) => UpdateProgress(args.Generation);
```

Para lanzar una nueva generaci�n a un proveedor, debemos llamar al m�todo `` del *manager*:

```csharp
PromptRequestModel prompt = new(provider);
PromptResultModel result = await ImageGenerationManager.AiProvidersManager.PromptAsync(prompt, cancellationToken);
		 
	if (!result.CanProcess || string.IsNullOrWhiteSpace(result.Id))
		MainViewModel.HostController.SystemController.ShowMessage($"Can't execute the prompt{Environment.NewLine}{result.Message}");
	else
		TasksListViewModel.Add(promptVersionViewModel, prompt.Positive, result.Id);
```

**Nota:** esta es una versi�n simplificada sacada del proyecto de ViewModels.

`PromptRequestModel` es la clase con los datos del prompt, incluye el proveedor, los testos positivos y negativos, el modelo seleccionado, etc...

El m�todo `PromptAsync` llama al proveedor de IA adecuado para indicarle que inicie el proceso, nos devuelve un resultado que
incluye el `Id` del proceso en caso que la llamada sea correcta (el proveedor puede rechazarla porque la imagen sea demasiado grande, tenga demasiados
pasos o simplemente porque en ese momento no pueda ejecutarla).

A partir de ese momento, el *manager* se encarga de llamar cada cierto tiempo a la API y controlar el proceso. Cuando acaba, simplemente descarga las
im�genes generadas.

La llamada a `TasksListViewModel.Add(...)` que aparece en el c�digo anterior, es para mostrar al usuario el progreso en la lista de tareas de la aplicaci�n
de escritorio, realmente no es necesaria para el *manager* que simplemente lanza eventos seg�n va avanzando en el proceso.

