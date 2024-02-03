using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BauPromptImage.Desktop.Views;

/// <summary>
///		Ventana con una imagen
/// </summary>
public partial class ImageView : Window
{
	public ImageView(MainWindow mainWindow, string fileName)
	{
		InitializeComponent();
		MainWindow = mainWindow;
		FileName = fileName;
	}

	/// <summary>
	///		Inicializa la vista
	/// </summary>
	private void InitView()
	{
		if (!string.IsNullOrWhiteSpace(FileName) && System.IO.File.Exists(FileName))
			try
			{
				ImageSource imageSource = new BitmapImage(new Uri(FileName));

					// Carga la imagen
					imgViewer.Source = imageSource;
			}
			catch (Exception exception)
			{
				MainWindow.MainController.HostController.SystemController.ShowMessage($"Error when load image: {exception.Message}");
			}
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		InitView();
	}

	/// <summary>
	///		Ventana principal
	/// </summary>
	private MainWindow MainWindow { get; }

	/// <summary>
	///		Nombre del archivo
	/// </summary>
	private string FileName { get; }
}
