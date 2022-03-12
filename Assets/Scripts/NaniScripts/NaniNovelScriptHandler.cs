using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;

public class NaniNovelScriptHandler {

    private string scriptName;
    private string novelEventName;

    public NaniNovelScriptHandler(string scriptName, string novelEventName) {
        this.scriptName = scriptName;
        this.novelEventName = novelEventName;
    }

    public async void PlayNaniNovelScript() {
        var switchCommand = new ChangeToNovelMode { ScriptName = scriptName, NovelEventName = novelEventName };
	    switchCommand.ExecuteAsync().Forget();
    }

}
