using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingToggle : MonoBehaviour
{

    public GameObject Panel;

    public void ToggleButton() 
    {
        SceneManager.LoadScene(2);
    }
}
