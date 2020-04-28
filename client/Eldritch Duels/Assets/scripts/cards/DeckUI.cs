using System;
using System.Collections.Generic;
using System.Linq;
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
        public string id;
        public string token;
        public string cmd;
        public string name;
        public string[] deck;
        public bool shared;

        public deckupload(string cmd, string id, string token, string[] deck, string name, bool shared)
        {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
            this.deck = deck;
            this.name = name;
            this.shared = shared;
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

            if (Global.selectedDeck != null)
            {
                DeckNameInput.GetComponent<UnityEngine.UI.InputField>().text = Global.selectedDeck.DeckName;
                if(Global.selectedDeck.CardsInDeck.Count == 0)
                {
                    Debug.Log(Global.GetDeckByNameFromServer(Global.selectedDeck.DeckName, false).Length);
                }
            }
            if (Global.selectedDeck == null)
                Global.selectedDeck = new Deck();


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
            //get UI panels
            GameObject col = GameObject.Find("Collection");
            GameObject deck = GameObject.Find("Deck");
            int pos = collectPage * 8;
            //scan each collection ui placeholders
            foreach(Transform t in col.transform)
            {
                t.gameObject.SetActive(false);
                //if card exists at position
                if(pos < inCollection.Count)
                {
                    //add card to ui with amount available
                    t.gameObject.SetActive(true);
                    t.gameObject.GetComponent<UnityEngine.UI.Image>().material = inCollection[pos].c.CardImage;
                    t.gameObject.GetComponent<DeckControl>().setCard(inCollection[pos].c);
                    t.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Add (" + inCollection[pos].count + ")";
                }
                pos++;
            }
            pos = deckPage * 8;
            //scan each placeholder in deck panel
            foreach (Transform t in deck.transform)
            {
                t.gameObject.SetActive(false);
                //check is card in deck exists at position
                if (pos < inDeck.Count)
                {
                    //add card to ui
                    t.gameObject.SetActive(true);
                    t.gameObject.GetComponent<UnityEngine.UI.Image>().material = inDeck[pos].c.CardImage;
                    t.gameObject.GetComponent<DeckControl>().setCard(inDeck[pos].c);
                    t.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Remove (" + inDeck[pos].count + ")";
                }
                pos++;
            }

            
        }
        //scroll panel controls
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
        //add card to deck being added
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
                        //check id deck already contains same card
                        if(inDeck[j].c.CardID == c.CardID)
                        {
                            if(inDeck[j].count >= Constants.MAX_CARD_ALLOWED && inDeck[j].c.SpellRarity == CardRarity.LEGENDARY)
                            {
                                return;
                            }
                            //increament amount in deck
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
                    //new card being added so add new container
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

        //remove card from deck being edited
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
            //find input elements
            if (GameObject.Find("Deck Name Text").GetComponent<UnityEngine.UI.Text>().text.Trim().Equals(""))
            {
                UIError.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Deck Name Cannot Be Empty";
                UIError.SetActive(true);
                return;
            }
            if (GameObject.Find("Deck Name Text").GetComponent<UnityEngine.UI.Text>().text.Trim().Equals("no decks"))
            {
                UIError.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Invalid Deck Name";
                UIError.SetActive(true);
                return;
            }
            if (deckSize < Constants.MIN_DECK_SIZE)
            {
                UIError.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Not Enough Cards In Deck!";
                UIError.SetActive(true);
                return;
            }
            //check if name taken
            Deck newDeck = new Deck();
            for (int i = 0; i < Global.userDecks.Count; i++)
            {
                if (GameObject.Find("Deck Name Text").GetComponent<UnityEngine.UI.Text>().text.Equals(Global.userDecks[i].DeckName))
                {
                    Deck updatedDeck = Global.userDecks[i];
                    updatedDeck.CardsInDeck = inDeck;
                    Global.userDecks[i] = updatedDeck;
                    //sync with server
                    try
                    {
                        deckupload saved = new deckupload("saveDeck", Global.getID(), Global.getToken(), deckToString(updatedDeck), updatedDeck.DeckName, false);
                        string json = JsonConvert.SerializeObject(saved);
                        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                        Global.stream.Write(data, 0, data.Length);
                        data = new Byte[256];
                        string responseData = string.Empty;
                        Int32 bytes = Global.stream.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                        Debug.Log(responseData);
                    }
                    catch (Exception)
                    {

                    }
                    SceneManager.LoadScene("Decks");
                    return;
                }

            }

            //sets new deck values
            newDeck.DeckName = GameObject.Find("Deck Name Text").gameObject.GetComponent<UnityEngine.UI.Text>().text;
            newDeck.CardsInDeck = inDeck;
            Debug.Log(newDeck);
            //add to local deck list
            Global.AddDeck(newDeck);
            //sync with server
            try
            {
                deckupload saved = new deckupload("saveDeck", Global.getID(), Global.getToken(), deckToString(newDeck), newDeck.DeckName, false);
                string json = JsonConvert.SerializeObject(saved);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                Global.stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responseData = string.Empty;
                Int32 bytes = Global.stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            }
            catch (Exception)
            {

            }

            SceneManager.LoadScene("Decks");
        }

        //takes a deck and converts it to a string[] representation
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

       


    }
}
