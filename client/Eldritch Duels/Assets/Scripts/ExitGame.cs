using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
  
    public void doExitGame()
    {
        Debug.Log("Has quit Game");
        Application.Quit();
    }
}
