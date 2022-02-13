using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private GameObject[] menuItems;
    
    [SerializeField]
    private Slider masterSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private GameObject optionsPanel;
    [SerializeField]
    private EventSystem eventSystem;

    private GameObject lastSelectedObject;
    private GameObject newSelectedObject;

    private void Start() {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void LoadScene(int _sceneIndex) {
        SceneManager.LoadScene(_sceneIndex);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void TriggerPanel(GameObject _panel) {
        if (_panel.active) {
            eventSystem.SetSelectedGameObject(lastSelectedObject);
        }
        else {
            lastSelectedObject = eventSystem.currentSelectedGameObject;
            newSelectedObject = _panel.GetComponentInChildren<Button>().gameObject;
            eventSystem.SetSelectedGameObject(newSelectedObject);
        }
        for (int i = 0; i < menuItems.Length; i++) {
            menuItems[i].SetActive(!menuItems[i].active);
        }
        _panel.SetActive(!_panel.active);
        
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
