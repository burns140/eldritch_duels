using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using eldritch;
using eldritch.cards;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EndDuelScript : MonoBehaviour
{
    private const string WON_PREF_KEY = "whowon"; // PREF KEY to store who won
    private const string CREDIT_PREF_KEY = "credits"; // PREF KEY to store credits
    public GameObject wonText; // who won text in UI
    public GameObject creditsValue; // value of credits earned text in UI
    public Button goToLobbyButton; // button to go to lobby
    void Start(){
        int baseCredit = Global.numTurns * 5 /2 + 50;
        int myCred = baseCredit;
        string who = PlayerPrefs.GetString(WON_PREF_KEY);
        int cred = PlayerPrefs.GetInt(CREDIT_PREF_KEY);
        string winString = "";
        int xpAmount;
        if(who == "you"){
            wonText.GetComponent<Text>().text = "YOU WON !!";
            winString = "addWin";
            xpAmount = 100;
        } else {
            wonText.GetComponent<Text>().text = "YOU LOST";
            myCred /=2;
            winString = "addLoss";
            xpAmount = 50;
        }

        Request req = new Request(Global.getID(), Global.getToken(), winString);
        Global.NetworkRequest(req);

        XPRequest xpReq = new XPRequest(Global.getID(), Global.getToken(), "addXP", xpAmount);
        Global.NetworkRequest(xpReq);


        // TODO: SET THIS TO BE A REAL VALUE THAT IS COUNTED DURING THE DUEL
        int cardsPlayedAmount = 5;
        CardsPlayedRequest cardReq = new CardsPlayedRequest(Global.getID(), Global.getToken(), "addCardsPlayed", cardsPlayedAmount);
        Global.NetworkRequest(cardReq);


        // Stephen uncommented this so recomment it if need be
        Global.addCredits(myCred);
        creditsValue.GetComponent<Text>().text = ""+cred;

    }

    public void goToLobby(){ // go to the lobby scene
        SceneManager.LoadScene("Lobby");
    }
}
