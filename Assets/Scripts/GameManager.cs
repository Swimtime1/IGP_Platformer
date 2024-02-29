using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    #region Variables
    
    // GameObject Variables
    public GameObject startScreen, winScreen, inGameUI, pauseScreen, chooseScreen;
    public GameObject cam;

    // TextMeshProUGUI Variables

    // Boolean Variables
    public static bool gameActive;

    // Integer Variables

    // BoxCollider2D Variables

    // SpriteRenderer Variables

    // Sprite Variables

    // Image Variables

    // Script Variables
    public LevelManager lm;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        gameActive = false;
    }

    /* // Update is called once per frame
    void Update()
    {
        
    } */

    #region Menu Operations
    
    // Deactivates every menu
    void CloseMenus()
    {
        startScreen.SetActive(false);
        chooseScreen.SetActive(false);
        winScreen.SetActive(false);
        inGameUI.SetActive(false);
        pauseScreen.SetActive(false);
    }

    // Opens the Start Screen
    public void OpenStart()
    {
        gameActive = false;
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
    }

    // Opens the Pause Menu
    public void OpenPause()
    {
        gameActive = false;
        CloseMenus();
        pauseScreen.SetActive(true);
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
}
