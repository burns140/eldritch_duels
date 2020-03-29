using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.IO;
using eldritch;
using eldritch.cards;

public class BuyPack : MonoBehaviour
{
    public Button card1;
    public Button card2;
    public Button card3;
    public Button card4;
    public Button card5;

    public Button lobbybutton;

    public Text moneyAmount;

    public Button confirm;

    public Button purchase;

    public Image ErrorPanel;

    public Image CardsPanel;

    public Image card1image;
    public Image card2image;
    public Image card3image;
    public Image card4image;
    public Image card5image;

    public Image card1imageback;
    public Image card2imageback;
    public Image card3imageback;
    public Image card4imageback;
    public Image card5imageback;

    private int clicked = 0;
    // Start is called before the first frame update
    void Start()
    {
        getCollection creds = new getCollection("getCredits", Global.getID(), Global.getToken());
        string amount = Global.NetworkRequest(creds);
        Global.usercredits = Int32.Parse(amount);
        moneyAmount.text = amount;
        card1.onClick.AddListener(clickedButton);
        card2.onClick.AddListener(clickedButton);
        card3.onClick.AddListener(clickedButton);
        card4.onClick.AddListener(clickedButton);
        card5.onClick.AddListener(clickedButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        
    }

    public void purchasePack()
    {
        /*getCollection creds = new getCollection("getCredits", Global.getID(), Global.getToken());
        string amount = Global.NetworkRequest(creds);
        Debug.Log(amount);
        Int32 amountnum = Int32.Parse(amount);
        if (amountnum > 100) //CHANGE TO WHATEVER THE COST SHOULD BE*/
        if (Global.usercredits > 100)
        {
            clicked = 0;
            getCollection asdf = new getCollection("openPack", Global.getID(), Global.getToken());
            string res = Global.NetworkRequest(asdf);
            string[] cards = res.Split(',');
            Global.usercredits -= 100;
            moneyAmount.text = Global.usercredits.ToString();
            CreditRequest decrease = new CreditRequest("updateCredits", Global.getID(), Global.getToken(), -100);
            string consumer = Global.NetworkRequest(decrease);
            foreach (string c in cards)
            {
                Debug.Log(c);
                AddCardRequest newcard = new AddCardRequest(Global.getID(), Global.getToken(), c, "addCard");
                string added = Global.NetworkRequest(newcard);
            }
            for (int a = 0; a < cards.Length; a++)
            {
                cards[a] = cards[a].Replace(" ", String.Empty);
            }
            //sets up card screen
            lobbybutton.interactable = false;
            card1.interactable = true;
            card2.interactable = true;
            card3.interactable = true;
            card4.interactable = true;
            card5.interactable = true;
            card1image.gameObject.SetActive(false);
            card1image.sprite = Resources.Load<Sprite>("images/" + cards[0]);
            card1imageback.gameObject.SetActive(true);
            card2image.gameObject.SetActive(false);
            card2image.sprite = Resources.Load<Sprite>("images/" + cards[1]);
            card2imageback.gameObject.SetActive(true);
            card3image.gameObject.SetActive(false);
            card3image.sprite = Resources.Load<Sprite>("images/" + cards[2]);
            card3imageback.gameObject.SetActive(true);
            card4image.gameObject.SetActive(false);
            card4image.sprite = Resources.Load<Sprite>("images/" + cards[3]);
            card4imageback.gameObject.SetActive(true);
            card5image.gameObject.SetActive(false);
            card5image.sprite = Resources.Load<Sprite>("images/" + cards[4]);
            card5imageback.gameObject.SetActive(true);
            confirm.gameObject.SetActive(false);

            CardsPanel.gameObject.SetActive(true);
        }
        else
        {
            ErrorPanel.gameObject.SetActive(true);
        }
    }

    public void clickedButton() // shows confirm button
    {
        clicked++;
        if (clicked == 5)
        {
            confirm.gameObject.SetActive(true);
        }
    }

    //TODO: send pack request, verify credit amount, receive cards in comma delimited list, send request to add card for each card, show cards on UI

}
