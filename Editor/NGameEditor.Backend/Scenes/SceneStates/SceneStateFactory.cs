using Microsoft.Extensions.Logging;
using NGame.Assets;
using NGame.SceneAssets;
using NGameEditor.Bridge.InterProcessCommunication;

namespace NGameEditor.Backend.Scenes.SceneStates;



public interface ISceneStateFactory
{
	ISceneState Create();
}



class SceneStateFactory(
	ILastOpenedSceneLoader lastOpenedSceneLoader,
	IStartSceneLoader startSceneLoader,
	ILogger<SceneStateFactory> logger,
	ISceneDescriptionMapper sceneDescriptionMapper,
	IFrontendApi frontendApi
)
	: ISceneStateFactory
{
	private readonly ILogger _logger = logger;


	public ISceneState Create()
	{
		var backendScene = GetBackendScene();
		var sceneState = new SceneState(backendScene);

		sceneState.LoadedSceneChanged += args =>
		{
			var sceneDescription = sceneDescriptionMapper.Map(args.NewBackendScene);
			frontendApi.UpdateLoadedScene(sceneDescription);
		};
		
		sceneState.SetLoadedScene(backendScene);

		return sceneState;
	}


	private BackendScene GetBackendScene()
	{
		var lastOpenedSceneResult = lastOpenedSceneLoader.GetLastOpenedScene();
		if (lastOpenedSceneResult.TryGetValue(out var lastOpenedScene))
		{
			return lastOpenedScene;
		}

		_logger.LogInformation("{Error}", lastOpenedSceneResult.ErrorValue!.Title);

		var startSceneResult = startSceneLoader.GetStartScene();

		if (startSceneResult.TryGetValue(out var startScene))
		{
			return startScene;
		}

		var newScene =
			new BackendScene(
				null,
				new SceneAsset
				{
					Id = AssetId.Create(Guid.NewGuid())
				}
			);

		return newScene;
	}
}
