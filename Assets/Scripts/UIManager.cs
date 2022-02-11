using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;
    
    [SerializeField]
    private Slider masterSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private GameObject optionsPanel;

    private void Start() {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }
    public void ChangeScene(int _sceneIndex) {
        SceneManager.LoadScene(_sceneIndex);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void TriggerPanel() {
        optionsPanel.SetActive(!optionsPanel.active);
    }

    #region Sound Management
    public void ChangeMasterVolume(float _volume) {
        mixer.SetFloat("MasterVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(masterSlider, "MasterVolume");
    }
    public void ChangeSFXVolume(float _volume) {
        mixer.SetFloat("SFXVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(sfxSlider, "SFXVolume");
    }
    public void ChangeMusicVolume(float _volume) {
        mixer.SetFloat("MusicVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(musicSlider, "MusicVolume");
    }

    private void SaveVolumeLevel(Slider _slider, string _prefsName) {
        float sliderValue = _slider.value;
        PlayerPrefs.SetFloat(_prefsName, sliderValue);
    }
    #endregion
}
