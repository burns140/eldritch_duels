using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;

public class Quit : MonoBehaviour
{
    public void doExitGame()
    {
        Debug.Log("Has quit Game");
        Global.client.Close();
        Application.Quit();
    }
}
