using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;

public class ScriptRunner : MonoBehaviour {

    [SerializeField] private string naniNovelScriptName;

    private async void Start () {
        await RuntimeInitializer.InitializeAsync();

        if (Engine.Initialized) {
            PlayNaniNovelScript();
        } else {
            Engine.OnInitializationFinished += PlayNaniNovelScript;
        }
    }

    async void PlayNaniNovelScript() {
        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        await scriptPlayer.PreloadAndPlayAsync(naniNovelScriptName);
    }
     
}
