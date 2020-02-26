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
            Resources.Load<GameObject>("ContentManager").GetComponent<ContentLibrary>().AddCard(card);
        }

        public static Card GetCard(int cardID)
        {
            return Resources.Load<GameObject>("ContentManager").GetComponent<ContentLibrary>().GetCard(cardID);
        }
        public static Card GetCard(string cardName)
        {
            return Resources.Load<GameObject>("ContentManager").GetComponent<ContentLibrary>().GetCard(cardName);
        }
        public static List<Card> GetAllCards()
        {
            return Resources.Load<GameObject>("ContentManager").GetComponent<ContentLibrary>().GetAllCards();
        }
        public static void RemoveCard(string cardName)
        {
            Resources.Load<GameObject>("ContentManager").GetComponent<ContentLibrary>().RemoveCard(cardName);
        }
        public static int GetNextID()
        {
            return Resources.Load<GameObject>("ContentManager").GetComponent<ContentLibrary>().GetNextID();
        }
        #endregion

    }

    
}
