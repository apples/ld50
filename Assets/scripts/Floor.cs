using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Floor : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    private IEnumerator coroutine;

    private List<string> destroyOnFloorCollisionTags = new List<string>{"Crate"};

    private void OnCollisionEnter(Collision other) {
        coroutine = EndAfterSeconds(3);
        if(tmpText == null && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Raft"))){
            SceneManager.LoadScene("endScene");
        }else if(other.gameObject.CompareTag("Player")){
            tmpText.text = "Game Over!\nSadie Hit the Ground...";
            tmpText.gameObject.SetActive(true);
            StartCoroutine(coroutine);
        }else if(other.gameObject.CompareTag("Raft")){
            tmpText.text = "Game Over!\nThe Raft Hit the Ground...";
            tmpText.gameObject.SetActive(true);
            StartCoroutine(coroutine);
        }

        if(destroyOnFloorCollisionTags.Contains((other.gameObject.tag))){
            Destroy(other.gameObject);
        }
    }

    private IEnumerator EndAfterSeconds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("endScene");
    }
}
