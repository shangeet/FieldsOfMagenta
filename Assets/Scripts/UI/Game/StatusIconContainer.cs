using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusIconContainer : AbstractMenu {
    
    public PlayerInfo playerInfo;
    public PlayerAnimator playerAnimator;
    private Dictionary<EffectType, Sprite> statusEffectSpriteDict;
    private Dictionary<BuffAttr, Sprite> buffEffectSpriteDict;
    private GameObject statusEffectPrefabSlot;
    private List<Sprite> statusesToShow = new List<Sprite>();
    private GameMaster gameMaster;
    private static float xPadding = 0.35f;
    private static float yPadding = -0.1f;
    public bool initialized = false;

    protected override void Awake() {
        base.Awake();
    }

    void Start() {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
    }

    void Update() {
        if (initialized) {
            if (playerInfo != null) {
                updateStausesToShow();            
            }

            if (playerAnimator != null) {
                updatePosition();           
                Vector3 playerAnimPos = playerAnimator.transform.position;
                Vector3Int newPlayerPos = new Vector3Int(Mathf.FloorToInt(playerAnimPos.x), Mathf.FloorToInt(playerAnimPos.y), Mathf.FloorToInt(playerAnimPos.z));
                playerInfo = sharedResourceBus.GetPlayerInfoAtPos(newPlayerPos);     
            }            
        }  
    }

    public void Initialize(PlayerInfo newPlayerInfo, PlayerAnimator newPlayerAnimator, Dictionary<EffectType, Sprite> spriteDict, Dictionary<BuffAttr, Sprite> buffSpriteDict, GameObject statusPrefabSlot) {
        playerInfo = newPlayerInfo;
        playerAnimator = newPlayerAnimator;
        statusEffectSpriteDict = spriteDict;
        buffEffectSpriteDict = buffSpriteDict;
        statusEffectPrefabSlot = statusPrefabSlot;
        initialized = true;
    }

    // public void UpdateStatusContainer(PlayerInfo newPlayerInfo) {
    //     if (initialized) {
    //         playerInfo = newPlayerInfo;
    //         if (playerInfo != null) {
    //             updateStausesToShow();            
    //         }

    //         if (playerAnimator != null) {
    //             updatePosition();           
    //             Vector3 playerAnimPos = playerAnimator.transform.position;
    //             Vector3Int newPlayerPos = new Vector3Int(Mathf.FloorToInt(playerAnimPos.x), Mathf.FloorToInt(playerAnimPos.y), Mathf.FloorToInt(playerAnimPos.z));
    //             playerInfo = sharedResourceBus.GetPlayerInfoAtPos(newPlayerPos);     
    //         }            
    //     }        
    // }

    private void updateStausesToShow() {
        List<Sprite> currentStatuses = new List<Sprite>();

        foreach (StatusEffect effect in playerInfo.statusList) {

            if (effect is BuffStatusEffect) {
                BuffStatusEffect buffEffect = effect as BuffStatusEffect;
                if (buffEffectSpriteDict.ContainsKey(buffEffect.GetBuffAttr())) {
                    currentStatuses.Add(buffEffectSpriteDict[buffEffect.GetBuffAttr()]);
                }

            } else if (statusEffectSpriteDict.ContainsKey(effect.effectType)) {
                currentStatuses.Add(statusEffectSpriteDict[effect.effectType]);  
            }
        }
        statusesToShow = currentStatuses;
        updateGameObjectList();
    }

    private void updatePosition() {
        Vector3 pos = playerAnimator.transform.position;
        transform.position = new Vector3(pos.x, pos.y + 0.45f, pos.z);
        //transform.localScale = spriteGameScale;       
    }

    private void updateGameObjectList() {
        //destroy old objects
        GameObject statusContainer = GameObject.Find("SC-" + playerInfo.id);
        foreach (Transform child in statusContainer.transform) {
            Destroy(child.gameObject);
        }

        int slots = playerInfo.statusList.Count;
        float yPosStart = yPadding;
        float xPos = xPadding;
        float zPos = playerAnimator.transform.position.z;
        float step = (1.0f + yPadding*2) / slots;
        for (int i = 0; i < slots; i++) {
            float yPos = yPosStart - (step*i);
            StatusEffect playerStatusEffect = playerInfo.statusList[i];
            GameObject piStatusRendererGo = (GameObject) Instantiate(statusEffectPrefabSlot);
            //add to status container
            piStatusRendererGo.transform.SetParent(statusContainer.transform);
            //update location
            piStatusRendererGo.transform.localPosition = new Vector3(xPos, yPos, zPos);
            //update icon and turns remaining
            SpriteRenderer renderer = piStatusRendererGo.transform.Find("StatusSprite").GetComponent<SpriteRenderer>();
            if (playerStatusEffect is BuffStatusEffect) {
                BuffStatusEffect buffStatusEffect = playerStatusEffect as BuffStatusEffect;
                renderer.sprite = buffEffectSpriteDict[buffStatusEffect.GetBuffAttr()];  
            } else {
                renderer.sprite = statusEffectSpriteDict[playerStatusEffect.effectType]; 
            }
            TextMeshProUGUI turnsRemText = piStatusRendererGo.transform.Find("TurnsRemaining").gameObject.GetComponent<TextMeshProUGUI>();
            turnsRemText.text = playerStatusEffect.remainingActiveTurns.ToString();
        }

    }
}
