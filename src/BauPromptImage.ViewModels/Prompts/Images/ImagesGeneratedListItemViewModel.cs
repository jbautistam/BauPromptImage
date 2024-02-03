
namespace BauPromptImage.ViewModels.Prompts.Images;

/// <summary>
///		Imagen de la lista de generaciones
/// </summary>
public class ImagesGeneratedListItemViewModel : Bau.Libraries.BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _fileName = default!;

	public ImagesGeneratedListItemViewModel(ImagesGeneratedListViewModel imagesGeneratedListViewModel, string fileName)
	{
		ImagesGeneratedListViewModel = imagesGeneratedListViewModel;
		FileName = fileName;
	}

	/// <summary>
	///		Borra un archivo de imagen
	/// </summary>
	public void Delete()
	{
		if (ImagesGeneratedListViewModel.PromptVersionViewModel.PromptVersionListViewModel.PromptFileViewModel.MainViewModel.HostController
					.SystemController.ShowQuestion($"Do you want todelete the file '{Path.GetFileName(FileName)}'?", "Accept", "Cancel"))
		{
			// Borra el archivo
			Bau.Libraries.LibHelper.Files.HelperFiles.KillFile(FileName);
			// Elimina la imagen de la lista
			ImagesGeneratedListViewModel.Items.Remove(this);
		}
	}

	/// <summary>
	///		Lista de imágenes a la que se asocia
	/// </summary>
	public ImagesGeneratedListViewModel ImagesGeneratedListViewModel { get; }

	/// <summary>
	///		Versión
	/// </summary>
	public string FileName
	{ 
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}
}
