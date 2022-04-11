using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FogPlaneEndGame : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    private IEnumerator coroutine;
    public AudioSource sfxBoom;

    private void Start() {
        coroutine = EndAfterSeconds(3);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("Raft")){
            tmpText.text = "Game Over!";
            tmpText.gameObject.SetActive(true);
            StartCoroutine(coroutine);
            sfxBoom.Play();
        }
    }

    private IEnumerator EndAfterSeconds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("endScene");
    }
}
