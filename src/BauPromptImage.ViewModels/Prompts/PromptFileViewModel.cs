using Bau.Libraries.BauMvvm.ViewModels;
using BauPromptImage.Application.Models.Prompts;

namespace BauPromptImage.ViewModels.Prompts;

/// <summary>
///		ViewModel del archivo del prompt
/// </summary>
public class PromptFileViewModel : BaseObservableObject
{
	// Constantes privadas
	private const string DefaultExtension = "prompt.def";
	private const string DefaultFileName = $"NewFile.{DefaultExtension}";
	private const string Mask = $"Prompt image files (*.{DefaultExtension})|*.{DefaultExtension}|All files (*.*)|*.*";
	// Variables privadas
	private string _name = default!, _description = default!;
	private PromptVersionListViewModel _versionsViewModel = default!;

	public PromptFileViewModel(MainViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
		VersionsViewModel = new PromptVersionListViewModel(this);
	}

	/// <summary>
	///		Crea un nuevo archivo
	/// </summary>
	internal void NewFile()
	{
		if (CanOpenFile())
		{
			string? fileName = GetNewFileName();

				if (!string.IsNullOrWhiteSpace(fileName))
					Load(fileName);
		}
	}

	/// <summary>
	///		Carga un archivo
	/// </summary>
	internal void LoadFile()
	{
		if (CanOpenFile())
		{
			string? fileName = MainViewModel.HostController.DialogsController.OpenDialogLoad(null, Mask, DefaultFileName, DefaultExtension);

				if (!string.IsNullOrWhiteSpace(fileName))
					Load(fileName);
		}
	}

	/// <summary>
	///		Carga un archivo
	/// </summary>
	internal void Load(string fileName)
	{
		// Carga el archivo
		PromptFile = MainViewModel.PromptGenerator.LoadFile(fileName);
		if (PromptFile.Prompts.Count == 0)
			PromptFile.Prompts.Add(new PromptModel(string.Empty)
											{
												Version = 1
											}
								  );
		// Asigna las propiedades
		Name = PromptFile.Name;
		Description = PromptFile.Description;
		// Carga los prompts
		VersionsViewModel.Clear();
		foreach (PromptModel prompt in PromptFile.Prompts)
			VersionsViewModel.Add(prompt);
		VersionsViewModel.SelectLast();
	}

	/// <summary>
	///		Carga las imágenes
	/// </summary>
	internal void LoadImages()
	{
		VersionsViewModel.LoadImages();
	}

	/// <summary>
	///		Comprueba si se puede abrir un archivo
	/// </summary>
	private bool CanOpenFile()
	{
		return true;
	}

	/// <summary>
	///		Guarda un archivo
	/// </summary>
	internal void SaveFile(bool saveAs)
	{
		string? fileName;

			// Si hay que cambiar el nombre de archivo
			if (PromptFile is null || string.IsNullOrWhiteSpace(PromptFile.FileName) || saveAs)
				fileName = GetNewFileName();
			else
				fileName = PromptFile.FileName;
			// Graba el archivo
			if (!string.IsNullOrWhiteSpace(fileName))
				Save(fileName);
	}

	/// <summary>
	///		Graba un archivo
	/// </summary>
	private void Save(string fileName)
	{
		// Vuelca los datos
		PromptFile = new PromptFileModel(fileName);
		PromptFile.Name = Name;
		PromptFile.Description = Description;
		// Crea el item
		foreach (PromptVersionViewModel prompt in VersionsViewModel.Items)
			PromptFile.Prompts.Add(prompt.GetPrompt());
		// Graba el archivo
		MainViewModel.PromptGenerator.SaveFile(fileName, PromptFile);
	}

	/// <summary>
	///		Obtiene un nuevo nombre de archivo
	/// </summary>
	private string? GetNewFileName() => MainViewModel.HostController.DialogsController.OpenDialogSave(null, Mask, DefaultFileName, DefaultExtension);

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public MainViewModel MainViewModel { get; }

	/// <summary>
	///		Archivo de prompt cargado
	/// </summary>
	public PromptFileModel PromptFile { get; private set; } = default!;

	/// <summary>
	///		Nombre del prompt
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descripción del prompt
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}
	
	/// <summary>
	///		Lista de versiones
	/// </summary>
	public PromptVersionListViewModel VersionsViewModel
	{
		get { return _versionsViewModel; }
		set { CheckObject(ref _versionsViewModel, value); }
	}
}