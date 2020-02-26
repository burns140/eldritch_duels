using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingToggle : MonoBehaviour
{

    public GameObject Panel;

    public void ToggleButton() 
    {
        if (Panel != null) 
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
