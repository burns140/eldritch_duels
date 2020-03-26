using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyPack : MonoBehaviour
{
    public GameObject succeedPanel;
    public GameObject failedPanel;


    public void buy() 
    {
        bool succeed = true;

        if (succeed)
            succeedPanel.SetActive(true);
        else
            failedPanel.SetActive(true);
    }
}
