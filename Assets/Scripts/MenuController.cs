using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour, IDataPersistence
{

    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject btn_back;
    [SerializeField] TMP_Dropdown qualitySettings;
    [SerializeField] Slider volume;
    [SerializeField] Slider sensibility;
    [SerializeField] AudioSource[] audios;

    private void Awake()
    {
        hideAll();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

    }

    public void playGame()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void creditsShow()
    {
        hideAll();
        creditsPanel.SetActive(true);
        btn_back.SetActive(true);
    }

    public void creditsHide()
    {
        creditsPanel.SetActive(false);
    }
    
    public void back()
    {

        DataPersistenceManager.instance.loadGame();
        hideAll();
    }
    public void hideAll()
    {
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        btn_back.SetActive(false);
    }

    public void settingsHide()
    {
        settingsPanel.SetActive(false);
    }
    public void settingsShow()
    {
        DataPersistenceManager.instance.loadGame();
        hideAll();
        settingsPanel.SetActive(true);
        btn_back.SetActive(true);
    }
    #region Settings
    public void changeQuality()
    {
        QualitySettings.SetQualityLevel(qualitySettings.value);
    }

    public void changeVolume()
    {
        foreach(AudioSource audio in audios)
        {
            audio.volume = volume.value;
        }
    }
    public void save()
    {
        DataPersistenceManager.instance.saveGame();
    }

    public void LoadData(SettingsData data)
    {
        this.qualitySettings.value = data.qualityLevel;
        this.volume.value = data.volume;
        this.sensibility.value = data.sensibility;
    }

    public void SaveData(ref SettingsData data)
    {
        data.qualityLevel = this.qualitySettings.value;
        data.volume = this.volume.value;
        data.sensibility = this.sensibility.value;
    }

    #endregion Settings
}
