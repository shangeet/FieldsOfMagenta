using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour {

    Button startButton; 
    Button loadButton;
    Button configButton;
    Button quitButton;
    [SerializeField]string startButtonScenePath;

    MasterGameStateController masterGameStateController;

    const string startSceneProloguePath = "";
    
    void Start() {
        startButton = GameObject.Find("StartGameButton").GetComponent<Button>() as Button;
        loadButton = GameObject.Find("LoadGameButton").GetComponent<Button>() as Button;
        configButton = GameObject.Find("ConfigButton").GetComponent<Button>() as Button;
        quitButton = GameObject.Find("QuitGameButton").GetComponent<Button>() as Button; 
        startButton.onClick.AddListener(onStartButtonPressed);
        masterGameStateController = GameObject.Find("GameMasterInstance").AddComponent<MasterGameStateController>();
    }

    void onStartButtonPressed() {
        //create new gamemaster instance to record player information
        PlayerInfo newPlayer = new PlayerInfo("1", "Shan", false, BattleClass.Warrior, "images/portraits/test_face");
        newPlayer.setAnimationPaths("images/sprites/CharacterSpriteSheets/Ally/MainCharacter",
            "Animations/MainCharacter/Main_Game",
            "Animations/MainCharacter/Main_Battle");
        masterGameStateController.instance.CreateNewSaveInstance(newPlayer);
        SceneManager.LoadScene(startButtonScenePath, LoadSceneMode.Single);
    }
}
