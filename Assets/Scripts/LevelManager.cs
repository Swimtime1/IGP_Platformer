using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    #region Variables
    
    // GameObject Variables
    public GameObject[] levels;
    [SerializeField] private GameObject[] levelButtons, pushBlocks;
    public GameObject cam, backGround;

    // Integer Variables
    [SerializeField] private int currLev, maxLev;

    // Script Variables
    public PlayerController playerController;

    // Vector3 Variables
    public Vector3[] startPos, camPos, pushBlockPos;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        maxLev = currLev;

        ChooseLev(currLev);

        UnlockThrough();
    }

    // Updates the maximum level unlocked and calls ChooseLev(currLev + 1)
    public void ChooseLev()
    {
        currLev++;
        if(currLev > maxLev) { UnlockMax(); }
        
        // makes sure another level is available
        if(maxLev < levels.Length) { ChooseLev(currLev); }
        else { gameObject.GetComponent<GameManager>().OpenWin(); }
    }

    // Determines which level to open
    public void ChooseLev(int lev)
    {
        // closes previous level if it exists
        if(lev - 1 >= 0) { levels[lev - 1].SetActive(false); }
        
        Reset();

        currLev = lev;
        levels[lev].SetActive(true);
        playerController.MoveLevels(startPos[lev]);
        cam.transform.position = camPos[lev];
        backGround.transform.position = new Vector3(camPos[lev].x, camPos[lev].y, 0f);
    }

    #region Getters

    // Returns the value of maxLev
    public int GetMaxLev()
    {
        return maxLev;
    }

    #endregion

    #region Unlocks

    // Unlocks every level in the Level Select Menu up to maxLev
    private void UnlockThrough()
    {
        // loops through every level through maxLev
        for(int i = 0; i <= maxLev; i++)
        {
            levelButtons[i].GetComponent<Button>().interactable = true;
            levelButtons[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        // loops through every level through maxLev
        for(int i = maxLev + 1; i < levelButtons.Length; i++)
        {
            levelButtons[i].GetComponent<Button>().interactable = false;
            levelButtons[i].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    // Unlocks the new Maximum Level
    private void UnlockMax()
    {
        maxLev = currLev;

        // makes sure maxLev is inbounds before unlocking
        if(maxLev < levelButtons.Length)
        {
            levelButtons[maxLev].GetComponent<Button>().interactable = true;
            levelButtons[maxLev].transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    #endregion

    #region Resets

    // Determines which level to reset
    private void Reset()
    {
        // Resets Bramble
        
        
        // Resets each Push Block
        for(int i = 0; i < pushBlocks.Length; i++)
        { pushBlocks[i].transform.position = pushBlockPos[i]; }
    }

    #endregion
}
