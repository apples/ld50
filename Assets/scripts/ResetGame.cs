using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    public IntScriptableObject maxRaftHeight; 
    
    void Start()
    {
        maxRaftHeight.value = 0;
    }
}
