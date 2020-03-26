using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyPack : MonoBehaviour
{
    public GameObject succeedPanel;
    public GameObject failedPanel;
    public Text moneyText;


    public void buy() 
    {
        // TODO: check if money is enough to buy the pack

        bool succeed = true;

        if (succeed)
            succeedPanel.SetActive(true);
        else
            failedPanel.SetActive(true);

        // TODO: make sure you update new money value to the server
 
    }

    // update money text field
    void Update()
    {
        // constantly change money value
        // TODO: update money from server
        moneyText.text = "500";
    }
}
