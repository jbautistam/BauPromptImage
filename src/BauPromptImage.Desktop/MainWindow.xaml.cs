using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using BauPromptImage.ViewModels.Explorers;

namespace BauPromptImage.Desktop;

/// <summary>
///		Ventana principal
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa la ventana
	/// </summary>
	private async Task InitWindowAsync(CancellationToken cancellationToken)
	{
		// Inicializa los controladores
		MainController = new Controllers.MainWindowController("BauPromptImage.Desktop", this);
		DataContext = ViewModel = MainController.MainViewModel;
		// Carga la configuración
		MainController.Configuration.Load();
		// Inicializa los manejadores de eventos
		ViewModel.CopiedText += (sender, args) => CopyCategoryText(args);
		// Inicializa el viewModel
		await ViewModel.InitializeAsync(GetFileNameCategories(), cancellationToken);
		// Carga el último archivo
		if (!string.IsNullOrWhiteSpace(MainController.Configuration.LastFileName))
			ViewModel.Load(MainController.Configuration.LastFileName);
	}

	/// <summary>
	///		Obtiene el nombre de archivo de categorías
	/// </summary>
	private string GetFileNameCategories()
	{
		string folder = System.IO.Path.GetDirectoryName(Environment.ProcessPath) ?? string.Empty;

			return System.IO.Path.Combine(folder, "data/Prompts.xml");
	}

	/// <summary>
	///		Abre la imagen
	/// </summary>
	private void OpenImage()
	{
		if (ViewModel.PromptFileViewModel.VersionsViewModel.SelectedItem is not null)
		{
			string? fileName = ViewModel.PromptFileViewModel.VersionsViewModel.SelectedItem.ImagesViewModel.SelectedItem?.FileName;

				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(fileName))
				{
					Views.ImageView imageView = new(this, fileName);

						// Muestra la ventana
						imageView.Owner = this;
						imageView.Show();
				}
		}
	}

	/// <summary>
	///		Borra una image
	/// </summary>
	private void DeleteImage()
	{
		if (ViewModel.PromptFileViewModel.VersionsViewModel.SelectedItem is not null)
			ViewModel.PromptFileViewModel.VersionsViewModel.SelectedItem.ImagesViewModel.SelectedItem?.Delete();
	}

	/// <summary>
	///		Copia el texto de una categoría
	/// </summary>
	private void CopyCategoryText(string text)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			// Añade un separador
			if (!string.IsNullOrWhiteSpace(txtResultPositive.Text) && !string.IsNullOrWhiteSpace(text))
				txtResultPositive.AppendText(", ");
			// Añade el texto
			txtResultPositive.AppendText(text);
		}
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	private void Save(bool saveAs)
	{
		ViewModel.Save(saveAs);
	}

	/// <summary>
	///		Comprueba si hay algún elemento sin guardar
	/// </summary>
	private bool CanExitApp()
	{
		return true;
	}

	/// <summary>
	///		Sale de la aplicación
	/// </summary>
	private void ExitApp()
	{
		// Graba la configuración
		if (!string.IsNullOrWhiteSpace(ViewModel.PromptFileViewModel.PromptFile.FileName))
		{
			MainController.Configuration.LastFileName = ViewModel.PromptFileViewModel.PromptFile.FileName;
			MainController.Configuration.Save();
		}
		// Cierra la aplicación (puede que dé una excepción al cerrar porque ya se está cerrando en el evento Closing)
		try
		{
			Close();
		}
		catch {}
	}

	private void trvCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{ 
		if (trvCategories.DataContext is TreeCategoriesViewModel viewModel)
			viewModel.SelectedNode = (sender as TreeView)?.SelectedItem as CategoryNodeViewModel;
	}

	private void trvCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{ 
		if (trvCategories.DataContext is TreeCategoriesViewModel viewModel && (sender as TreeView)?.SelectedItem is CategoryNodeViewModel node)
		{
			viewModel.SelectedNode = node;
			viewModel.CopyThisCommand.Execute(null);
		}
	}

	private void trvCategories_MouseDown(object sender, MouseButtonEventArgs e)
	{ 
		if (trvCategories.DataContext is TreeCategoriesViewModel viewModel && e.ChangedButton == MouseButton.Left)
			viewModel.SelectedNode = null;
	}

	/// <summary>
	///		Controlador principal
	/// </summary>
	public Controllers.MainWindowController MainController { get; private set; } = default!;

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ViewModels.MainViewModel ViewModel { get; private set; } = default!;

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		await InitWindowAsync(CancellationToken.None);
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		e.Cancel = !CanExitApp();
		if (!e.Cancel)
			ExitApp();
	}

	private void lstThumbs_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (lstThumbs.SelectedItem != null)
			lstThumbs.ScrollIntoView(lstThumbs.SelectedItem);
	}

	private void cmdSave_Click(object sender, RoutedEventArgs e)
	{
		Save(false);
	}

	private void cmdSaveAs_Click(object sender, RoutedEventArgs e)
	{
		Save(true);
	}

	private void cmdCopyPositive_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(txtResultPositive.Text);
	}

	private void cmdCopyNegative_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(txtResultNegative.Text);
	}

	private void cmdDeletePositive_Click(object sender, RoutedEventArgs e)
	{
		txtResultPositive.Text = string.Empty;
	}

	private void cmdDeleteNegative_Click(object sender, RoutedEventArgs e)
	{
		txtResultNegative.Text = string.Empty;
	}

	private void lstThumbs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		OpenImage();
	}

	private void mnuImages_Open_Click(object sender, RoutedEventArgs e)
	{
		OpenImage();
	}

	private void mnuImages_Delete_Click(object sender, RoutedEventArgs e)
	{
		DeleteImage();
	}
}