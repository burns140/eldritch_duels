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
    public GameObject eloText;

    class UpdateElo {
        public string token, id, cmd, status;
        public int enemyElo;

        public UpdateElo(int enemyElo) {
            this.cmd = "updateElo";
            this.token = Global.getToken();
            this.id = Global.getID();
            this.status = "ERROR";
            
            this.enemyElo = enemyElo;
        }

        public void win() {
            this.status = "win";
        }

        public void lose() {
            this.status = "lose";
        }
    }

    class UpdateStats {
        public string token, id, cmd, type;

        public UpdateStats(string type) {
            this.cmd = "incrementStat";
            this.token = Global.getToken();
            this.id = Global.getID();

            this.type = type;
        }
    }

    class LossRequest {
        public string token, id, cmd;
        public int surrender;

        public LossRequest(int surrender, string cmd) {
            this.surrender = surrender;
            token = Global.getToken();
            id = Global.getID();
            this.cmd = cmd;
        }
    }

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
        
        if (winString == "addLoss") {
            LossRequest lreq = new LossRequest(Global.surrender, "addLoss");
            Global.NetworkRequest(lreq);
        } else {
            Request req = new Request(Global.getID(), Global.getToken(), winString);
            Global.NetworkRequest(req);
        }

        Global.surrender = 0;
        

        XPRequest xpReq = new XPRequest(Global.getID(), Global.getToken(), "addXP", xpAmount);
        Global.NetworkRequest(xpReq);


        // TODO: SET THIS TO BE A REAL VALUE THAT IS COUNTED DURING THE DUEL
        CardsPlayedRequest cardReq = new CardsPlayedRequest(Global.getID(), Global.getToken(), "addCardsPlayed", Global.cardsPlayedThisGame);
        Global.NetworkRequest(cardReq);
        Global.cardsPlayedThisGame = 0;


        // Stephen uncommented this so recomment it if need be
        Global.addCredits(myCred);
        creditsValue.GetComponent<Text>().text = ""+cred;
        
        // Elo update
        string resp;

        if (Global.matchType == MatchType.COMPETITIVE) {
            UpdateElo eloReq = new UpdateElo(Global.enemyElo);
            if (who == "you")
                eloReq.win();
            else
                eloReq.lose();

            /*json = JsonConvert.SerializeObject(eloReq);
            data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);

            bytes = Global.stream.Read(data, 0, data.Length);
            resp = System.Text.Encoding.ASCII.GetString(data, 0, bytes);*/

            resp = Global.NetworkRequest(eloReq);

            eloText.GetComponent<Text>().text = resp;
        }

        // Stats update
        string vicStatus = who == "you" ? "Wins" : "Losses";
        UpdateStats statsReq;
        switch (Global.matchType) {
            case MatchType.AI:
                statsReq = new UpdateStats("ai" + vicStatus);
                break;

            case MatchType.CASUAL:
                statsReq = new UpdateStats("casual" + vicStatus);
                break;

            case MatchType.COMPETITIVE:
                statsReq = new UpdateStats("competitive" + vicStatus);
                break;

            default:
                Debug.Log("Critical error: invalid match type (" + Global.matchType + ")");
                statsReq = null;
                Application.Quit();
                break;
        }
        Debug.Log(statsReq);
        resp = Global.NetworkRequest(statsReq);
        /*json = JsonConvert.SerializeObject(statsReq);
        data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);

        bytes = Global.stream.Read(data, 0, data.Length);
        resp = System.Text.Encoding.ASCII.GetString(data, 0, bytes);*/

        if (resp != "done")
            Debug.Log("Update stats response: " + resp);
        else
            Debug.Log("stats updated");


        // clear match variables
        Global.matchType = MatchType.UNSET;
        Global.enemyElo = 0;
        Global.DuelMyTurn = false;
        Global.matchID = null;
    }

    public void goToLobby(){ // go to the lobby scene
        SceneManager.LoadScene("Lobby");
    }
}
