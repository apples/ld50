using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private string optionsFilename = "Options";

    private List<ResolutionInfo> resolutions;

    private void Awake() {
        LoadSavedOptions();
    }

    private void Start()
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions.Select(x => x.displayName).ToList());
        resolutionDropdown.value = resolutions.FindIndex(x =>
            x.resolution.width == Screen.currentResolution.width &&
            x.resolution.height == Screen.currentResolution.height &&
            x.resolution.refreshRate == Screen.currentResolution.refreshRate);
        resolutionDropdown.RefreshShownValue();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    private void Update()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
    }

    public void SetMasterVolume(float value){
        if(value == -40f){
            value = -80f;
        }
        audioMixer.SetFloat("masterVolume", value);
        SaveCurrentOptions();
    }
    public void SetSFXVolume(float value){
        if(value == -40f){
            value = -80f;
        }
        audioMixer.SetFloat("sfxVolume", value);
        SaveCurrentOptions();
    }
    public void SetMusicVolume(float value){
        if(value == -40f){
            value = -80f;
        }
        audioMixer.SetFloat("musicVolume", value);
        SaveCurrentOptions();
    }

    public void SetResolutionIndex(int value)
    {
        var res = resolutions[value].resolution;
        Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
        SaveCurrentOptions();
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    public void LoadSavedOptions(){
        resolutions = Screen.resolutions.Select(x => new ResolutionInfo(x)).ToList();

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

        if (optionsEntry.screenWidth == 0)
        {
            optionsEntry.screenWidth = Screen.currentResolution.width;
            optionsEntry.screenHeight = Screen.currentResolution.height;
            optionsEntry.screenRefreshRate = Screen.currentResolution.refreshRate;
            optionsEntry.fullscreen = Screen.fullScreen;
        }
        else
        {
            var idx = resolutions.FindIndex(x =>
                x.resolution.width == optionsEntry.screenWidth &&
                x.resolution.height == optionsEntry.screenHeight &&
                x.resolution.refreshRate == optionsEntry.screenRefreshRate);

            if (idx >= 0)
            {
                SetResolutionIndex(idx);
            }

            Screen.fullScreen = optionsEntry.fullscreen;
        }
    }

    public void SaveCurrentOptions(){
        OptionsEntry optionsEntry = new OptionsEntry
        {
            masterVolume = masterSlider.value,
            sfxVolume = sfxSlider.value,
            musicVolume = musicSlider.value,
            screenWidth = Screen.currentResolution.width,
            screenHeight = Screen.currentResolution.height,
            screenRefreshRate = Screen.currentResolution.refreshRate,
            fullscreen = Screen.fullScreen,
        };
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

        optionsEntry.screenWidth = Screen.currentResolution.width;
        optionsEntry.screenHeight = Screen.currentResolution.height;
        optionsEntry.screenRefreshRate = Screen.currentResolution.refreshRate;
        optionsEntry.fullscreen = Screen.fullScreen;

        SaveOptions(optionsEntry);
    }

    [System.Serializable]
    private class OptionsEntry{
        public float musicVolume;
        public float sfxVolume;
        public float masterVolume;

        public int screenWidth;
        public int screenHeight;
        public int screenRefreshRate;

        public bool fullscreen;
    }

    private class ResolutionInfo
    {
        public Resolution resolution;
        public string displayName;

        public ResolutionInfo(Resolution res)
        {
            resolution = res;
            displayName = $"{res.width}x{res.height}@{res.refreshRate}";
        }
    }
}
