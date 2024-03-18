using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialButtonPress : MonoBehaviour
{
    #region Variables

    // Float Variables
    private float rBut, gBut, bBut;

    // Image Variables
    [SerializeField] private Image button;

    // TextMeshProUGUI Variables
    [SerializeField] private TextMeshProUGUI text;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        rBut = button.color.r;
        gBut = button.color.g;
        bBut = button.color.b;

        StartCoroutine(Blink());
    }

    // Causes Tutorial Buttons to blink
    IEnumerator Blink()
    {
        // stops when the Start Menu is no longer showing
        while(true)
        {
            // decreases transparency
            for(float i = 150f; i < 255f; i++)
            {
                Blink(i);
                yield return new WaitForSeconds(0.005f);
            }

            // increases transparency
            for(float i = 255f; i > 150f; i--)
            {
                Blink(i);
                yield return new WaitForSeconds(0.005f);
            }
        }
    }

    // Updates the Tutorial Buttons' colors
    void Blink(float i)
    {
        Color butCol = new Color(rBut, gBut, bBut, (i/255f));
        Color textCol = new Color(1f, 1f, 1f, (i/255f));
        
        // changes the button's color unless null
        if(button) { button.color = butCol; }
        
        // changes the text's color unless null
        if(text) { text.color = textCol; }
    }
}
