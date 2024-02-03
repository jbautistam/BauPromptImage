using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace BauPromptImage.ViewModels;

/// <summary>
///		ViewModel de la ventana principal
/// </summary>
public class MainViewModel : BaseObservableObject
{
	// Eventos públicos
	public event EventHandler<string>? CopiedText;

	public MainViewModel(IHostController hostController)
	{
		// Inicializa los objetos
		HostController = hostController;
		PromptGenerator = new Application.PromptGenerator();
		// Inicializa los viewModels
		TreeCategoriesViewModel = new Explorers.TreeCategoriesViewModel(this);
		PromptFileViewModel = new Prompts.PromptFileViewModel(this);
		TasksQueueViewModel = new Prompts.Generations.TasksQueueViewModel(this);
		// Asigna los comandos
		NewFileCommand = new BaseCommand(_ => PromptFileViewModel.NewFile());
		LoadCommand = new BaseCommand(_ => PromptFileViewModel.LoadFile());
		SaveCommand = new BaseCommand(_ => PromptFileViewModel.SaveFile(false));
		SaveAsCommand = new BaseCommand(_ => PromptFileViewModel.SaveFile(true));
		NewVersionCommand = new BaseCommand(_ => PromptFileViewModel.VersionsViewModel.Add());
		DeleteVersionCommand = new BaseCommand(_ => PromptFileViewModel.VersionsViewModel.Delete());
		CompileCommand = new BaseCommand(async _ => await PromptFileViewModel.VersionsViewModel.CompileAsync(CancellationToken.None));
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public async Task InitializeAsync(string categoriesFileName, CancellationToken cancellationToken)
	{
		// Inicializa la API de generación de imágenes
		await TasksQueueViewModel.InitializeAsync(cancellationToken);
		// Carga los modelos del generador
		PromptGenerator.Load(categoriesFileName);
		// Carga las categorías en el árbol
		TreeCategoriesViewModel.Refresh();
	}

	/// <summary>
	///		Carga un archivo
	/// </summary>
	public void Load(string fileName)
	{
		PromptFileViewModel.Load(fileName);
	}

	/// <summary>
	///		Graba el prompt
	/// </summary>
	public void Save(bool saveAs)
	{
		PromptFileViewModel.SaveFile(saveAs);
	}

	/// <summary>
	///		Lanza el evento de texto seleccionado en las categorías
	/// </summary>
	internal void RaiseCategoryTextEvent(string text)
	{
		CopiedText?.Invoke(this, text);
	}

	/// <summary>
	///		Controlador principal
	/// </summary>
	public IHostController HostController { get; }

	/// <summary>
	///		Generador de prompts
	/// </summary>
	public Application.PromptGenerator PromptGenerator { get; }

	/// <summary>
	///		ViewModel del árbol de categorías
	/// </summary>
	public Explorers.TreeCategoriesViewModel TreeCategoriesViewModel { get; }

	/// <summary>
	///		ViewModel del archivo
	/// </summary>
	public Prompts.PromptFileViewModel PromptFileViewModel { get; }

	/// <summary>
	///		ViewModel de la cola de tareas
	/// </summary>
	public Prompts.Generations.TasksQueueViewModel TasksQueueViewModel { get; }

	/// <summary>
	///		Comando de nuevo archivo
	/// </summary>
	public BaseCommand NewFileCommand { get; }

	/// <summary>
	///		Comando de carga
	/// </summary>
	public BaseCommand LoadCommand { get; }

	/// <summary>
	///		Comando de grabación
	/// </summary>
	public BaseCommand SaveCommand { get; }

	/// <summary>
	///		Comando de guardar como
	/// </summary>
	public BaseCommand SaveAsCommand { get; }	

	/// <summary>
	///		Comando para crear una nueva versión
	/// </summary>
	public BaseCommand NewVersionCommand { get; }

	/// <summary>
	///		Comando para borrar una versión
	/// </summary>
	public BaseCommand DeleteVersionCommand { get; }

	/// <summary>
	///		Comando de compilación
	/// </summary>
	public BaseCommand CompileCommand { get; }
}