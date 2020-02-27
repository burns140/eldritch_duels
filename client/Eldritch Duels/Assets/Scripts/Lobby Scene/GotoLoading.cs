using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoLoading : MonoBehaviour
{
    public void MovetoLoading()
    {
        SceneManager.LoadScene(8);
    }
}
