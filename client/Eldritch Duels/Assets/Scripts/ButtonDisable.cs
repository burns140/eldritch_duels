using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDisable : MonoBehaviour
{
    public GameObject Button;

    public void DiableButton() 
    {
        if (Button != null) 
        {
            Button.SetActive(false);
        }
    }
}
