using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    #region Variables
    
    // GameObject Variables
    public GameObject[] levels;
    [SerializeField] private GameObject[] levelButtons;
    public GameObject cam;

    // Integer Variables
    [SerializeField] private int currLev, maxLev;

    // Script Variables
    public PlayerController playerController;

    // Vector3 Variables
    public Vector3[] startPos, camPos;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        maxLev = currLev;

        ChooseLev(currLev);
    }

    // Updates the maximum level unlocked and calls ChooseLev(currLev + 1)
    public void ChooseLev()
    {
        currLev++;
        if(currLev > maxLev) { maxLev = currLev; } // updates maxLev if necessary
        
        // makes sure another level is available
        if(maxLev < levels.Length) { ChooseLev(currLev); }
        else { Debug.Log("Moving to next level"); }
    }

    // Determines which level to open
    public void ChooseLev(int lev)
    {
        // closes previous level if it exists
        if(lev - 1 >= 0) { levels[lev - 1].SetActive(false); }

        currLev = lev;
        levels[lev].SetActive(true);
        playerController.MoveLevels(startPos[lev]);
        cam.transform.position = camPos[lev];
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
        for(int i = 0; i < maxLev; i++)
        {
            levelButtons[i].GetComponent<Button>().interactable = true;
            levelButtons[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        // loops through every level through maxLev
        for(int i = maxLev; i < levelButtons.Length; i++)
        {
            levelButtons[i].GetComponent<Button>().interactable = false;
            levelButtons[i].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    #endregion
}
