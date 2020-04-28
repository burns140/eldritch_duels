using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using eldritch;
using eldritch.cards;

public class DeckHolder : MonoBehaviour
{
    public Text deckName;
    private Deck deck;
    public Deck Deck{
        get{return this.deck;}
        set{
            this.deck = value;
            this.gameObject.GetComponent<Image>().material = this.deck.CardsInDeck[0].c.CardImage;
            this.deckName.text = value.DeckName;
            
        }
    }

    public void SelectDeck(){
        Global.selectedDeck = this.deck;
    }
}
