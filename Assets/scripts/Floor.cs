using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Floor : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Raft")){
            Debug.Log("End game!");
            SceneManager.LoadScene("endScene");
        }
    }
}
