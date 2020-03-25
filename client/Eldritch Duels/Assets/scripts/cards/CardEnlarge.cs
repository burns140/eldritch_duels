using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using eldritch.cards;
[RequireComponent(typeof(UnityEngine.UI.Image))]
public class CardEnlarge : MonoBehaviour
{
    public GameObject Enlarger;
    public Text owned;
    public Card c;
    public void Enlarge()
    {
        Material m = this.gameObject.GetComponent<UnityEngine.UI.Image>().material;
        if(Enlarger != null)
        {
            Enlarger.GetComponent<UnityEngine.UI.Image>().material = m;
            if(c != null)
            {
                owned.text = "OWNED: " + c.CopiesOwned;
            }
            Enlarger.SetActive(true);
        }
    }
}
