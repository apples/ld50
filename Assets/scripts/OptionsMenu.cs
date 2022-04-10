using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMasterVolume(float value){
        if(value == -40f){
            value = -80f;
        }
        audioMixer.SetFloat("masterVolume", value);
    }
    public void SetSFXVolume(float value){
        if(value == -40f){
            value = -80f;
        }
        audioMixer.SetFloat("sfxVolume", value);
    }
    public void SetMusicVolume(float value){
        if(value == -40f){
            value = -80f;
        }
        audioMixer.SetFloat("musicVolume", value);
    }
}
