using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;
using Bau.Libraries.LibAiImageGeneration.Models;
using BauPromptImage.Application.Models.Prompts;

namespace BauPromptImage.ViewModels.Prompts.Generations;

/// <summary>
///		ViewModel de la cola de tareas de procesamiento
/// </summary>
public class TasksQueueViewModel : BaseObservableObject
{
	// Variables privadas
	private TasksListViewModel _tasksListViewModel = default!;

	public TasksQueueViewModel(MainViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
		TasksListViewModel = new TasksListViewModel(this);
		ImageGenerationManager = new Bau.Libraries.LibAiImageGeneration.ImageGenerationManager();
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public async Task InitializeAsync(CancellationToken cancellationToken)
	{
		// Inicializa el controlador
		if (!ImageGenerationManager.AiProvidersManager.Providers.ContainsKey("Horde"))
			ImageGenerationManager.AddProvider("Horde", new Bau.Libraries.LibStableHorde.Api.StableHordeManager(new Uri("https://stablehorde.net"), "0000000000"));
		if (!ImageGenerationManager.AiProvidersManager.Providers.ContainsKey("Fake"))
			ImageGenerationManager.AddProvider("Fake", new Bau.Libraries.LibFakeAi.Api.FakeAiManager());
		await ImageGenerationManager.ConnectAsync(cancellationToken);
		// Añade el manegador de eventos
		ImageGenerationManager.Progress += (sender, args) => UpdateProgress(args.Generation);
	}

	/// <summary>
	///		Añade una tarea de generación
	/// </summary>
	internal async Task AddTaskAsync(PromptVersionViewModel promptVersionViewModel, CancellationToken cancellationToken)
	{
		PromptModel prompt = promptVersionViewModel.GetPrompt();
		Bau.Libraries.LibAiImageGeneration.Domain.Models.Results.PromptResultModel result = await ImageGenerationManager.AiProvidersManager.PromptAsync(ConvertToAiPrompt(prompt), cancellationToken);
		 
			if (!result.CanProcess || string.IsNullOrWhiteSpace(result.Id))
				MainViewModel.HostController.SystemController.ShowMessage($"Can't execute the prompt{Environment.NewLine}{result.Message}");
			else
				TasksListViewModel.Add(promptVersionViewModel, prompt.Positive, result.Id);
	}

	/// <summary>
	///		Actualiza el progreso
	/// </summary>
	private void UpdateProgress(GenerationModel generation)
	{
		foreach (TasksListItemViewModel item in TasksListViewModel.Items)
			if (item.TaskId.Equals(generation.Id, StringComparison.CurrentCultureIgnoreCase))
				item.UpdateStatus(generation);
	}

	/// <summary>
	///		Convierte el modelo de prompts
	/// </summary>
	private PromptRequestModel ConvertToAiPrompt(PromptModel prompt)
	{
		PromptRequestModel request = new(prompt.Provider, Path.GetDirectoryName(MainViewModel.PromptFileViewModel.PromptFile.FileName) ?? string.Empty,
										 MainViewModel.PromptFileViewModel.VersionsViewModel.SelectedItem?.GetFileImagePrefix() ?? string.Empty);

			// Asigna las propiedades
			request.Prompt = prompt.Positive;
			request.NegativePrompt = prompt.Negative;
			request.Nsfw = prompt.Nsfw;
			request.Models.Add(prompt.Model);
			request.Sampler = ConvertSampler(prompt.Sampler);
			request.CfgScale = prompt.CfgScale;
			request.DenoisingStrength = prompt.DenoisingStrength;
			request.Seed = prompt.Seed;
			request.Height = prompt.Height;
			request.Width = prompt.Width;
			request.PostProcessing.AddRange(ConvertPostprocessing(prompt.PostProcessing));
			request.Steps = prompt.Steps;
			request.ImagesToGenerate = prompt.ImagesToGenerate;
			// Devuelve la solicitud
			return request;
	}

	/// <summary>
	///		Convierte el modelo de sampleado
	/// </summary>
	private PromptRequestModel.SamplerType ConvertSampler(PromptModel.SamplerType sampler)
	{
		return sampler switch
					{
						PromptModel.SamplerType.k_dpm_fast => PromptRequestModel.SamplerType.k_dpm_fast,
						PromptModel.SamplerType.k_heun => PromptRequestModel.SamplerType.k_heun,
						PromptModel.SamplerType.k_dpmpp_sde => PromptRequestModel.SamplerType.k_dpmpp_sde,
						PromptModel.SamplerType.dpmsolver => PromptRequestModel.SamplerType.dpmsolver,
						PromptModel.SamplerType.k_dpm_2_a => PromptRequestModel.SamplerType.dpmsolver,
						PromptModel.SamplerType.k_dpm_adaptive => PromptRequestModel.SamplerType.k_dpm_adaptive,
						PromptModel.SamplerType.k_dpmpp_2s_a => PromptRequestModel.SamplerType.k_dpmpp_2s_a,
						PromptModel.SamplerType.k_lms => PromptRequestModel.SamplerType.k_lms,
						PromptModel.SamplerType.k_euler_a => PromptRequestModel.SamplerType.k_euler_a,
						PromptModel.SamplerType.lcm => PromptRequestModel.SamplerType.lcm,
						PromptModel.SamplerType.DDIM => PromptRequestModel.SamplerType.DDIM,
						PromptModel.SamplerType.k_dpmpp_2m => PromptRequestModel.SamplerType.k_dpmpp_2m,
						PromptModel.SamplerType.k_dpm_2 => PromptRequestModel.SamplerType.k_dpm_2,
						_ => PromptRequestModel.SamplerType.k_euler,
					};
	}

	/// <summary>
	///		Convierte la lista de tipos de postproceso
	/// </summary>
	private List<PromptRequestModel.PostProcessType> ConvertPostprocessing(List<PromptModel.PostProcessType> postProcessing)
	{
		List<PromptRequestModel.PostProcessType> result = new();

			// Añade los datos convertidos
			foreach (PromptModel.PostProcessType postProcess  in postProcessing)
				result.Add(ConvertPostprocess(postProcess));
			// Devuelve la lista
			return result;

			// Convierte el tipo de postproceso
			PromptRequestModel.PostProcessType ConvertPostprocess(PromptModel.PostProcessType type)
			{
				return type switch
						{
							PromptModel.PostProcessType.RealESRGAN_x4plus => PromptRequestModel.PostProcessType.RealESRGAN_x4plus,
							PromptModel.PostProcessType.RealESRGAN_x2plus => PromptRequestModel.PostProcessType.RealESRGAN_x2plus,
							PromptModel.PostProcessType.RealESRGAN_x4plus_anime_6B => PromptRequestModel.PostProcessType.RealESRGAN_x4plus_anime_6B,
							PromptModel.PostProcessType.NMKD_Siax => PromptRequestModel.PostProcessType.NMKD_Siax,
							PromptModel.PostProcessType.x4_AnimeSharp => PromptRequestModel.PostProcessType.x4_AnimeSharp,
							PromptModel.PostProcessType.CodeFormers => PromptRequestModel.PostProcessType.CodeFormers,
							PromptModel.PostProcessType.strip_background => PromptRequestModel.PostProcessType.strip_background,
							_ => PromptRequestModel.PostProcessType.GFPGAN,
						};
		}
	}

	/// <summary>
	///		Manager de generación
	/// </summary>
	public Bau.Libraries.LibAiImageGeneration.ImageGenerationManager ImageGenerationManager { get; }

	/// <summary>
	///		ViewModel de la ventana principal
	/// </summary>
	public MainViewModel MainViewModel { get; }

	/// <summary>
	///		Lista de generación de tareas
	/// </summary>
	public Generations.TasksListViewModel TasksListViewModel
	{
		get { return _tasksListViewModel; }
		set { CheckObject(ref _tasksListViewModel, value); }
	}
}
