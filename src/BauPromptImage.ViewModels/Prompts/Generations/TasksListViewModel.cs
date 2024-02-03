using System.Collections.ObjectModel;
using Bau.Libraries.BauMvvm.ViewModels;

namespace BauPromptImage.ViewModels.Prompts.Generations;

/// <summary>
///		Lista de tareas a generar
/// </summary>
public class TasksListViewModel : BaseObservableObject
{
	// Variables privadas
	private TasksListItemViewModel? _selectedItem;

	public TasksListViewModel(TasksQueueViewModel tasksQueueViewModel)
	{
		TasksQueueViewModel = tasksQueueViewModel;
	}

	/// <summary>
	///		Añade una tarea
	/// </summary>
	internal void Add(PromptVersionViewModel promptVersionViewModel, string positive, string taskId)
	{
		Items.Add(new TasksListItemViewModel(this, promptVersionViewModel, positive, taskId));
	}

	/// <summary>
	///		Limpia los elementos
	/// </summary>
	internal void Clear()
	{
		Items.Clear();
		SelectedItem = null;
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	internal void Delete()
	{
		if (SelectedItem is not null)
		{
			// Elimina el elemento seleccionado
			Items.Remove(SelectedItem);
			// Añade un elemento si no hay nada
			//if (Items.Count == 0)
			//	Add(string.Empty, string.Empty);
			//// Selecciona el último elemento
			//SelectLast();
		}
	}

	/// <summary>
	///		ViewModel de la cola de tareas
	/// </summary>
	public TasksQueueViewModel TasksQueueViewModel { get; }

	/// <summary>
	///		Elementos de la lista
	/// </summary>
	public ObservableCollection<TasksListItemViewModel> Items { get; } = new();

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public TasksListItemViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}
}
