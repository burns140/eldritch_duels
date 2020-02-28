using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;

public class Quit : MonoBehaviour
{
    public UnityEngine.UI.Button quitbutton;

    public void Start()
    {
        quitbutton.onClick.AddListener(doExitGame);
    }
    public void doExitGame()
    {
        Debug.Log("Has quit Game");
        Application.Quit();
    }
}
