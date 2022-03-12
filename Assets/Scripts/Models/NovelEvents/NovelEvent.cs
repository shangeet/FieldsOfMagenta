using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NovelEvent {
    
    public string eventName;
    public string scriptName;
    public List<NovelEventCondition> conditions;

    [System.NonSerialized]
    public bool isCompleted;

    public NovelEvent(string scriptName, List<NovelEventCondition> conditions) {
        this.scriptName = scriptName;
        this.conditions = conditions;
        this.isCompleted = false;
    }

    public bool IsCompleted() {
        return isCompleted;
    }

    public bool IsValid(PlayerInfo info, Vector3Int playerCurrentPosition) {
        bool isValid = true;
        foreach (NovelEventCondition condition in conditions) {
            isValid = isValid && condition.MeetsCondition(info, playerCurrentPosition);
        }
        return isValid;
    }

    public void PlayEvent() {
        isCompleted = true;
        NaniNovelScriptHandler handler = new NaniNovelScriptHandler(scriptName, eventName);
        handler.PlayNaniNovelScript();
    }

}
