using NGameEditor.Bridge.Files;
using NGameEditor.Bridge.InterProcessCommunication;
using NGameEditor.Bridge.Scenes;
using NGameEditor.Functionality.Files;
using NGameEditor.Functionality.Scenes;
using NGameEditor.Functionality.Shared;

namespace NGameEditor.Functionality.InterProcessCommunication;



public class FrontendApi(
	IUiThreadDispatcher uiThreadDispatcher,
	IFileBrowserUpdater fileBrowserUpdater,
	ISceneUpdater sceneUpdater
) : IFrontendApi
{
	public void UpdateFiles(DirectoryDescription rootDirectory) =>
		uiThreadDispatcher.DoOnUiThread(
			() => fileBrowserUpdater.UpdateProjectFiles(rootDirectory)
		);


	public void UpdateLoadedScene(SceneDescription sceneDescription) =>
		uiThreadDispatcher.DoOnUiThread(
			() => sceneUpdater.UpdateLoadedScene(sceneDescription)
		);
}
