using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace eldritch.cards
{
    public class DeckMenuUI : MonoBehaviour
    {
        int deckPage = 0;
        int maxPage = 0;
        public GameObject previewPanel;
        private void Start()
        {
#if DEBUG
            testDeckUI();
#endif
            Debug.Log("Number of Decks: " + Global.userDecks.Count);
            updateUI();
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void PageLeft()
        {
            deckPage--;
            if(deckPage < 0)
            {
                deckPage = maxPage;
            }
        }
        public void PageRight()
        {
            deckPage++;
            if(deckPage > maxPage)
            {
                deckPage = 0;
            }
        }

        private void updateUI()
        {
            maxPage = Global.userDecks.Count == 0? 0 : Global.userDecks.Count%21 == 0? Global.userDecks.Count/21 -1 : Global.userDecks.Count/21;
            
            int pos = deckPage * 21;
            
            foreach(Transform d in previewPanel.transform)
            {
                if (pos < Global.userDecks.Count)
                {
                    Debug.Log(pos);
                    Deck dd = Global.userDecks[pos];
                    d.gameObject.GetComponent<DeckMenuController>().PreviewDeck = dd;
                    d.gameObject.SetActive(true);
                }
                else
                {
                    d.gameObject.SetActive(false);
                }
                pos++;
            }
        }

        public void NewDeck()
        {
            Global.selectedDeck = null;
            LoadScene("DeckBuilder");
        }

        public void EditDeck(Deck d)
        {
            Global.selectedDeck = d;
            LoadScene("DeckBuilder");
        }

        public void DeleteDeck(Deck d)
        {
            Global.RemoveDeck(d.DeckName);
            updateUI();
        }

        private void testDeckUI()
        {
            if (Global.userCards.Count == 0)
                Global.InitUserCards("0-20,1-25");
            Global.InitNewPlayer();
        }
    }
}
