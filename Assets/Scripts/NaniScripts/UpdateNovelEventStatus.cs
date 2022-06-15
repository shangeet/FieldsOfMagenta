using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;

[CommandAlias("setNovelEventCompleted")]
public class UpdateNovelEventStatus : Command {

    [RequiredParameter] public StringParameter eventName = "";

    public override async UniTask ExecuteAsync (AsyncToken asyncToken = default) {
        Debug.Log(eventName);
        Debug.Log("Naninovel event manager updated");

        SharedResourceBus sharedResourceBus = GameObject.Find("SharedResourceBus").GetComponent<SharedResourceBus>();
        NovelEventManager novelEventManager = sharedResourceBus.GetNovelEventManager();
        Debug.Log(eventName);
        await novelEventManager.MarkEventCompleted(eventName);
        await novelEventManager.SetEventRunning(false);  
    }
    
}
