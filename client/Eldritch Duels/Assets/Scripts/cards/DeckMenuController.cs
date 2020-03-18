using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace eldritch.cards {
    public class DeckMenuController : MonoBehaviour
    {
        public GameObject Controller;
        public GameObject nameText;
        private Deck deck;
        public Deck PreviewDeck
        {
            get { return this.deck; }
            set { this.deck = value; updatePreview(); }
        }

        private void updatePreview()
        {
            if(this.deck.CardsInDeck.Count > 0)
                this.GetComponent<UnityEngine.UI.Image>().material = this.deck.CardsInDeck[0].c.CardImage;
            nameText.GetComponent<UnityEngine.UI.Text>().text = this.deck.DeckName;
        }

        public void ShareDeck(){
            Controller.GetComponent<DeckMenuUI>().ShareDeck(this.deck);
        }

        public void DeleteDeck()
        {
            Controller.GetComponent<DeckMenuUI>().DeleteDeck(this.deck);
        }
        public void EditDeck()
        {
            Global.selectedDeck = this.deck;
            SceneManager.LoadScene("DeckBuilder");

        }
        public void CopyDeck()
        {
            Global.CopySharedDeck(this.deck);
        }
    }
}
