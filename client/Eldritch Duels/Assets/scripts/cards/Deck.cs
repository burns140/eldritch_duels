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

        private List<CardContainer> deck;
        public List<CardContainer> CardsInDeck
        {
            get { return this.deck; }
            set { this.deck = value; }
        }

        public int DeckSize
        {
            get { return this.deck.Count; }
        }

    }
}
