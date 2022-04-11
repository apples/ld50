using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    public IntScriptableObject maxRaftHeight; 
    public OptionsMenu optionsMenu;
    
    void Start()
    {
        // uncomment this to test coming in as a fresh player
        // PlayerPrefs.DeleteAll();
     
        maxRaftHeight.value = 0;
        optionsMenu.LoadSavedOptions();
    }
}
