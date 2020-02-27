using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoSetting : MonoBehaviour
{
    public void GotoSettings()
    {
        SceneManager.LoadScene(2);  
    }
}
