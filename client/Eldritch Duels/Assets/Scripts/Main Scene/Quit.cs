using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    public void doExitGame()
    {
        Debug.Log("Has quit Game");
        Application.Quit();
    }
}
