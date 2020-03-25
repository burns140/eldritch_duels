using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace eldritch.cards
{
    public class alldeckretrieval
    {
        public string id;
        public string token;
        public string cmd;
        public bool shared;

        public alldeckretrieval(string cmd, string id, string token, bool shared)
        {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
            this.shared = shared;
        }
    }
    public enum DeckType
    {
        SHARED,
        USER
    }

    public class DeckMenuUI : MonoBehaviour
    {
        int deckPage = 0;
        int maxPage = 0;
        public GameObject previewPanel;
        public DeckType deckType = DeckType.USER;
        public InputField emailInput;
        private Deck toShare;
        private void Start()
        {
            if(deckType == DeckType.USER)
            {
                SetUpUserDecks();
            }else if(deckType == DeckType.SHARED)
            {
                SetupSharedDecks();
            }
            
            Debug.Log("Number of Decks: " + Global.userDecks.Count);
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
        private void SetupSharedDecks()
        {
            alldeckretrieval saved = new alldeckretrieval("getAllDecks", Global.getID(), Global.getToken(), true);
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
                if (!Global.ContainsSharedDeck(deckName) && !deckName.Equals("no decks"))
                {
                    string[] cards = Global.GetDeckByNameFromServer(temp[i], true);
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
                    Global.AddSharedDeck(d);
                }
            }
        }



        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void PageLeft()
        {
            deckPage--;
            if(deckPage < 0)
            {
                deckPage = maxPage;
            }
        }
        public void PageRight()
        {
            deckPage++;
            if(deckPage > maxPage)
            {
                deckPage = 0;
            }
        }

        private void updateUI()
        {
            maxPage = Global.userDecks.Count == 0? 0 : Global.userDecks.Count%21 == 0? Global.userDecks.Count/21 -1 : Global.userDecks.Count/21;
            
            int pos = deckPage * 21;
            
            foreach(Transform d in previewPanel.transform)
            {
                if(deckType == DeckType.USER){
                    if (pos < Global.userDecks.Count)
                    {
                        Debug.Log(pos);
                        Deck dd = Global.userDecks[pos];
                        d.gameObject.GetComponent<DeckMenuController>().PreviewDeck = dd;
                        d.gameObject.SetActive(true);
                    }
                    else
                    {
                        d.gameObject.SetActive(false);
                    }
                    pos++;
                }else{
                    if (pos < Global.sharedDecks.Count)
                    {
                        Debug.Log(pos);
                        Deck dd = Global.sharedDecks[pos];
                        d.gameObject.GetComponent<DeckMenuController>().PreviewDeck = dd;
                        d.gameObject.SetActive(true);
                    }
                    else
                    {
                        d.gameObject.SetActive(false);
                    }
                    pos++;
                }
            }
        }

        public void NewDeck()
        {
            Global.selectedDeck = null;
            LoadScene("DeckBuilder");
        }

        public void EditDeck(Deck d)
        {
            Global.selectedDeck = d;
            LoadScene("DeckBuilder");
        }

        public void DeleteDeck(Deck d)
        {
            if(deckType == DeckType.USER){
                Global.RemoveDeck(d.DeckName, false);
            }else{
                Global.RemoveDeck(d.DeckName, true);
            }
            updateUI();
        }

        private void testDeckUI()
        {
            if (Global.userCards.Count == 0)
                Global.InitUserCards("0-20,1-25",0);
            Global.InitNewPlayer();
        }

        public void CopyDeck(Deck d)
        {
            Global.CopySharedDeck(d);
        }

        public void ShareDeck(Deck d)
        {
            this.toShare = d;
        }
        public void ConfirmShare()
        {
            if (emailInput != null && emailInput.text != null && emailInput.text != "")
            {
                Global.ShareDeck(toShare.DeckName, emailInput.text);
            }
            else
            {
                Debug.Log("Email empty");
            }
        }
    }
}
