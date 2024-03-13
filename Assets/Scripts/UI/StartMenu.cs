using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour, IDataPersistence
{

    [SerializeField]PlayerInput playerInput;
    InputAction menuAction;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject instructionsMenu;

    [SerializeField] PlayerController playerController;

    [SerializeField] Slider volume;
    [SerializeField] Slider sensibility;

    [SerializeField] GameRules rules;
    [SerializeField] AudioSource[] audios;

    private void Start()
    {
        menuAction = playerInput.actions.FindAction("menu");
        menuAction.performed += showStartMenu;
        hideAll();
        DataPersistenceManager.instance.loadGame();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        instructionsMenu.SetActive(true);
    }


    private void Update()
    {
    }

    void hideAll()
    {
        settingsMenu.SetActive(false);
        startMenu.SetActive(false);
    }

    public void changeVolume()
    {
        foreach (AudioSource audio in audios)
        {
            audio.volume = volume.value;
        }
    }
    public void changeSensibility()
    {
        playerController.lookSpeed = sensibility.value;
    }

    public void resume()
    {
        hideAll();
        rules.isPause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void showStartMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            hideAll();
            startMenu.SetActive(true);
            rules.isPause = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
    public void back()
    {
        DataPersistenceManager.instance.loadGame();
        hideAll();
        startMenu.SetActive(true);
    }

    public void save()
    {
        DataPersistenceManager.instance.saveGame();
    }

    public void options()
    {
        hideAll();
        settingsMenu.SetActive(true);
    }

    public void closeInstructions()
    {
        instructionsMenu.SetActive(false);
        rules.isPause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void LoadData(SettingsData data)
    {
        this.volume.value = data.volume;
        this.sensibility.value = data.sensibility;
    }

    public void SaveData(ref SettingsData data)
    {
        data.volume = this.volume.value;
        data.sensibility = this.sensibility.value;
    }

    private void OnDestroy()
    {
        menuAction.performed -= showStartMenu;
    }
}
