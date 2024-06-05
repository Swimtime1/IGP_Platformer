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
    [SerializeField] private bool first, second, third;

    // TextMeshProUGUI Variables

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

        first = false;
        second = false;
        third = false;
    }

    #region Button Press

    // Opens the hints box
    public void Open()
    {
        hintButton.SetActive(false);
        close.SetActive(true);
        background.SetActive(true);
    }

    #endregion
}
