using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoAcheivement : MonoBehaviour
{
    public void MovetoMarket()
    {
        SceneManager.LoadScene("Achievement");
    }
}
