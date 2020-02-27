﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace eldritch.cards {
    public struct CardContainer
    {
        public Card c;
        public int count;
    }

    public class deckupload
    {
        private string id;
        private string cmd;
        private string name;
        private string[] deck;

        public deckupload(string cmd, string id, string[] deck, string name)
        {
            this.id = id;
            this.cmd = cmd;
            this.deck = deck;
            this.name = name;
        }
    }

    public class DeckUI : MonoBehaviour
    {
        private int collectPage = 0;
        private int maxCollectPage = 0;
        private int deckPage = 0;
        private int maxDeckPage = 0;
        private int deckSize = 0;
        public Material testMat;
        public GameObject UIError;
        public GameObject DeckNameInput;
        List<CardContainer> inDeck = new List<CardContainer>();
        List<CardContainer> inCollection = new List<CardContainer>();
        // Update is called once per frame
        private void Start()
        {
            
           if(Global.selectedDeck != null)
                DeckNameInput.GetComponent<UnityEngine.UI.InputField>().text = Global.selectedDeck.DeckName;
            if (Global.selectedDeck == null)
                Global.selectedDeck = new Deck();
#if DEBUG
            testDeck();
#endif
        
            //get deck
            
            foreach (Card c in Global.userCards)
            {
                CardContainer cc;
                cc.c = c;
                cc.count = c.CopiesOwned;
                inCollection.Add(cc);
            }
            
            foreach (CardContainer c in Global.selectedDeck.CardsInDeck)
            {
                for(int i = 0; i < c.count; i++)
                {
                    AddCard(c.c);
                }
            }
            this.maxDeckPage = inDeck.Count / 8;
            this.maxCollectPage = inCollection.Count / 8;
            

            updateUI();
            
        }
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        private void updateUI()
        {
            if (deckPage > maxDeckPage)
                deckPage = maxDeckPage;
            if (collectPage > maxCollectPage)
                collectPage = maxCollectPage;
            GameObject col = GameObject.Find("Collection");
            GameObject deck = GameObject.Find("Deck");
            int pos = collectPage * 8;
            foreach(Transform t in col.transform)
            {
                t.gameObject.SetActive(false);
                if(pos < inCollection.Count)
                {
                    t.gameObject.SetActive(true);
                    t.gameObject.GetComponent<UnityEngine.UI.Image>().material = inCollection[pos].c.CardImage;
                    t.gameObject.GetComponent<DeckControl>().setCard(inCollection[pos].c);
                    t.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Add (" + inCollection[pos].count + ")";
                }
                pos++;
            }
            pos = deckPage * 8;
            foreach (Transform t in deck.transform)
            {
                t.gameObject.SetActive(false);
                if (pos < inDeck.Count)
                {
                    t.gameObject.SetActive(true);
                    t.gameObject.GetComponent<UnityEngine.UI.Image>().material = inDeck[pos].c.CardImage;
                    t.gameObject.GetComponent<DeckControl>().setCard(inDeck[pos].c);
                    t.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Remove (" + inDeck[pos].count + ")";
                }
                pos++;
            }

            
        }
        public void CollectLeft()
        {
            collectPage--;
            if (collectPage < 0)
                collectPage = maxCollectPage;
            updateUI();
        }
        public void CollectRight()
        {
            collectPage++;
            if (collectPage > maxCollectPage)
                collectPage = 0;
            updateUI();
        }
        public void DeckLeft()
        {
            deckPage--;
            if (deckPage < 0)
                deckPage = maxDeckPage;
            updateUI();
        }
        public void DeckRight()
        {
            deckPage++;
            if (deckPage > maxDeckPage)
                deckPage = 0;
            updateUI();
        }

        public void AddCard(Card c)
        {
            if(deckSize >= Constants.MAX_DECK_SIZE)
            {
                return;
            }
            
            for(int i = 0; i < inCollection.Count; i++)
            {
                if(inCollection[i].c.CardID == c.CardID && inCollection[i].count > 0)
                {
                    for(int j = 0; j < inDeck.Count; j++)
                    {
                        if(inDeck[j].c.CardID == c.CardID)
                        {
                            CardContainer temp = inDeck[j];
                            temp.count++;
                            inDeck[j] = temp;
                            temp = inCollection[i];
                            temp.count--;
                            inCollection[i] = temp;
                            this.maxDeckPage = inDeck.Count / 8;
                            this.maxCollectPage = inCollection.Count / 8;
                            deckSize++;
                            updateUI();
                            return;
                        }
                    }
                    CardContainer co = inCollection[i];
                    co.count--;
                    inCollection[i] = co;
                    CardContainer di;
                    di.c = c;
                    di.count = 1;
                    inDeck.Add(di);
                    deckSize++;
                    this.maxDeckPage = inDeck.Count / 8;
                    this.maxCollectPage = inCollection.Count / 8;
                    updateUI();

                }
            }
        }
        public void RemoveCard(Card c)
        {
            
            for (int i = 0; i < inCollection.Count; i++)
            {
                if (inCollection[i].c.CardID == c.CardID)
                {
                    for (int j = 0; j < inDeck.Count; j++)
                    {
                        if (inDeck[j].c.CardID == c.CardID)
                        {
                            CardContainer temp = inDeck[j];
                            temp.count--;
                            inDeck[j] = temp;
                            if(temp.count <= 0)
                            {
                                inDeck.RemoveAt(j);
                            }
                            temp = inCollection[i];
                            temp.count++;
                            inCollection[i] = temp;
                            this.maxDeckPage = inDeck.Count / 8;
                            this.maxCollectPage = inCollection.Count / 8;
                            deckSize--;
                            updateUI();
                            return;
                        }
                    }
                    

                }
            }
        }

        public void SaveDeck()
        {
            if (GameObject.Find("Deck Name Text").GetComponent<UnityEngine.UI.Text>().text.Trim().Equals(""))
            {
                UIError.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Deck Name Cannot Be Empty";
                UIError.SetActive(true);
                return;
            }
            if(deckSize != Constants.MIN_DECK_SIZE)
            {
                UIError.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Not Enough Cards In Deck!";
                UIError.SetActive(true);
                return;
            }
            //check if name taken
            for(int i = 0; i < Global.userDecks.Count; i++)
            {
                if(GameObject.Find("Deck Name Text").GetComponent<UnityEngine.UI.Text>().text.Equals(Global.userDecks[i].DeckName))
                {
                    Deck updatedDeck = Global.userDecks[i];
                    updatedDeck.CardsInDeck = inDeck;
                    Global.userDecks[i] = updatedDeck;
                    SceneManager.LoadScene("Decks");
                    return;
                }

            }

            Deck newDeck = new Deck();
            newDeck.DeckName = GameObject.Find("Deck Name Text").gameObject.GetComponent<UnityEngine.UI.Text>().text;
            newDeck.CardsInDeck = inDeck;
            Global.AddDeck(newDeck);
            try
            {
                deckupload saved = new deckupload("saveDeck", Global.userID.ToString(), deckToString(newDeck), newDeck.DeckName);
                string json = JsonConvert.SerializeObject(saved);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                Global.stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent");
                data = new Byte[256];
                string responseData = string.Empty;
                Int32 bytes = Global.stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                Thread.Sleep(2500);
            }
            catch (Exception)
            {

            }

            SceneManager.LoadScene("Decks");
        }

        string[] deckToString(Deck d)
        {
            List<CardContainer> cards = d.CardsInDeck;
            string[] temp = new string[cards.Count];
            for(int i = 0; i < cards.Count; i++)
            {
                temp[i] = cards.ElementAt(i).c.CardName + "-" + cards.ElementAt(i).count;
                Debug.Log(temp[i]);
            }
            return temp;
        }

        void testDeck()
        {
            return;
            for(int i = 0; i < 10; i++)
            {
                Card c = new Card(i, "test card " + i);
                c.AttackPower = 100;
                c.DefencePower = 100;
                c.SpellRarity = CardRarity.LEGENDARY;
                c.CopiesOwned = i;
                c.CardImage = testMat;
                Global.userCards.Add(c);

            }
        }


    }
}
