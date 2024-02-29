using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Variables
    
    // GameObject Variables
    public GameObject[] levels;
    public GameObject cam;

    // Integer Variables
    public static int currLev;
    private int maxLev;

    // Script Variables
    public PlayerController playerController;

    // Vector3 Variables
    public Vector3[] startPos, camPos;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        maxLev = 0;

        ChooseLev(currLev);
    }

    // Updates the maximum level unlocked and calls ChooseLev(maxLev + 1)
    public void ChooseLev()
    {
        maxLev++;
        currLev = maxLev;
        
        // makes sure another level is available
        if(maxLev < levels.Length) { ChooseLev(maxLev); }
        else { Debug.Log("Moving to next level"); }
    }

    // Determines which level to open
    public void ChooseLev(int lev)
    {
        // closes previous level if it exists
        if(lev - 1 >= 0) { levels[lev - 1].SetActive(false); }

        levels[lev].SetActive(true);
        playerController.MoveLevels(startPos[lev]);
        cam.transform.position = camPos[lev];
    }
}
