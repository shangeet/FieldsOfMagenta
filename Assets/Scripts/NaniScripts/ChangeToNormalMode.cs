using UnityEngine;
using Naninovel;

[CommandAlias("normalMode")]
public class ChangeToNormalMode : Command {

    [ParameterAlias("reset")]
    public BooleanParameter ResetState = true;

    public override async UniTask ExecuteAsync (AsyncToken asyncToken = default) {

        Debug.Log("Changing back to normal mode");

        // 1. Switch cameras.
        var advCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        advCamera.enabled = true;
        var naniMainCamera = Engine.GetService<ICameraManager>().Camera;
        naniMainCamera.enabled = false;
        var naniUICamera = Engine.GetService<ICameraManager>().UICamera;
        naniUICamera.enabled = false;

        // 2. Disable Naninovel input + event system.
        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = false;
        // GameObject blockUI = GameObject.Find("Naninovel<Runtime>/ContinueInputUI");
        // blockUI.SetActive(false);
        
        // 3. Stop script player.
        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        scriptPlayer.Stop();

        // 4. Reset state.
        if (ResetState) {
            var stateManager = Engine.GetService<IStateManager>();
            await stateManager.ResetStateAsync();            
        }

        // 5. Enable input
        GameObject gameMasterGo = GameObject.Find("Grid/Tilemap");
        if (gameMasterGo != null) {
            GameMaster gameMaster = gameMasterGo.GetComponent<GameMaster>();
            gameMaster.EnableInput();
        }

    }
}