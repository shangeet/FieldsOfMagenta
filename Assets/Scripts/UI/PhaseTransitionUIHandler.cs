using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTransitionUIHandler : MonoBehaviour {
    
    GameObject playerPhaseTransitionImage;
    GameObject enemyPhaseTransitionImage;
    GameMaster gameMaster;

    //keep track of what's on/off display
    bool isPhaseTransitionRunning;

    void Awake() {
        setupUIElements();
    }

    void Start() {
        gameMaster = gameObject.GetComponent<GameMaster>();
    }

    void setupUIElements() {

        //Set player/enemy phase transition images in the right place, turn them off for now
        playerPhaseTransitionImage = GameObject.Find("PlayerPhaseImg");
        enemyPhaseTransitionImage = GameObject.Find("EnemyPhaseImg");
        Vector2 leftMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2 (Camera.main.pixelWidth, Camera.main.pixelHeight/2));
        leftMostScreenPosition.x *= -1;
        playerPhaseTransitionImage.transform.position = leftMostScreenPosition;
        enemyPhaseTransitionImage.transform.position = leftMostScreenPosition;
        playerPhaseTransitionImage.SetActive(false);
        enemyPhaseTransitionImage.SetActive(false);
        isPhaseTransitionRunning = false;
    }

   public IEnumerator translatePhaseImage(string phase) {
        isPhaseTransitionRunning = true;
        if (phase == "PlayerPhase") {
            yield return new WaitForSeconds(1);
            Vector2 rightMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight/2));
            playerPhaseTransitionImage.SetActive(true);
            Vector2 leftMostScreenPosition = playerPhaseTransitionImage.transform.position;

            for(float t = 0.0f; t < 1.0f; t+=0.1f) {
                playerPhaseTransitionImage.GetComponent<ImageTransitions>().TranslateAcrossScreen(leftMostScreenPosition, rightMostScreenPosition, t);

                if (t == 0.5f) {
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(0.05f);
            }
            playerPhaseTransitionImage.transform.position = leftMostScreenPosition;
            playerPhaseTransitionImage.SetActive(false);
        } else if (phase == "EnemyPhase") {
            yield return new WaitForSeconds(1);
            Vector2 rightMostScreenPosition = Camera.main.ScreenToWorldPoint(new Vector2 (Camera.main.pixelWidth, Camera.main.pixelHeight/2));
            enemyPhaseTransitionImage.SetActive(true);
            Vector2 leftMostScreenPosition = enemyPhaseTransitionImage.transform.position;

            for(float t = 0.0f; t < 1.0f; t+=0.1f) {
                enemyPhaseTransitionImage.GetComponent<ImageTransitions>().TranslateAcrossScreen(leftMostScreenPosition, rightMostScreenPosition, t);

                if (t == 0.5f) {
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(0.05f);
            }
            enemyPhaseTransitionImage.transform.position = leftMostScreenPosition;
            enemyPhaseTransitionImage.SetActive(false);
        }
        isPhaseTransitionRunning = false;
    }

    public bool IsPhaseTransitionRunning() {
        return isPhaseTransitionRunning;
    }

}
