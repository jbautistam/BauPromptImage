using Bau.Libraries.LibAiImageGeneration.Models;

namespace BauPromptImage.ViewModels.Prompts.Generations;

/// <summary>
///		Tarea de listViewModel
/// </summary>
public class TasksListItemViewModel : Bau.Libraries.BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _fileName = default!, _shortFileName = default!, _version = default!, _positive = default!;
	private string _taskId = default!, _plannedEndTime = default!;
	private string _status = default!;
	private int _progressMinimum, _progressMaximum, _progressValue;
	private Bau.Libraries.BauMvvm.ViewModels.Media.MvvmColor _background = Bau.Libraries.BauMvvm.ViewModels.Media.MvvmColor.White;

	public TasksListItemViewModel(TasksListViewModel tasksListViewModel, PromptVersionViewModel promptVersionViewModel, string positive, string taskId)
	{
		TasksListViewModel = tasksListViewModel;
		PromptVersionViewModel = promptVersionViewModel;
		FileName = promptVersionViewModel.PromptVersionListViewModel.PromptFileViewModel.PromptFile.FileName;
		Version = promptVersionViewModel.VersionText;
		Positive = positive;
		TaskId = taskId;
		ProgressMinimum = 1;
		ProgressMaximum = 4;
		ProgressValue = 1;
	}

	/// <summary>
	///		Actualiza el estado
	/// </summary>
	internal void UpdateStatus(GenerationModel generation)
	{
		switch (generation.Status)
		{
			case GenerationModel.GenerationStatus.Error:
					Status = $"Error. {generation.Message}";
					Background = Bau.Libraries.BauMvvm.ViewModels.Media.MvvmColor.Yellow;
					ProgressValue = 4;
				break;
			case GenerationModel.GenerationStatus.Success:
					Status = "Success";
					PlannedEndTime = GetLocalDate(DateTime.UtcNow);
					TasksListViewModel.TasksQueueViewModel.MainViewModel.PromptFileViewModel.LoadImages();
					ProgressValue = 4;
					Background = Bau.Libraries.BauMvvm.ViewModels.Media.MvvmColor.GreenYellow;
				break;
			case GenerationModel.GenerationStatus.Processing:
					Status = $"Processing. Position: {generation.QueuePosition.ToString()}. Finished: {generation.Finished.ToString()}. Processing: {generation.Processing.ToString()}";
					PlannedEndTime = GetLocalDate(generation.NextSearch);
					ProgressValue = 2;
				break;
			case GenerationModel.GenerationStatus.Generated:
					Status = "Generated";
					ProgressValue = 3;
				break;
		}
	}

	/// <summary>
	///		Obtiene la cadena de la fecha local
	/// </summary>
	private string GetLocalDate(DateTime date) => $"{TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local):HH:mm:ss}";

	/// <summary>
	///		Lista de tareas a la que se asocia
	/// </summary>
	public TasksListViewModel TasksListViewModel { get; }

	/// <summary>
	///		ViewModel de la versión
	/// </summary>
	public PromptVersionViewModel PromptVersionViewModel { get; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
			{
				if (!string.IsNullOrWhiteSpace(value))
					ShortFileName = Path.GetFileName(value);
				else
					ShortFileName = string.Empty;
			}
		}
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string ShortFileName
	{
		get { return _shortFileName; }
		set { CheckProperty(ref _shortFileName, value); }
	}

	/// <summary>
	///		Versión
	/// </summary>
	public string Version
	{
		get { return _version;}
		set { CheckProperty(ref _version, value); }
	}

	/// <summary>
	///		Prompt positivo
	/// </summary>
	public string Positive
	{
		get { return _positive; }
		set { CheckProperty(ref _positive, value); }
	}

	/// <summary>
	///		Id de tarea
	/// </summary>
	public string TaskId
	{ 
		get { return _taskId; }
		set { CheckProperty(ref _taskId, value); }
	}

	/// <summary>
	///		Estado
	/// </summary>
	public string Status
	{
		get { return _status; }
		set { CheckProperty(ref _status, value); }
	}

	/// <summary>
	///		Valor mínimo de la barra de progreso
	/// </summary>
	public int ProgressMinimum
	{
		get { return _progressMinimum; }
		set { CheckProperty(ref _progressMinimum, value); }
	}

	/// <summary>
	///		Valor máximo de la barra de progreso
	/// </summary>
	public int ProgressMaximum
	{
		get { return _progressMaximum; }
		set { CheckProperty(ref _progressMaximum, value); }
	}	

	/// <summary>
	///		Valor de la barra de progreso
	/// </summary>
	public int ProgressValue
	{
		get { return _progressValue; }
		set { CheckProperty(ref _progressValue, value); }
	}

	/// <summary>
	///		Hora de fin planificada
	/// </summary>
	public string PlannedEndTime
	{
		get { return _plannedEndTime; }
		set { CheckProperty(ref _plannedEndTime, value); }
	}

	/// <summary>
	///		Color de fondo
	/// </summary>
	public Bau.Libraries.BauMvvm.ViewModels.Media.MvvmColor Background
	{
		get { return _background; }
		set { CheckObject(ref _background, value); }
	}
}
