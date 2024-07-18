using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintsController : MonoBehaviour
{
    #region Variables
    
    // GameObject Variables
    [SerializeField] private GameObject hintButton, left, right, close, background;

    // Boolean Variables
    [SerializeField] private bool firstActive, secondActive;
    [SerializeField] private bool secondUnlocked, thirdUnlocked;

    // TextMeshProUGUI Variables
    [SerializeField] private TextMeshProUGUI text;

    // String Variables
    [SerializeField] private string hint1, hint2, hint3;

    // Script Variables

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        hintButton.SetActive(false);
        left.SetActive(false);
        right.SetActive(false);
        close.SetActive(false);
        background.SetActive(false);

        firstActive = false;
        secondActive = false;
        
        secondUnlocked = false;
        thirdUnlocked = false;

        /* StartCoroutine(UnlockHints()); */
    }

    #region Button Press

    // Opens the hints box
    public void Open()
    {
        hintButton.SetActive(false);
        close.SetActive(true);
        background.SetActive(true);

        // starts process to unlock second hint if necessary
        if(!secondUnlocked && firstActive) { StartCoroutine(UnlockNext()); }
    }

    // Closes the hints box
    public void Close()
    {
        hintButton.SetActive(true);
        close.SetActive(false);
        background.SetActive(false);
    }

    // Moves to an earlier hint
    public void MoveLeft()
    {
        // checks if the second hint is currently active, and otherwise assumes the third
        if(secondActive)
        {
            secondActive = false;
            firstActive = true;

            left.SetActive(false);
            right.SetActive(true);

            text.text = hint1;
        }
        else
        {
            secondActive = true;

            left.SetActive(true);
            right.SetActive(true);

            text.text = hint2;
        }
    }

    // Moves to a later hint
    public void MoveRight()
    {
        // checks if the second hint is currently active, and otherwise assumes the first
        if(secondActive)
        {
            secondActive = false;

            left.SetActive(true);
            right.SetActive(false);

            text.text = hint3;
        }
        else
        {
            firstActive = false;
            secondActive = true;

            left.SetActive(true);
            right.SetActive(false);

            text.text = hint2;

            // starts process to unlock third hint if necessary
            if(!thirdUnlocked) { StartCoroutine(UnlockNext()); }
            else { right.SetActive(true); }
        }
    }

    #endregion

    #region Coroutines

    // Provides the first hint after 30 seconds
    public IEnumerator UnlockHints()
    {
        // counts down 30 seconds
        for(int i = 30; i > 0; i--) { yield return new WaitForSeconds(1.0f); }

        text.text = hint1;
        firstActive = true;
        hintButton.SetActive(true);
    }

    // Provides another hint after 30 seconds
    private IEnumerator UnlockNext()
    {
        // counts down 30 seconds
        for(int i = 30; i > 0; i--) { yield return new WaitForSeconds(1.0f); }

        right.SetActive(true);

        // updates the booleans
        if(!secondUnlocked) { secondUnlocked = true; }
        else { thirdUnlocked = true; }
    }

    #endregion
}
