using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;
using Naninovel.Commands;
using UnityEngine.SceneManagement;

[CommandAlias("changeScene")]
public class ChangeToBattleMode : Command {

    [RequiredParameter] public StringParameter nextScenePath;

    public override UniTask ExecuteAsync (AsyncToken asyncToken = default) {
        Debug.Log("Executing async...");
        Debug.Log("Scene Path: " + nextScenePath);
        if (Assigned(nextScenePath)) {
            Debug.Log(nextScenePath);
            ExitNaniNovel();
        }
        return UniTask.CompletedTask;
    }

    async void ExitNaniNovel() {
        var stateManager = Engine.GetService<IStateManager>();
        await stateManager.ResetStateAsync();
        ChangeScene();
    }

    void ChangeScene() {
        SceneManager.LoadScene(nextScenePath, LoadSceneMode.Single);
    }
}
