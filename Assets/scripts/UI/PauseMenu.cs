using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public bool IsGamePaused{ get; private set; }

    [SerializeField] private GameObject pauseMenuGameObject;
    [SerializeField] private GameObject UIGameObject;
    [SerializeField] private IntScriptableObject maxRaftHeight;

    private PlayerInputActions playerInputActions;

    private void Awake() 
    {
        playerInputActions = InputManager.inputActions;
        playerInputActions.Player.Enable();
    }

    void Start()
    {
        IsGamePaused = false;
    }

    void Update()
    {
        if(playerInputActions.Player.Escape.WasPerformedThisFrame()){
            if(IsGamePaused){
                Resume();
            }else{
                Pause();
            }
        }
    }

    public void Pause()
    {
        IsGamePaused = true;
        UIGameObject.SetActive(false);
        pauseMenuGameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume(){
        IsGamePaused = false;
        UIGameObject.SetActive(true);
        pauseMenuGameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart(){
        maxRaftHeight.value = 0;
        Resume();
        SceneManager.LoadScene("gameScene");
    }

    public void GoToIntroMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("introMenu");
    }

    public void QuitGame(){
        Time.timeScale = 1f;
        Debug.Log("Quitting Game...");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
