using eldritch.cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace eldritch {



    public class CollectionUI : MonoBehaviour
    {
        int page = 0;
        
        public int maxCards = 12;
        public int totalPages = 0;
        public GameObject CardPanel;

        private void Awake()
        {
#if DEBUG
            testCardCollection();
#endif
        }

        void Start()
        {
            updateCardUI();
        }

        void Update()
        {
            totalPages = Global.userCards.Count / maxCards + Global.userCards.Count % maxCards == 0 ? 0 : Global.userCards.Count > maxCards? 1 : 0;
            
        }
        public void PageLeft()
        {

            page--;
            if(page < 0)
            {
                page = totalPages;
            }
            
            updateCardUI();
            
        }
        

        public void PageRight()
        {
            page++;
            if(page > totalPages)
            {
                page = 0;
            }
            updateCardUI();
        }

        private void updateCardUI()
        {

            int count = 0;
            foreach (Transform child in CardPanel.transform)
            {
                child.gameObject.SetActive(false);
                if (page * 12 + count < (page + 1) * 12 && page * 12 + count < Global.userCards.Count)
                {
                    child.gameObject.SetActive(true);
                    Card c = Global.userCards[page * 12 + count];
                    if (c != null)
                    {
                        child.gameObject.GetComponent<UnityEngine.UI.Image>().material = c.CardImage;
                    }
                }

                count++;
            }
        }

        private void testCardCollection()
        {
            Debug.Log("Running collection test...");
            if(Global.userCards.Count == 0)
            {
                //create dummy cards
                for(int i = 0; i < 15; i++)
                {
                    Card c = new Card(i, "c" + i);
                    Global.userCards.Add(c);
                }
            }
        }
    }
}
