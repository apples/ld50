using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Floor : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public AudioSource sfxBoom;

    private IEnumerator coroutine;

    private void OnCollisionEnter(Collision other) {
        coroutine = EndAfterSeconds(3);
        if(tmpText == null && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Raft"))){
            SceneManager.LoadScene("endScene");
        }else if(other.gameObject.CompareTag("Player")){
            tmpText.text = "Game Over!\nSadie Hit the Ground...";
            tmpText.gameObject.SetActive(true);
            sfxBoom.Play();
            StartCoroutine(coroutine);
        }else if(other.gameObject.CompareTag("Raft")){
            tmpText.text = "Game Over!\nThe Raft Hit the Ground...";
            tmpText.gameObject.SetActive(true);
            sfxBoom.Play();
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator EndAfterSeconds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("endScene");
    }
}
