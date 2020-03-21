using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoMarket : MonoBehaviour
{
    public void MovetoMarket()
    {
        SceneManager.LoadScene("MarketScene");
    }
}
