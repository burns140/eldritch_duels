using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.IO;
using eldritch;

public class BuyPack : MonoBehaviour
{
    public Button card1;
    public Button card2;
    public Button card3;
    public Button card4;
    public Button card5;

    public Text moneyAmount;

    public Button confirm;

    public Button purchase;

    public Image ErrorPanel;

    public Image card1image;
    public Image card2image;
    public Image card3image;
    public Image card4image;
    public Image card5image;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: Update moneyAmount with current amount of credits from server
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void purchasePack()
    {
        //TODO: CONFIRM CREDIT AMOUNT, SHOW ERROR IF NOT ENOUGH
        bool hasCredits = true;
        if (hasCredits)
        {
            getCollection asdf = new getCollection("openPack", Global.getID(), Global.getToken());
            string res = Global.NetworkRequest(asdf);
            string[] cards = res.Split(',');
            //TODO: ADJUST CREDIT AMOUNT ON SCREEN
            foreach (string c in cards)
            {
                AddCardRequest newcard = new AddCardRequest(Global.getID(), Global.getToken(), "addCard", c);
                string added = Global.NetworkRequest(newcard);
            }
            //TODO: SHOW CARDS ON SCREEN

            //TODO: SHOW CONFIRM BUTTON ONCE ALL CARDS HAVE BEEN CLICKED
        }
        else
        {
            ErrorPanel.gameObject.SetActive(true);
        }
    }

    //TODO: send pack request, verify credit amount, receive cards in comma delimited list, send request to add card for each card, show cards on UI

}
