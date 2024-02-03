using Bau.Libraries.BauMvvm.Views.Wpf.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace BauPromptImage.Desktop.Controllers;

/// <summary>
///		Controlador principal
/// </summary>
public class MainWindowController
{
	public MainWindowController(string appName, MainWindow mainWindow)
	{
		HostController = new HostController(appName, mainWindow);
		HostHelperController = new HostHelperController(mainWindow);
		MainViewModel = new ViewModels.MainViewModel(HostController);
		Configuration = new ConfigurationModel();
	}

	/// <summary>
	///		Controlador principal
	/// </summary>
	public IHostController HostController { get; }

	/// <summary>
	///		Controlador de Windows
	/// </summary>
	public HostHelperController HostHelperController { get; }

	/// <summary>
	///		Configuración
	/// </summary>
	public ConfigurationModel Configuration { get; }

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ViewModels.MainViewModel MainViewModel { get; }
}
