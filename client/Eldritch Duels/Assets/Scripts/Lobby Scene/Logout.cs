﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using eldritch;
public class Logout : MonoBehaviour
{
    // Start is called before the first frame update
    public void MoveToMain()
    {
        Global.logout();
        SceneManager.LoadScene(0);  // 0 is the index of Main Scene
    }

}
