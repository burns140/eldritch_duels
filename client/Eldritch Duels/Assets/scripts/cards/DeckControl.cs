using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eldritch.cards {
    public class DeckControl : MonoBehaviour
    {
        private Card _card;
        public GameObject Enlarger;
        public GameObject root;
        public void setCard(Card c)
        {
            this._card = c;
        }

        public void AddDeck()
        {
            if(this._card != null)
            {
                root.GetComponent<DeckUI>().AddCard(this._card);
            }
        }
        public void RemoveDeck()
        {
            if (this._card != null)
            {
                root.GetComponent<DeckUI>().RemoveCard(this._card);
            }
        }
        public void Enlarge()
        {
            if ( Enlarger != null && _card != null)
            {
                Enlarger.SetActive(true);
                Enlarger.GetComponent<UnityEngine.UI.Image>().material = _card.CardImage;
                
            }
        }
    }
}
