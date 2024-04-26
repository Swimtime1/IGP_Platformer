using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BrambleController : MonoBehaviour
{
    #region Variables

    // GameObject Variables
    [SerializeField] private GameObject group;
    
    // AudioSource Variables
    [SerializeField] private AudioSource audio;

    // Tilemap Variables
    [SerializeField] private Tilemap sr;

    // ParticleSystem Variables
    [SerializeField] private ParticleSystem flames;

    #endregion
    
    // Dissolves bramble
    public IEnumerator Dissolve(bool dissolving, bool isDissolvable)
    {
        audio.Play();
        flames.Play();
        
        float r = sr.color.r;
        float g = sr.color.g;
        float b = sr.color.b;
        float a = sr.color.a * 255;

        // fades the bramble
        while((a > 0) && isDissolvable && dissolving)
        {
            a -= 1f;
            sr.color = new Color(r, g, b, (a / 255f));
            yield return new WaitForSeconds(0.01f);
        }

        dissolving = false;
        flames.Stop();
        audio.Stop();

        // makes sure other finished dissolving rather than player moved
        if(a <= 0) { Destroy(); }
    }

    // Spreads flames to adjacent bramble
    public void Spread(bool dissolving, bool isDissolvable)
    {
        int numAdj = group.transform.childCount;

        // calls Dissolve for each bramble in group
        for(int i = 0; i < numAdj; i++)
        {
            // skips this object
            if(group.transform.GetChild(i) == this) { continue; }

            BrambleController bc = group.transform.GetChild(i).GetComponent<BrambleController>();
            StartCoroutine(bc.Dissolve(dissolving, isDissolvable));
        }
    }

    // Turns off this, and adjacent, bramble
    private void Destroy()
    {
        int numAdj = group.transform.childCount;

        // calls Dissolve for each bramble in group
        for(int i = 0; i < numAdj; i++)
        {
            // skips this object
            if(group.transform.GetChild(i) == this) { continue; }

            group.transform.GetChild(i).gameObject.SetActive(false);
        }

        this.gameObject.SetActive(false);
    }
}
