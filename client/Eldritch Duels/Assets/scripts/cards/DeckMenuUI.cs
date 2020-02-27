using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace eldritch.cards
{
    public class alldeckretrieval
    {
        public string id;
        public string token;
        public string cmd;

        public alldeckretrieval(string cmd, string id, string token)
        {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    public class DeckMenuUI : MonoBehaviour
    {
        int deckPage = 0;
        int maxPage = 0;
        public GameObject previewPanel;
        private void Start()
        {

            alldeckretrieval saved = new alldeckretrieval("getAllDecks", Global.getID(), Global.getToken());
            string json = JsonConvert.SerializeObject(saved);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);
            data = new Byte[256];
            string responseData = string.Empty;
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            string[] temp = responseData.Split(',');
            Debug.Log("Getting deck cards");
            for(int i = 0; i < temp.Length; i++)
            {
                string deckName = temp[i];
                if (!Global.ContainsDeck(deckName) && !deckName.Equals("no decks"))
                {
                    string[] cards = Global.GetDeckByNameFromServer(temp[i]);
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
            Thread.Sleep(2500);
            Debug.Log("Number of Decks: " + Global.userDecks.Count);
            updateUI();
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
            Global.RemoveDeck(d.DeckName);
            updateUI();
        }

        private void testDeckUI()
        {
            if (Global.userCards.Count == 0)
                Global.InitUserCards("0-20,1-25",0);
            Global.InitNewPlayer();
        }
    }
}
