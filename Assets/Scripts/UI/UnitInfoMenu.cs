using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

public class UnitInfoMenu : MonoBehaviour {
    
    GameObject unitInfoMenu; 

    GameMaster gameMaster;

    void Awake() {
        setupUIElements();
    }

    void Start() {
        gameMaster = gameObject.GetComponent<GameMaster>();
    }

    void setupUIElements() {
        // unit info menu
        unitInfoMenu = GameObject.Find("UnitInfoMenu");
        unitInfoMenu.SetActive(false);
    }

    public void openUnitInfoMenu(PlayerInfo playerInfo) {
        unitInfoMenu.SetActive(true);
        PlayerAnimator animator = playerInfo.playerAnimator;
        Image playerFace = GameObject.Find("PlayerFace").GetComponent<Image>() as Image;
        Text hp = GameObject.Find("HPDisplay").GetComponent<Text>() as Text;
        Text atk = GameObject.Find("AtkDisplay").GetComponent<Text>() as Text;
        Text def = GameObject.Find("DefDisplay").GetComponent<Text>() as Text;
        Text mov = GameObject.Find("MovDisplay").GetComponent<Text>() as Text;
        playerFace.sprite = animator.playerPortrait;
        hp.text = "HP " + playerInfo.currentHealth.ToString() + "/" + Mathf.Max(playerInfo.baseHealth, playerInfo.currentHealth).ToString();
        atk.text = "ATK " + playerInfo.currentAttack.ToString();
        def.text = "DEF " + playerInfo.currentDefense.ToString();
        mov.text = "MOV " + playerInfo.currentMov.ToString();
    }

    public void closeUnitInfoMenu() {
        unitInfoMenu.SetActive(false);
    }
}
