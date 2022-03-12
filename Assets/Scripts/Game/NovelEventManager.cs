using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;

[System.Serializable]
public class NovelEventManager : MonoBehaviour {
    
    public List<NovelEvent> novelEvents;
    private GameMaster gameMaster;
    public bool eventRunning = false;

    // Start is called before the first frame update
    async void Awake() {
        // Initialize Engine + normal mode
        await RuntimeInitializer.InitializeAsync();
    }
    void Start() {
        gameMaster = GameObject.Find("Grid/Tilemap").GetComponent<GameMaster>();
        if (Engine.Initialized) {
            DisableBlockUI();
        } else {
            Engine.OnInitializationFinished += DisableBlockUI;
        }
    }

    void Update() {
        if (Engine.Initialized) {
            EvaluateNovelEvents();   
        } else {
            Engine.OnInitializationFinished += EvaluateNovelEvents;
        }
    }

    public void EvaluateNovelEvents() {
        List<PlayerInfo> validPlayers = gameMaster.GetAllPlayerInfos();
        foreach (NovelEvent novelEvent in novelEvents) {
            if (!novelEvent.IsCompleted()) {
                foreach (PlayerInfo info in validPlayers) {
                    Vector3Int playerPosition = gameMaster.GetPlayerPosition(info);
                    var scriptPlayer = Engine.GetService<IScriptPlayer>();
                    if (novelEvent.IsValid(info, playerPosition) && !eventRunning && !gameMaster.uiAnimationsPlaying()) {
                        print("VALID EVENT!");
                        eventRunning = true;
                        novelEvent.PlayEvent();
                    }                
                }                
            }
        }
    }

    public IEnumerator SetEventRunning(bool isRunning) {
        eventRunning = isRunning;
        yield return new WaitForSeconds(0.01f);
    }

    public bool IsEventRunning() {
        return eventRunning;
    }

    public void DisableBlockUI() {
        GameObject blockUI = GameObject.Find("Naninovel<Runtime>/ContinueInputUI");
        blockUI.SetActive(false);        
    }

    public bool AllEventsPlayed() {
        int eventsPlayed = 0;
        foreach (NovelEvent novelEvent in novelEvents) {
            if (novelEvent.IsCompleted()) {
                eventsPlayed++;
            }
        }
        return eventsPlayed == novelEvents.Count;
    }

    public IEnumerator MarkEventCompleted(string eventName) {
        for (int i = 0; i < novelEvents.Count; i++) {
            NovelEvent novelEvent = novelEvents[i];
            if (novelEvent.eventName.Equals(eventName)) {
                novelEvent.isCompleted = true;
                novelEvents[i] = novelEvent;
                yield return new WaitForSeconds(1);
            }
        }
    }
}
