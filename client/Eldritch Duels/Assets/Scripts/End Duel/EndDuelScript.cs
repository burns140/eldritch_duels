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
    void Awake(){
        string who = PlayerPrefs.GetString(WON_PREF_KEY);
        int cred = PlayerPrefs.GetInt(CREDIT_PREF_KEY);
        if(who == "you"){
            wonText.GetComponent<Text>().text = "YOU WON !!";
        }
        else{
            wonText.GetComponent<Text>().text = "YOU LOST";
        }
        //Global.addCredits();
        creditsValue.GetComponent<Text>().text = ""+cred;
    }

    public void goToLobby(){ // go to the lobby scene
        SceneManager.LoadScene("Lobby");
    }
}
