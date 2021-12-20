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

    const string startSceneProloguePath = "";
    
    void Start() {
        startButton = GameObject.Find("StartGameButton").GetComponent<Button>() as Button;
        loadButton = GameObject.Find("LoadGameButton").GetComponent<Button>() as Button;
        configButton = GameObject.Find("ConfigButton").GetComponent<Button>() as Button;
        quitButton = GameObject.Find("QuitGameButton").GetComponent<Button>() as Button; 
        startButton.onClick.AddListener(onStartButtonPressed);
    }

    void onStartButtonPressed() {
        SceneManager.LoadScene(startButtonScenePath, LoadSceneMode.Single);
    }
}
