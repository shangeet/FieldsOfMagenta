using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;

[CommandAlias("setNovelEventCompleted")]
public class UpdateNovelEventStatus : Command {

    [ParameterAlias("eventName")]
    public StringParameter EventName = "";

    public override async UniTask ExecuteAsync (AsyncToken asyncToken = default) {

        Debug.Log("Naninovel event manager updated");

        GameObject game = GameObject.Find("Grid/Tilemap");
        NovelEventManager novelEventManager = game.GetComponent<NovelEventManager>();
        await novelEventManager.MarkEventCompleted(EventName);
        await novelEventManager.SetEventRunning(false);  
    }
    
}
