using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using BauPromptImage.Application.Models.Categories;
using Bau.Libraries.BauMvvm.ViewModels;

namespace BauPromptImage.ViewModels.Explorers;

/// <summary>
///		Arbol de categorías
/// </summary>
public class TreeCategoriesViewModel : TreeViewModel
{
	// Enumerados públicos
	public enum GenerationMode
	{
		OnlyThis,
		CopyParents,
		CopyDescendants
	}
	// Variables privadas
	private bool _canCopyParents, _canCopyDescendants;

	public TreeCategoriesViewModel(MainViewModel mainViewModel)
	{
		// Inicializa los viewModel
		MainViewModel = mainViewModel;
		CanCopyParents = false;
		CanCopyDescendants = false;
		// Inicializa los comandos
		CopyThisCommand = new BaseCommand(_ => GetTextForEditor(GenerationMode.OnlyThis));
		CopyParentsCommand = new BaseCommand(_ => GetTextForEditor(GenerationMode.CopyParents), _ => CanCopyParents)
										.AddListener(this, nameof(SelectedNode));
		CopyDescendantsCommand = new BaseCommand(_ => GetTextForEditor(GenerationMode.CopyDescendants), _ => CanCopyDescendants)
										.AddListener(this, nameof(SelectedNode));
		// Inicializa los manejadores de eventos
		PropertyChanged += (sender, args) => {
												if (!string.IsNullOrWhiteSpace(args.PropertyName))
												{
													if (args.PropertyName.Equals(nameof(SelectedNode), StringComparison.CurrentCultureIgnoreCase))
														UpdateCanCopyNodes();
												}
											 };
	}

	/// <summary>
	///		Añade los nodos raíz
	/// </summary>
	protected override void AddRootNodes()
	{
		// Limpia el árbol
		Children.Clear();
		// Añade los nodos principales
		foreach (CategoryModel category in MainViewModel.PromptGenerator.Categories)
		{
			CategoryNodeViewModel node = CreateNode(category);

				// Añade el nodo
				Children.Add(node);
				// Expande el nodo para que lo cargue
				node.Expand();
		}
	}

	/// <summary>
	///		Genera el prompt
	/// </summary>
	private string GeneratePrompt(ControlHierarchicalViewModel node)
	{
		string result = string.Empty;

			// Recorre los elementos de la categoría generando el resultado
			if (node is CategoryNodeViewModel categoryNode)
			{
				// Añade el texto del nodo
				if (node.IsChecked)
					result = result.AddWithSeparator(node.Text, ",");
				// Añade el texto de los hijos
				foreach (ControlHierarchicalViewModel child in categoryNode.Children)
					result = result.AddWithSeparator(GeneratePrompt(child), ",");
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Actualiza el árbol
	/// </summary>
	public void Refresh()
	{
		AddRootNodes();
	}

	/// <summary>
	///		Modifica los valores que indica si se pueden copiar los descendentes / ascendentes
	/// </summary>
	private void UpdateCanCopyNodes()
	{
		if (SelectedNode is CategoryNodeViewModel node)
		{
			CanCopyDescendants = node.Children.Count > 0;
			CanCopyParents = node.Parent is not null;
		}
	}

	/// <summary>
	///		Obtiene el texto para el editor
	/// </summary>
	private void GetTextForEditor(GenerationMode mode)
	{
		if (SelectedNode is CategoryNodeViewModel node)
			node.GetTextForEditor(mode);
	}

	/// <summary>
	///		Crea un nodo
	/// </summary>
	private CategoryNodeViewModel CreateNode(CategoryModel category) => new CategoryNodeViewModel(this, null, category);

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public MainViewModel MainViewModel { get; }

	/// <summary>
	///		Indica si se pueden copiar los ascendentes
	/// </summary>
	public bool CanCopyParents 
	{ 
		get { return _canCopyParents; }
		set { CheckProperty(ref _canCopyParents, value); }
	}

	/// <summary>
	///		Indica si se pueden copiar los descendentes
	/// </summary>
	public bool CanCopyDescendants
	{ 
		get { return _canCopyDescendants; }
		set { CheckProperty(ref _canCopyDescendants, value); }
	}

	/// <summary>
	///		Comando para copiar los datos de un nodo
	/// </summary>
	public BaseCommand CopyThisCommand { get; }

	/// <summary>
	///		Comando para copiar el texto de este elemento y sus ascendentes
	/// </summary>
	public BaseCommand CopyParentsCommand { get; }

	/// <summary>
	///		Comando para copiar el texto de este elemento y sus descendentes
	/// </summary>
	public BaseCommand CopyDescendantsCommand { get; }
}
