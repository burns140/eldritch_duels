using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eldritch.cards {
public class CardSelector : MonoBehaviour
    {
        public Card _currentCard;
        public CardCrafter crafter;

        public void Select()
        {
            crafter.Select(this._currentCard);
        }

    }
}
