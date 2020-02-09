using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace eldritch.cards
{
    public static class Library
    {
        #region cards
        private static Hashtable _cards = new Hashtable();
        private static Hashtable _cardID = new Hashtable();
        public static void AddCard(Card card)
        {
            if (_cards.ContainsKey(card.CardName))
            {
                _cards[card.CardName] = card;
                _cardID[card.CardID] = card.CardName;
            }
            else
            {
                _cards.Add(card.CardName, card);
                _cardID.Add(card.CardID, card.CardName);
            }
        }

        public static Card GetCard(int cardID)
        {
            return GetCard((string)_cardID[cardID]);
        }
        public static Card GetCard(string cardName)
        {
            return (Card)_cards[cardName];
        }
        #endregion

    }
}
