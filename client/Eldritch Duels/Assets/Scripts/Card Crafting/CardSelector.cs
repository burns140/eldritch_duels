using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace eldritch.cards {
public class CardSelector : MonoBehaviour
    {
        private Card _currentCard;
        public Card CurrentCard
        {
            get { return this._currentCard; }
            set { this._currentCard = value; this.gameObject.GetComponent<Image>().material = _currentCard.CardImage; }
        }
        public CardCrafter crafter;

        public void Select()
        {
            crafter.Select(this._currentCard);
        }

    }
}
