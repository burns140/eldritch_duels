//#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using eldritch.cards;
using System.IO;
using Newtonsoft.Json;

namespace eldritch {
    public static class Constants
    {
        public const int MAX_DECK_SIZE = 32;
        public const int MIN_DECK_SIZE = 32;
        public const int MAX_CARD_ALLOWED = 4;
    }
    public class getCollection
    {
        public string id;
        public string token;
        public string cmd;

        public getCollection(string cmd, string id, string token)
        {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }
    public class deleteDeck
    {
        public string id;
        public string token;
        public string cmd;
        public string name;

        public deleteDeck(string cmd, string id, string token, string name)
        {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
            this.name = name;
        }
    }
    public class AddCardRequest
    {
        public string id;
        public string token;
        public string cardid;
        public string cmd;

        public AddCardRequest(string id, string token, string cardid, string cmd)
        {
            this.id = id;
            this.token = token;
            this.cardid = cardid;
            this.cmd = cmd;
        }
    }

    public static class Global
    {
    	//Global variables, can be called by any class and script via Global.(variable) as long as "using eldritch" is in the imports
        public static string username = "";
        public static string enemyUsername = null;
        public static string matchID = null;
        public static int userID = 0;
        public static List<Card> userCards = new List<Card>();
        public static int usercredits = 0;
        public static Deck selectedDeck = new Deck();
        public static List<Deck> userDecks = new List<Deck>();
        public static List<Deck> sharedDecks = new List<Deck>();
        public static TcpClient client;
        public static NetworkStream stream;
        public static string tokenfile = "";
        public static int avatar = 0;
        public static string bio = "";
        public static bool inQueue = false;
        private static string hostIP = "66.253.158.241";

        public static void SetUpConnection()
        {
            try
            {
                //Connects to server and sets global variables, change localhost and port if connecting remotely.
                Global.client = new TcpClient("localhost", 8000);
                Global.client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Global.stream = Global.client.GetStream();
                Debug.Log("Connected to: " + "localhost");
            } catch(Exception e)
            {
                try
                {
                    //Connects to server and sets global variables, change localhost and port if connecting remotely.
                    Global.client = new TcpClient(hostIP, 8000);
                    Global.client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    Global.stream = Global.client.GetStream();
                    Debug.Log("Connected to: " + hostIP);
                }
                catch (Exception e2)
                {
                    Debug.Log("Error connecting to server.");
                    Debug.Log(e.Message);
                    Debug.Log(e2.Message);
                    Application.Quit();
                }
            }
        }

        public static string[] GetDeckByNameFromServer(string deckName)
        {
            Debug.Log(deckName);
            if(deckName.Equals("no decks"))
            {
                return null;
            }
            try
            {
                string[] useless = new string[1];
                deckupload saved = new deckupload("getOneDeck", Global.getID(), Global.getToken(), useless, deckName);
                string json = JsonConvert.SerializeObject(saved);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                Global.stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responseData = string.Empty;
                Int32 bytes = Global.stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Debug.Log("Deck resp: " + responseData);
                string[] temp = responseData.Split(',');
                return temp;
            }catch (Exception)
            {

            }
            return null;
        }


        public static string getToken()
        {
            try
            {
                // Read from the temp file.
                StreamReader myReader = File.OpenText(tokenfile);
                string token = myReader.ReadLine();
                myReader.Close();
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading token file: " + ex.Message);
                return String.Empty;
            }
        }

        public static string getID()
        {
            try
            {
                // Read from the temp file.
                StreamReader myReader = File.OpenText(tokenfile);
                myReader.ReadLine();
                string id = myReader.ReadLine();
                myReader.Close();
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading token file: " + ex.Message);
                return String.Empty;
            }
        }

