using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region Variables
    
    // GameObject Variables
    public GameObject startScreen, winScreen, inGameUI, pauseScreen, chooseScreen, credits;
    public GameObject cam;

    // TextMeshProUGUI Variables

    // Boolean Variables
    public static bool gameActive, gameStarted;

    // Integer Variables

    // BoxCollider2D Variables

    // SpriteRenderer Variables

    // Sprite Variables

    // Image Variables

    // Script Variables
    public LevelManager lm;
    private PlatformerActions input;

    #endregion

    // Called when the game is loaded
    private void Awake()
    {
        input = new PlatformerActions();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        gameActive = false;
        gameStarted = false;
        OpenStart();
    }

    #region Menu Operations
    
    // Deactivates every menu
    void CloseMenus()
    {
        startScreen.SetActive(false);
        chooseScreen.SetActive(false);
        winScreen.SetActive(false);
        inGameUI.SetActive(false);
        pauseScreen.SetActive(false);
        credits.SetActive(false);
    }

    // Opens the Start Screen
    public void OpenStart()
    {
        gameActive = false;
        gameStarted = false;
        CloseMenus();
        startScreen.SetActive(true);
    }

    // Opens the Menu where Players choose to continue or start a new game
    public void ChooseGame()
    {
        gameActive = false;
        CloseMenus();
        chooseScreen.SetActive(true);
    }

    // Opens the Win Screen
    public void OpenWin()
    {
        gameActive = false;
        CloseMenus();
        winScreen.SetActive(true);
    }

    // Opens the In-Game UI
    public void OpenGameUI()
    {
        gameActive = false;
        CloseMenus();
        inGameUI.SetActive(true);
        gameActive = true;
        gameStarted = true;
    }

    // Opens the Pause Menu
    public void OpenPause()
    {
        gameActive = false;
        CloseMenus();
        pauseScreen.SetActive(true);
    }

    // Opens the Credits Menu
    public void OpenCredits()
    {
        gameActive = false;
        CloseMenus();
        credits.SetActive(true);
    }

    #endregion

    // Moves to the next level
    public void OpenNextLevel()
    {
        gameActive = false;
        cam.SetActive(false);
        lm.ChooseLev();
        cam.SetActive(true);
        gameActive = true;
    }

    #region Input
    
    // Called when the script is enabled
    private void OnEnable()
    {
        input.Enable();
        input.Menus.PauseMenu.performed += OnPausePerformed;
        input.Menus.CloseStart.performed += OnStartPerformed;
    }

    // Called when the script is disabled
    private void OnDisable()
    {
        input.Disable();
        input.Menus.PauseMenu.performed -= OnPausePerformed;
        input.Menus.CloseStart.performed -= OnStartPerformed;
    }

    // Called when any of the binds associated with PauseMenu in input are used
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        // only opens the pause menu if the game is active
        if(pauseScreen.activeSelf) { OpenGameUI(); }
        else if(gameStarted) { OpenPause(); }
    }

    // Called when any of the binds associated with CloseStart in input are used
    private void OnStartPerformed(InputAction.CallbackContext context)
    {
        // only opens the level selection menu if the start menu is active
        if(startScreen.activeSelf) { ChooseGame(); }
    }

    #endregion

    // Causes the game to close
    public void Quit()
    {
        Application.Quit();
    }
}
