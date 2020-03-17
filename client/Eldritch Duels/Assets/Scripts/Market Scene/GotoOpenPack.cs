using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoOpenPack : MonoBehaviour
{
    public void MoveToOpenPack()
    {
        SceneManager.LoadScene("OpenPack");  // duel scene
    }
}
