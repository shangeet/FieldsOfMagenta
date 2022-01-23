using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpScreen : MonoBehaviour {

    GameObject playerExpBar;
    GameObject playerExpGainMenu;

    bool screenAnimationStarted;
    bool playerExpScreenShown;

    void Awake() {
        setupUIElements();
    }

    void Start() {}

    void Update() {
        if (screenAnimationStarted && !playerExpScreenShown) {
            closePlayerExpScreen();
        }
    }

    void setupUIElements() {
        playerExpBar = GameObject.Find("PlayerExpScreen").gameObject;
        playerExpGainMenu = GameObject.Find("ExpGainMenu").gameObject;
        playerExpGainMenu.SetActive(false);
        playerExpBar.SetActive(false);
        playerExpScreenShown = false;
        screenAnimationStarted = false;
    }

    public void ShowPlayerGainExpScreen(PlayerInfo oldPlayerInfo, PlayerInfo newPlayerInfo, List<int> totalExpToLvlUp, int timesLeveledUp, Vector3 playerCurrentPos) {
        print("LEVELD UP");
        print(timesLeveledUp);
        playerExpBar.SetActive(true);
        Vector3 barPosition = new Vector3(playerCurrentPos.x + 0.5f, playerCurrentPos.y - 0.1f, playerCurrentPos.z);
        playerExpBar.transform.position = barPosition;
        playerExpScreenShown = true;
        screenAnimationStarted = true;
        GameObject barContent = playerExpBar.transform.GetChild(0).gameObject;
        StartCoroutine(setExpGainOnBar(barContent, oldPlayerInfo, newPlayerInfo, totalExpToLvlUp, timesLeveledUp));
    }

    IEnumerator setExpGainOnBar(GameObject expBar, PlayerInfo oldPlayerInfo, PlayerInfo newPlayerInfo, List<int> totalExpToLvlUp, int timesLeveledUp) {

        int prevExp = oldPlayerInfo.totalExperience;
        int currExp = newPlayerInfo.totalExperience;

        if (timesLeveledUp == 0) {
            int totalExp = totalExpToLvlUp[0];
            StartCoroutine(animateExpGainOnBar(expBar, prevExp, currExp, totalExp));
            yield return new WaitForSeconds(2);
        } else {
            int totalExp;
            // animate initial level up separately
            totalExp = totalExpToLvlUp[0];
            StartCoroutine(animateExpGainOnBar(expBar, prevExp, totalExp, totalExp));
            yield return new WaitForSeconds(2);
            // animate terminal level ups
            for (int i = 1; i < timesLeveledUp - 1; i++) {
                totalExp = totalExpToLvlUp[i];
                StartCoroutine(animateExpGainOnBar(expBar, 0, totalExp, totalExp));
                yield return new WaitForSeconds(2); 
            }   
            // animate current exp gain for current level   
            totalExp = totalExpToLvlUp[totalExpToLvlUp.Count - 1];
            StartCoroutine(animateExpGainOnBar(expBar, 0, currExp, totalExp));
            yield return new WaitForSeconds(2);

            //show exp gain menu
            playerExpGainMenu.SetActive(true);
            playerExpGainMenu.GetComponent<ExpGainMenu>().OpenExpGainMenu(oldPlayerInfo, newPlayerInfo);
        }
        playerExpScreenShown = false;
    } 

    IEnumerator animateExpGainOnBar(GameObject expBar, int prevExp, int currExp, int totalExpLvlUp) {
        float initialExp = (float) prevExp;
        float currentExp = (float) currExp;
        float totalExp = (float) totalExpLvlUp;
        float step = 10.0f;
        while ((currentExp - initialExp) > 0.05f) {
            initialExp += step;
            float value = ((float) initialExp) / totalExp;
            expBar.GetComponent<ExpBar>().SetExperience(value);
            yield return new WaitForSeconds(0.01f);
        }
    }

    void closePlayerExpScreen() {
        screenAnimationStarted = false;  
        playerExpBar.SetActive(false);     
    }

    public bool IsPlayerExpScreenShown() {
        return playerExpScreenShown;
    }

    public bool IsExperienceScreenProcessing() {
        return playerExpScreenShown || (playerExpGainMenu.activeSelf && playerExpGainMenu.GetComponent<ExpGainMenu>().IsExpGainMenuDisplayed());
    }

}
