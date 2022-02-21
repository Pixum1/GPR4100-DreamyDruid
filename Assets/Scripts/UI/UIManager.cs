using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu")]
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

    [SerializeField]
    private Image transitionImage;
    [SerializeField]
    private float transitionSpeed;
    private bool transition;
    private bool menuTransition;
    private int currentMenuLoadIndex;
    [SerializeField]
    private TMP_Text skipText;
    private Color color = new Color(0, 0, 0, 0);
    [SerializeField]
    private TMP_Text transitionText;

    [SerializeField]
    private Button[] worldButtons;


    [Header("InGame")]
    [SerializeField]
    private GameObject pausePanel;

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (menuTransition)
        {
            skipText.enabled = true;
            if (Input.GetButtonDown("Jump"))
            {
                SceneManager.LoadScene(currentMenuLoadIndex);
            }
        }

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            if (Input.GetButtonDown("Escape"))
            {
                TriggerPanel(pausePanel);
            }
            if (!transition)
            {
                if (pausePanel.active)
                {
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            }
        }
        else
        {

            for (int i = 1; i < worldButtons.Length; i++)
            {
                if (PlayerPrefs.GetInt("WorldUnlock") > i)
                {
                    worldButtons[i].interactable = true;
                }
                else
                {
                    worldButtons[i].interactable = false;
                }
            }
        }
    }
    public void LoadScene(int _sceneIndex)
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
            StartCoroutine(InGameTransition(_sceneIndex));
        else
            StartCoroutine(Transition(_sceneIndex));
    }

    private IEnumerator InGameTransition(int _sceneIndex)
    {
        transition = true;
        Time.timeScale = 0f;
        if (pausePanel.active)
            pausePanel.SetActive(false);
        while (transitionImage.color.a < 1)
        {
            color.a += Time.unscaledDeltaTime / transitionSpeed;
            transitionImage.color = color;
            yield return null;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(_sceneIndex);
    }

    private IEnumerator Transition(int _sceneIndex)
    {
        currentMenuLoadIndex = _sceneIndex;
        this.GetComponent<Canvas>().enabled = false;
        while (transitionImage.color.a < 1)
        {
            color.a += Time.unscaledDeltaTime / transitionSpeed;
            transitionImage.color = color;
            yield return null;
        }
        menuTransition = true;
        transitionText.text = $"World {_sceneIndex}";

        while (transitionText.color.a < 1)
        {
            transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, transitionText.color.a + Time.unscaledDeltaTime / transitionSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        menuTransition = false;
        SceneManager.LoadScene(_sceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void TriggerPanel(GameObject _panel)
    {
        if (_panel.active)
        {
            eventSystem.SetSelectedGameObject(lastSelectedObject);
        }
        else
        {
            lastSelectedObject = eventSystem.currentSelectedGameObject;

            newSelectedObject = _panel.GetComponentInChildren<Button>().gameObject;
            eventSystem.SetSelectedGameObject(newSelectedObject);
        }
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].SetActive(!menuItems[i].active);
        }
        _panel.SetActive(!_panel.active);

    }

    #region Sound Management
    public void ChangeMasterVolume(float _volume)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(masterSlider, "MasterVolume");
    }
    public void ChangeSFXVolume(float _volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(sfxSlider, "SFXVolume");
    }
    public void ChangeMusicVolume(float _volume)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(musicSlider, "MusicVolume");
    }

    private void SaveVolumeLevel(Slider _slider, string _prefsName)
    {
        float sliderValue = _slider.value;
        PlayerPrefs.SetFloat(_prefsName, sliderValue);
    }
    #endregion
}
