using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eldritch.cards {
    public class Deck
    {
        private string name = "";
        public string DeckName
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private List<CardContainer> deck = new List<CardContainer>();
        public List<CardContainer> CardsInDeck
        {
            get { return this.deck; }
            set { this.deck = value; }
        }

        public int DeckSize
        {
            get { int c = 0; foreach (CardContainer cc in CardsInDeck) { c += cc.count; } return c; }
        }
        public int AmountInDeck(string cardName)
        {
            foreach(CardContainer cc in CardsInDeck)
            {
                if (cc.c.CardName.Equals(cardName))
                {
                    return cc.count;
                }
            }
            return 0;
        }

    }
}
