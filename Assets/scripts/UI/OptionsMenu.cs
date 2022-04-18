using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;


    private string optionsFilename = "Options";

    private void Awake() {
        LoadSavedOptions();
    }

    public void SetMasterVolume(float value){
        Debug.Log(value);
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

    public void LoadSavedOptions(){
        OptionsEntry optionsEntry = JsonUtility.FromJson<OptionsEntry>(PlayerPrefs.GetString(optionsFilename));
        if(optionsEntry == null){
            SetDefaultOptions();
        }
        optionsEntry = JsonUtility.FromJson<OptionsEntry>(PlayerPrefs.GetString(optionsFilename));
        SetMasterVolume(optionsEntry.masterVolume);
        masterSlider.value = optionsEntry.masterVolume;
        SetMusicVolume(optionsEntry.musicVolume);
        musicSlider.value = optionsEntry.musicVolume;
        SetSFXVolume(optionsEntry.sfxVolume);
        sfxSlider.value = optionsEntry.sfxVolume;
    }

    public void SaveCurrentOptions(){
        OptionsEntry optionsEntry = new OptionsEntry{masterVolume = masterSlider.value, sfxVolume = sfxSlider.value, musicVolume = musicSlider.value};
        SaveOptions(optionsEntry);
    }

    private void SaveOptions(OptionsEntry optionsEntry){
        string json = JsonUtility.ToJson(optionsEntry);
        Debug.Log("Saving Options...\n" + json);
        PlayerPrefs.SetString(optionsFilename, json);
        PlayerPrefs.Save();
    }

    public void SetDefaultOptions(){
        Debug.Log("OptionsMenu setting default options");
        OptionsEntry optionsEntry = new OptionsEntry{musicVolume = 0f, sfxVolume = 0f, masterVolume = 0f};
        SaveOptions(optionsEntry);
    }

    [System.Serializable]
    private class OptionsEntry{
        public float musicVolume;
        public float sfxVolume;
        public float masterVolume;
        
    }
}
