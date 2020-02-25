using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace eldritch.cards
{
    public class DeckMenuUI : MonoBehaviour
    {
        private void Start()
        {
#if DEBUG
            testDeckUI();
#endif
            Debug.Log("Number of Decks: " + Global.userDecks.Count);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void NewDeck()
        {
            Global.selectedDeck = null;
            LoadScene("DeckBuilder");
        }

        public void EditDeck(Deck d)
        {
            Global.selectedDeck = d.CardsInDeck;
            LoadScene("DeckBuilder");
        }

        private void testDeckUI()
        {
            if (Global.userCards.Count == 0)
                Global.InitUserCards("0-20,1-25");
            Global.InitNewPlayer();
        }
    }
}
