using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class enables/disables Panel object

public class PanelToggler : MonoBehaviour
{
    public GameObject Panel;
    
    public void TogglePanel()
    {
        if (Panel != null) 
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
