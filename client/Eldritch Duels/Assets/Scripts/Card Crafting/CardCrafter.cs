using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace eldritch.cards
{
    [System.Serializable]
    public struct CraftingRecipe
    {
        public string BaseCard;
        public string FodderCard;
        public int CraftCost;
        public string ResultCard;
        public static CraftingRecipe Empty
        {
            get
            {
                CraftingRecipe cr;
                cr.BaseCard = "";
                cr.FodderCard = "";
                cr.ResultCard = "";
                cr.CraftCost = -1;
                return cr;
            }
        }
        
        
    }
    public class CardCrafter : MonoBehaviour
    {
        public Image BaseCard;
        public Image FodderCard;
        public Image ResultCard;
        public Text CraftCost;
        Image CardSelector;
        public GameObject placeholders;
        public GameObject selector;

        private int selectMode = 0;
        private Card baseCardSRC;
        private Card fodderCardSRC;
        private Card resultCardSRC;

        private int page = 0;
        private int maxPage = 0;
        private List<CardContainer> craftable = new List<CardContainer>();

        public void ChooseBase()
        {
            selectMode = 1;
            updateList();
            selector.gameObject.SetActive(true);
            updateSelectionUI();
        }

        public void ChooseFodder()
        {
            selectMode = 2;
            updateList();
            selector.gameObject.SetActive(true);
            updateSelectionUI();
        }

        public void Select(Card c)
        {
            if(selectMode == 1)
            {
                baseCardSRC = c;
            }else if(selectMode == 2)
            {
                fodderCardSRC = c;
            }
            selectMode = 0;
            updateUI();
        }
        //main ui excluding card selection panel
        private void updateUI()
        {
            //update images
            BaseCard.material = baseCardSRC.CardImage;
            FodderCard.material = fodderCardSRC.CardImage;

            CraftingRecipe cr = Library.PreviewCraft(baseCardSRC.CardName, fodderCardSRC.CardName);
            if (!cr.Equals(CraftingRecipe.Empty))
            {
                CraftCost.text = cr.CraftCost + "";
                resultCardSRC = Library.GetCard(cr.ResultCard);
                if(resultCardSRC!= null)
                {
                    ResultCard.material = resultCardSRC.CardImage;
                }
            }

        }

        //updates the list of craftable cards
        private void updateList()
        {
            //get the cards usable for crafting
            craftable = new List<CardContainer>();
            foreach (Card c in Global.userCards)
            {
                int mod = (baseCardSRC.CardName.Equals(c.CardName) || fodderCardSRC.CardName.Equals(c.CardName)) ? 1 : 0;
                foreach (Deck d in Global.userDecks)
                {
                    int inDeck = d.AmountInDeck(c.CardName);
                    if (inDeck < (c.CopiesOwned - mod))
                    {
                        CardContainer newCC;
                        newCC.c = c;
                        newCC.count = c.CopiesOwned - inDeck - mod;
                        craftable.Add(newCC);
                    }
                }
            }
            maxPage = craftable.Count / 24;
            page = 0;
        }

        //selector panel UI update
        public void updateSelectionUI()
        {
            
            //set the placeholder values
            int pos = page * 24;
            foreach(Transform i in placeholders.transform)
            {
                i.gameObject.SetActive(false);
                if(pos < craftable.Count)
                {
                    i.gameObject.SetActive(true);
                    i.gameObject.GetComponent<CardSelector>()._currentCard = craftable[pos].c;
                }
                pos++;
            }

        }


    }
}
