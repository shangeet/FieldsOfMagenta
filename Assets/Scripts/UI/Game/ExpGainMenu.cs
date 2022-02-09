using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpGainMenu : MonoBehaviour {

    GameObject expGainMenu;
    bool expGainMenuDisplayed;

    void Awake() {
        setupUIElements();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && expGainMenuDisplayed) {
            CloseExpGainMenu();
        }
    }

    void setupUIElements() {
        expGainMenu = GameObject.Find("ExpGainMenu");
        expGainMenuDisplayed = false;
    }

    public void OpenExpGainMenu(PlayerInfo oldPlayerInfo, PlayerInfo newPlayerInfo) {
        expGainMenuDisplayed = true;
        Image playerPortrait = expGainMenu.transform.GetChild(3).gameObject.GetComponent<Image>() as Image;
        playerPortrait.sprite = newPlayerInfo.playerAnimator.playerPortrait;

        //get stats
        string[] statNames = new string[8] {
            "HP",
            "ATK",
            "DEF",
            "MATK",
            "MDEF",
            "DEX",
            "LUK",
            "MOV"
        };

        int[] newStats = new int[8] {
            newPlayerInfo.baseHealth,
            newPlayerInfo.baseAttack,
            newPlayerInfo.baseDefense,
            newPlayerInfo.baseMagicAttack,
            newPlayerInfo.baseMagicDefense,
            newPlayerInfo.baseDexterity,
            newPlayerInfo.baseLuck,
            newPlayerInfo.baseMov
        };

        int[] oldStats = new int[8] {
            oldPlayerInfo.baseHealth,
            oldPlayerInfo.baseAttack,
            oldPlayerInfo.baseDefense,
            oldPlayerInfo.baseMagicAttack,
            oldPlayerInfo.baseMagicDefense,
            oldPlayerInfo.baseDexterity,
            oldPlayerInfo.baseLuck,
            oldPlayerInfo.baseMov
        };

        //populate left side
        GameObject statSlotListLeft = expGainMenu.transform.GetChild(2).gameObject;
        float yPadding = 1.5f;
        for (int i = 0; i < 4; i++) {
            GameObject slotRow = expGainMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            Text statName = slotRow.transform.GetChild(0).gameObject.GetComponent<Text>();
            Text statValue = slotRow.transform.GetChild(1).gameObject.GetComponent<Text>();
            Text statBoostValue = slotRow.transform.GetChild(2).gameObject.GetComponent<Text>();
            GameObject arrowAnim = slotRow.transform.GetChild(3).gameObject;
            
            int statDiff = newStats[i] - oldStats[i];
            statName.text = statNames[i];
            statValue.text = newStats[i].ToString();
            if (statDiff > 0) {
                statBoostValue.text = "+" + statDiff.ToString();
            } else {
                statBoostValue.text = "";
                arrowAnim.SetActive(false);
            }
            Vector3 currentPosition = slotRow.transform.localPosition;
            Vector3 newPosition = new Vector3(1.8f, yPadding, currentPosition.z); 
            slotRow.transform.position = newPosition;
            slotRow.transform.SetParent(statSlotListLeft.transform, false);
            slotRow.transform.localScale = new Vector3(0.025f, 0.025f, 1);
            yPadding -= 0.5f;
        }

        //populate right side
        GameObject statSlotListRight = expGainMenu.transform.GetChild(1).gameObject;
        yPadding = 1.5f;
        for (int i = 4; i < 8; i++) {
            GameObject slotRow = expGainMenu.GetComponent<ItemMenuSpawner>().SpawnPrefab();
            Text statName = slotRow.transform.GetChild(0).gameObject.GetComponent<Text>();
            Text statValue = slotRow.transform.GetChild(1).gameObject.GetComponent<Text>();
            Text statBoostValue = slotRow.transform.GetChild(2).gameObject.GetComponent<Text>();
            GameObject arrowAnim = slotRow.transform.GetChild(3).gameObject;
            
            int statDiff = newStats[i] - oldStats[i];
            statName.text = statNames[i];
            statValue.text = newStats[i].ToString();
            if (statDiff > 0) {
                statBoostValue.text = "+" + statDiff.ToString();
            } else {
                statBoostValue.text = "";
                arrowAnim.SetActive(false);
            }
            Vector3 currentPosition = slotRow.transform.localPosition;
            Vector3 newPosition = new Vector3(1.6f, yPadding, currentPosition.z); 
            slotRow.transform.position = newPosition;
            slotRow.transform.SetParent(statSlotListRight.transform, false);
            slotRow.transform.localScale = new Vector3(0.025f, 0.025f, 1);
            yPadding -= 0.5f;
        }
    }

    public void CloseExpGainMenu() {
        Image playerPortrait = expGainMenu.transform.GetChild(3).gameObject.GetComponent<Image>() as Image;
        playerPortrait.sprite = null;       
        //destroy slots
        GameObject statSlotListRight = expGainMenu.transform.GetChild(1).gameObject;
        GameObject statSlotListLeft = expGainMenu.transform.GetChild(2).gameObject;
        for (int i = 0; i < 4; i++) {
            Destroy(statSlotListLeft.transform.GetChild(i).gameObject);
            Destroy(statSlotListRight.transform.GetChild(i).gameObject);
        }
        expGainMenu.SetActive(false);
        expGainMenuDisplayed = false;
    }

    public bool IsExpGainMenuDisplayed() {
        return expGainMenuDisplayed;
    }
}
