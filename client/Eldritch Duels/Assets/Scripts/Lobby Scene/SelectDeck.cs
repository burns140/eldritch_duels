using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using eldritch;
using eldritch.cards;
using Newtonsoft.Json;

public class SelectDeck : MonoBehaviour
{
    private int page = 0;
    private int maxpage = 0;
    public GameObject DeckPanel;
    public GameObject AIDeckPanel;
    public Button ConfirmButton;

    void Start(){
        maxpage = Global.userDecks.Count/7;        
        if(Global.userDecks.Count == 0)
            SetUpUserDecks();

        updateUI();
    }

    private void SetUpUserDecks()
    {
        alldeckretrieval saved = new alldeckretrieval("getAllDecks", Global.getID(), Global.getToken(), false);
        string json = JsonConvert.SerializeObject(saved);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        string[] temp = responseData.Split(',');
        Debug.Log("Getting deck cards");
        for (int i = 0; i < temp.Length; i++)
        {
            string deckName = temp[i];
            if (!Global.ContainsDeck(deckName) && !deckName.Equals("no decks"))
            {
                string[] cards = Global.GetDeckByNameFromServer(temp[i], false);
                Deck d = new Deck();
                d.CardsInDeck = new List<CardContainer>();
                d.DeckName = deckName;
                if (cards != null && cards.Length > 1)
                {
                    for (int j = 1; j < cards.Length; j++)
                    {
                        //add cards
                        string[] pair = cards[j].Split('-');
                        Card c = Library.GetCard(pair[0]);
                        CardContainer cc;
                        cc.c = c;
                        cc.count = int.Parse(pair[1]);
                        d.CardsInDeck.Add(cc);
                    }
                }
                Global.AddDeck(d);
            }
        }
    }

    public void pageRight(){
        page++;
        if(page > maxpage){
            page = 0;
        }
    }
    public void pageLeft(){
        page--;
        if(page < 0){
            page = maxpage;
        }
    }

    private void updateUI(){
        int pos = 7*page;
        for(int i = 0; i < 7; i++){
            DeckPanel.transform.GetChild(i).gameObject.SetActive(false);
            AIDeckPanel.transform.GetChild(i).gameObject.SetActive(false);
            if(pos < Global.userDecks.Count){
                DeckPanel.transform.GetChild(i).gameObject.name = Global.userDecks[pos].DeckName;
                DeckPanel.transform.GetChild(i).gameObject.GetComponent<DeckHolder>().Deck = Global.userDecks[pos];
                DeckPanel.transform.GetChild(i).gameObject.SetActive(true);
                AIDeckPanel.transform.GetChild(i).gameObject.name = Global.userDecks[pos].DeckName;
                AIDeckPanel.transform.GetChild(i).gameObject.GetComponent<DeckHolder>().Deck = Global.userDecks[pos];
                AIDeckPanel.transform.GetChild(i).gameObject.SetActive(true);
            }

            pos++;
        }
    }

}