        //converts a tring into a list of cards
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
        private static List<Card> StringToCardsByName(string toParse)
        {
            List<Card> cards = new List<Card>();
            string[] pairs = toParse.Split(',');
            foreach (string s in pairs)
            {
                string[] tuple = s.Split('-');

                
                int amount = int.Parse(tuple[1]);
                GameObject g = GameObject.Find("ContentManager");
                if (g != null)
                {
                    Card c = g.GetComponent<ContentLibrary>().GetCard(tuple[0]);
                    if (c != null && amount > 0)
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
        public static void InitUserCards(string userCards, int mode)
        {
            if (mode == 0)
                Global.userCards = StringToCards(userCards);
            else if (mode == 1)
                Global.userCards = StringToCardsByName(userCards);
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

            try
            {
                AddCardRequest arc = new AddCardRequest(Global.getID(), Global.getToken(), cardName, "removeCardFromCollection");
                string json = JsonConvert.SerializeObject(arc);
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
                    Debug.Log("Adding card: " + cardName);
                    try
                    {
                        AddCardRequest arc = new AddCardRequest(Global.getID(), Global.getToken(), cardName, "addCardToCollection");
                        string json = JsonConvert.SerializeObject(arc);
                        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                        Global.stream.Write(data, 0, data.Length);
                        data = new Byte[256];
                        string responseData = string.Empty;
                        Int32 bytes = Global.stream.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    }
                    catch (Exception)
                    {
                        Debug.Log("error");
                    }
                    return;
                }
            }
            Card newCard = Library.GetCard(cardName);
            newCard.CopiesOwned = 1;
            userCards.Add(newCard);

            Debug.Log("Adding card: " + cardName);
            try
            {
                AddCardRequest arc = new AddCardRequest(Global.getID(), Global.getToken(), cardName, "addCardToCollection");
                string json = JsonConvert.SerializeObject(arc);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                Global.stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responseData = string.Empty;
                Int32 bytes = Global.stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            }
            catch (Exception)
            {
                Debug.Log("error");
            }
        }
        public static void InitNewPlayer()
        {
            if (userCards.Count == 0)
            {
                InitUserCards("0-32",0);
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
            for(int i = 0; i < userDecks.Count; i++)
            {
                if (d.DeckName.Equals(userDecks[i].DeckName))
                {
                    userDecks[i] = d;
                    return;
                }
            }
            userDecks.Add(d);
        }
        public static bool ContainsDeck(string name)
        {
            foreach(Deck d in userDecks)
            {
                if (d.DeckName.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }
        public static void RemoveDeck(string deckName)
        {
            for(int i = 0; i < userDecks.Count; i++)
            {
                if (userDecks[i].DeckName.Equals(deckName))
                {
                    try
                    {
                        deleteDeck saved = new deleteDeck("deleteDeck", Global.getID(), Global.getToken(), deckName);
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
                    userDecks.RemoveAt(i);
                }
            }
        }

        public static string GetCollection()
        {
            try
            {
                getCollection saved = new getCollection("getCollection", Global.getID(), Global.getToken());
                string json = JsonConvert.SerializeObject(saved);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                Global.stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responseData = string.Empty;
                Int32 bytes = Global.stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Debug.Log(responseData);
                return responseData;
            }
            catch (Exception)
            {

            }
            return null;
        }
        public static bool CopySharedDeck(Deck newDeck)
        {
            //find a shared deck
            Deck toCopy = null;
            foreach(Deck d in sharedDecks)
            {
                if (d.DeckName.Equals(newDeck.DeckName))
                {
                    toCopy = d;
                    break;
                }
            }
            if (toCopy == null)
                return false;

            //see if user has the cards to copy
            foreach(CardContainer cc in toCopy.CardsInDeck)
            {
                if(cc.c.CopiesOwned < cc.count)
                {
                    return false;
                }
            }

            //duplicate to userdecks
            userDecks.Add(toCopy);

            //sync with server
            try
            {
                deckupload saved = new deckupload("saveDeck", Global.getID(), Global.getToken(), DeckToString(newDeck), newDeck.DeckName);
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
                return false;
            }



            return true;
        }
        private static string[] DeckToString(Deck d)
        {
            List<CardContainer> cards = d.CardsInDeck;
            string[] temp = new string[cards.Count];
            for (int i = 0; i < cards.Count; i++)
            {
                temp[i] = cards.ElementAt(i).c.CardName + "-" + cards.ElementAt(i).count;
                Debug.Log(temp[i]);
            }
            return temp;
        }
        public static Card GetUserCard(string cardName){
            foreach(Card c in userCards){
                if(c.CardName.Equals(cardName)){
                    return c;
                }
            }

            return null;
        }

        public static bool EnoughToCraft(CraftingRecipe cr){
            foreach(Deck d in userDecks){
                if(cr.BaseCard.Equals(cr.FodderCard)){
                    if(d.AmountInDeck(cr.BaseCard) > (GetUserCard(cr.BaseCard).CopiesOwned + 2)){
                        return false;
                    }
                }else{
                    if(d.AmountInDeck(cr.BaseCard) >= GetUserCard(cr.BaseCard).CopiesOwned || 
                    d.AmountInDeck(cr.FodderCard) >= GetUserCard(cr.FodderCard).CopiesOwned){
                        return false;
                    }
                }
            }
            if(cr.BaseCard.Equals(cr.FodderCard) && GetUserCard(cr.BaseCard).CopiesOwned < 2){
                Debug.Log("Copies owned: " + GetUserCard(cr.BaseCard).CopiesOwned);
                return false;
            }
            if(GetUserCard(cr.BaseCard).CopiesOwned == 0 || GetUserCard(cr.FodderCard).CopiesOwned == 0){
                return false;
            }

            return true;
        }

    }

    

}
