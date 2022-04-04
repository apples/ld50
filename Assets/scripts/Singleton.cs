using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance{ get; private set;}
    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this.gameObject);
        }else{
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
