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

        private void testDeckUI()
        {
            Global.InitNewPlayer();
        }
    }
}
