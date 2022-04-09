using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadScene("gameScene");
    }

    public void QuitGame(){
        Debug.Log("Quitting Game...");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void GoToIntroMenu(){
        SceneManager.LoadScene("introMenu");
    }
}
