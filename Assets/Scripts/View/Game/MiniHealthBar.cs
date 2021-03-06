using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniHealthBar : AbstractMenu {

    public PlayerInfo playerInfo;
    public PlayerAnimator playerAnimator;
    public Slider healthSlider;
    private Vector3 spriteGameScale = new Vector3(1.0f, 0.6f, 1.0f);

    protected override void Awake() {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start() {
        healthSlider = gameObject.transform.Find("HealthBarContainer").gameObject.GetComponent<Slider>();
        gameObject.GetComponent<Canvas>().sortingLayerName = "UI";
    }

    // Update is called once per frame
    void Update() {
        //update player info

        if (playerInfo != null) {
            float healthRatio = ((float) playerInfo.currentHealth) / playerInfo.baseHealth;
            updateHealth(healthRatio);            
        }

        if (playerAnimator != null) {
            updatePosition();        
        }

        playerInfo = updatePlayerInfo();
        
    }

    public void Initialize(PlayerInfo newPlayerInfo, PlayerAnimator newPlayerAnimator) {
        playerInfo = newPlayerInfo;
        playerAnimator = newPlayerAnimator;
    }

    private void updatePosition() {
        Vector3 pos = playerAnimator.transform.position;
        transform.position = new Vector3(pos.x, pos.y - 0.45f, pos.z);
        transform.localScale = spriteGameScale;
    }

    private void updateHealth(float healthRatio) {
        healthSlider.value = (float) System.Math.Round(healthRatio, 2);
    }

    private bool isValid() {
        return playerInfo != null && playerAnimator != null && healthSlider != null;
    }

    private PlayerInfo updatePlayerInfo() {
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
        return sharedResourceBus.GetPlayerInfoAtPos(pos);
    }
}
