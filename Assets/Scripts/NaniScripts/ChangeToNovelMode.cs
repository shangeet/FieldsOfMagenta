using UnityEngine;
using Naninovel;

[CommandAlias("novelMode")]
public class ChangeToNovelMode : Command {

    public StringParameter ScriptName;
    public StringParameter Label;
    public StringParameter NovelEventName;

    public override async UniTask ExecuteAsync (AsyncToken asyncToken = default) {

        Debug.Log("Changing to novel mode");

        // 1. Disable Input
        GameObject uiHandlerGo = GameObject.Find("UIHandler");
        if (uiHandlerGo != null) {
            UIHandler uiHandler = uiHandlerGo.GetComponent<UIHandler>();
            uiHandler.DisableInput();
        }

        // 2. Switch cameras and adjust camera settings
        var advCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        advCamera.enabled = false;
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        var naniUICamera = Engine.GetService<ICameraManager>().UICamera;
        // naniUICamera.clearFlags = advCamera.clearFlags;
        // naniUICamera.cullingMask = advCamera.cullingMask;
        // naniUICamera.orthographicSize = advCamera.orthographicSize;
        naniCamera.enabled = true;
        naniUICamera.enabled = true;


        // 3. Load and play specified script (if assigned).
        if (Assigned(ScriptName)) {
            Debug.Log("Playing Naninovel script");
            var scriptPlayer = Engine.GetService<IScriptPlayer>();
            // var eventCompleteMarkerCommand = new EventCompleteMarker { NovelEventName = NovelEventName };
            // System.Func<Command, UniTask> eventCompleteMarker = eventCompleteMarkerCommand => eventCompleteMarkerCommand.ExecuteAsync();
            // scriptPlayer.AddPostExecutionTask(eventCompleteMarker);
            await scriptPlayer.PreloadAndPlayAsync(ScriptName, label: Label);
        }

        // 4. Enable Naninovel input.
        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;
        // GameObject blockUI = GameObject.Find("Naninovel<Runtime>/ContinueInputUI");
        // blockUI.SetActive(true);
    }

}