using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class CardEnlarge : MonoBehaviour
{
    public GameObject Enlarger;
    public void Enlarge()
    {
        Material m = this.gameObject.GetComponent<UnityEngine.UI.Image>().material;
        if(Enlarger != null)
        {
            Enlarger.GetComponent<UnityEngine.UI.Image>().material = m;
            Enlarger.SetActive(true);
        }
    }
}
