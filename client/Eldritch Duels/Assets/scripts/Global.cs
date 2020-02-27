#define DEBUG


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using eldritch.cards;

namespace eldritch {
    public static class Constants
    {
        public const int MAX_DECK_SIZE = 32;
        public const int MIN_DECK_SIZE = 32;
    }

    public static class Global
    {
        public static string username = "";
        public static int userID = 0;
        public static List<Card> userCards = new List<Card>();
        public static int usercredits = 0;
        public static Deck selectedDeck = new Deck();
        public static List<Deck> userDecks = new List<Deck>();
        public static TcpClient client = new TcpClient("localhost", 8000);
        public static NetworkStream stream = client.GetStream();


        private static List<Card> StringToCards(string toParse)
        {
            List<Card> cards = new List<Card>();
            string[] pairs = toParse.Split(',');
            foreach (string s in pairs)
            {
                string[] tuple = s.Split('-');

                int id = int.Parse(tuple[0]);
                int amount = int.Parse(tuple[1]);
                GameObject g = GameObject.Find("ContentManager");
                if (g != null)
                {
                    Card c = g.GetComponent<ContentLibrary>().GetCard(id);
                    if (c != null)
                    {
                        c.CopiesOwned = amount;
                        cards.Add(c);
                    }
                }

            }
            return cards;

        }

        private static List<CardContainer>StringToDeck(string cards)
        {
            List<CardContainer> cardsc = new List<CardContainer>();
            string[] pairs = cards.Split(',');
            foreach (string s in pairs)
            {
                string[] tuple = s.Split('-');

                int id = int.Parse(tuple[0]);
                int amount = int.Parse(tuple[1]);
                GameObject g = GameObject.Find("ContentManager");
                if (g != null)
                {
                    Card c = g.GetComponent<ContentLibrary>().GetCard(id);
                    if (c != null)
                    {
                        CardContainer cc;
                        cc.c = c;
                        cc.count = amount;
                        cardsc.Add(cc);
                    }
                }

            }
            return cardsc;
        }

        //format of user card string: "id-#,id2-#..."
        public static void InitUserCards(string userCards)
        {
            Global.userCards = StringToCards(userCards);
        }
        //user has destroyed a card in crafting
        public static void RemoveCard(string cardName)
        {
            for( int i = 0; i < Global.userCards.Count; i++)
            {
                if (userCards[i].CardName.Equals(cardName))
                {
                    Card tmp = userCards[i];
                    tmp.CopiesOwned--;
                    userCards[i] = tmp;
                    if(tmp.CopiesOwned <= 0)
                    {
                        userCards.RemoveAt(i);
                    }
                }
            }
        }
        public static void AddCard(string cardName)
        {
            for (int i = 0; i < Global.userCards.Count; i++)
            {
                if (userCards[i].CardName.Equals(cardName))
                {
                    Card tmp = userCards[i];
                    tmp.CopiesOwned++;
                    userCards[i] = tmp;
                    return;
                }
            }
            Card newCard = Library.GetCard(cardName);
            newCard.CopiesOwned = 1;
            userCards.Add(newCard);
        }
        public static void InitNewPlayer()
        {
            if (userCards.Count == 0)
            {
                InitUserCards("0-32");
            }
            if (userDecks.Count == 0)
            {
                InitUserDecks(new string[] { "First Deck,0-32" });
            }
        }
        public static void InitUserDecks(string[] decks)
        {
            Global.userDecks = new List<Deck>();
            foreach(string s in decks)
            {
                string[] first = s.Split(new char[] { ',' },2);
                Deck d = new Deck();
                d.DeckName = first[0];
                Debug.Log(first[1]);
                d.CardsInDeck = StringToDeck(first[1]);
                Global.userDecks.Add(d);

            }
        }
        public static void AddDeck(Deck d)
        {
            userDecks.Add(d);
        }
        public static void RemoveDeck(string deckName)
        {
            for(int i = 0; i < userDecks.Count; i++)
            {
                if (userDecks[i].DeckName.Equals(deckName))
                {
                    userDecks.RemoveAt(i);
                }
            }
        }

    }
}
