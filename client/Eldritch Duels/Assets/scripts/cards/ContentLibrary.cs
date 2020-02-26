using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eldritch.cards 
{
    [System.Serializable]
    public class ContentLibrary : MonoBehaviour
    {
        
        [SerializeField]
        public List<Card> _cards = new List<Card>();
        public void AddCard(Card card)
        {
            for(int i = 0; i < _cards.Count; i++)
            {
                if(_cards[i].CardName == card.CardName)
                {
                    _cards[i] = card;
                    return;
                }
            }
            if(_cards.Count == 0)
            {
                _cards.Add(card);
            }
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].CardID > card.CardID)
                {
                    _cards.Insert(i, card);
                    return;
                }
            }
            _cards.Add(card);

        }

        public Card GetCard(int cardID)
        {
            foreach(Card c in _cards)
            {
                if(c.CardID == cardID)
                {
                    return c;
                }
            }
            return null;
        }
        public Card GetCard(string cardName)
        {
            foreach (Card c in _cards)
            {
                if (c.CardName.Equals(cardName))
                {
                    return c;
                }
            }
            return null;
        }



        public List<Card> GetAllCards()
        {
            return _cards;
        }
        public void RemoveCard(string cardName)
        {
            for(int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].CardName.Equals(cardName))
                {
                    _cards.RemoveAt(i);
                    return;
                }
            }
        }
        public int GetNextID()
        {
            if(_cards.Count == 0)
            {
                return 0;
            }
            for(int i = 0; i < _cards.Count-1; i++)
            {
                if(_cards[i].CardID - _cards[i+1].CardID > 1)
                {
                    return _cards[i].CardID + 1;
                }
            }
            return _cards[_cards.Count - 1].CardID + 1;
        }
    }
}
