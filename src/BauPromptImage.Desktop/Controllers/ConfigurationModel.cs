namespace BauPromptImage.Desktop.Controllers;

/// <summary>
///		Datos de configuración
/// </summary>
public class ConfigurationModel
{
	/// <summary>
	///		Carga la configuración
	/// </summary>
	public void Load()
	{
		LastFileName = Properties.Settings.Default.LastFileName;
	}

	/// <summary>
	///		Graba la configuración
	/// </summary>
	public void Save()
	{
		// Asigna las propiedades
		Properties.Settings.Default.LastFileName = LastFileName;
		// Graba la configuración
		Properties.Settings.Default.Save();
	}

	/// <summary>
	///		Ultimo archivo con el que se ha trabajado
	/// </summary>
	public string? LastFileName { get; set; }
}
