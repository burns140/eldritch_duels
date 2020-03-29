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
    public GameObject wonText; // who won text in UI
    public GameObject creditsValue; // value of credits earned text in UI
    public Button goToLobbyButton; // button to go to lobby
    void Start(){
        int baseCredit = Global.numTurns * 5 /2 + 50;
        int myCred = baseCredit;
        string who = PlayerPrefs.GetString(WON_PREF_KEY);
        if(who == "you"){
            wonText.GetComponent<Text>().text = "YOU WON !!";
        }
        else{
            wonText.GetComponent<Text>().text = "YOU LOST";
            myCred /=2;
        }
        Global.addCredits(myCred);
        creditsValue.GetComponent<Text>().text = myCred + "";
    }

    public void goToLobby(){ // go to the lobby scene
        SceneManager.LoadScene("Lobby");
    }
}
