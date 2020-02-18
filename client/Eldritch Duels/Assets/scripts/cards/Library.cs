using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace eldritch.cards
{
    [System.Serializable]
    public static class Library
    {
        #region cards
        [SerializeField]
        private static Hashtable _cards = new Hashtable();
        private static Hashtable _cardID = new Hashtable();
        public static void AddCard(Card card)
        {
            GameObject.Find("ContentManager").GetComponent<ContentLibrary>().AddCard(card);
        }

        public static Card GetCard(int cardID)
        {
            return GameObject.Find("ContentManager").GetComponent<ContentLibrary>().GetCard(cardID);
        }
        public static Card GetCard(string cardName)
        {
            return GameObject.Find("ContentManager").GetComponent<ContentLibrary>().GetCard(cardName);
        }
        public static List<Card> GetAllCards()
        {
            return GameObject.Find("ContentManager").GetComponent<ContentLibrary>().GetAllCards();
        }
        public static void RemoveCard(string cardName)
        {
            GameObject.Find("ContentManager").GetComponent<ContentLibrary>().RemoveCard(cardName);
        }
        public static int GetNextID()
        {
            return GameObject.Find("ContentManager").GetComponent<ContentLibrary>().GetNextID();
        }
        #endregion

    }

    
}
