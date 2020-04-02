using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        public void CancelSelect()
        {
            selectMode = 0;
            selector.gameObject.SetActive(false);
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
            if(baseCardSRC != null)
                BaseCard.material = baseCardSRC.CardImage;
            if(fodderCardSRC != null)
                FodderCard.material = fodderCardSRC.CardImage;
            ResultCard.material = null;
            if (baseCardSRC != null && fodderCardSRC != null)
            {
                Debug.Log("Showing recipe");
                CraftingRecipe cr = Library.PreviewCraft(baseCardSRC.CardName, fodderCardSRC.CardName);
                if (!cr.Equals(CraftingRecipe.Empty))
                {

                    //CraftCost.text = "COST: " + cr.CraftCost + "";
                    resultCardSRC = Library.GetCard(cr.ResultCard);
                    if (resultCardSRC != null)
                    {
                        ResultCard.material = resultCardSRC.CardImage;
                    }
                }
            }

        }
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        //updates the list of craftable cards
        private void updateList()
        {
            //get the cards usable for crafting
            craftable = new List<CardContainer>();
            Debug.Log(Global.userCards.Count + " user cards");
            foreach (Card c in Global.userCards)
            {
                
                int mod = ((baseCardSRC != null && baseCardSRC.CardName.Equals(c.CardName)) || (fodderCardSRC != null && fodderCardSRC.CardName.Equals(c.CardName))) ? 1 : 0;
                int inDeck = 0;
                bool addable = true;
                foreach (Deck d in Global.userDecks)
                {
                    inDeck = d.AmountInDeck(c.CardName);
                    if (inDeck >= (c.CopiesOwned - mod))
                    {
                        addable = false;
                    }
                }
                if(addable){
                    CardContainer newCC;
                    newCC.c = c;
                    newCC.count = c.CopiesOwned - inDeck - mod;
                    craftable.Add(newCC);
                }
            }
            maxPage = craftable.Count / 24;
            page = 0;
        }

        public void Craft()
        {
            Global.RemoveCard(baseCardSRC.CardName);
            Global.RemoveCard(fodderCardSRC.CardName);
            Global.AddCard(resultCardSRC.CardName);
            //TODO update user credits

            //reset UI
            baseCardSRC = null;
            fodderCardSRC = null;
            resultCardSRC = null;
            CraftCost.text = "COST: ";
            BaseCard.material = null;
            FodderCard.material = null;
            ResultCard.material = null;
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
                    i.gameObject.GetComponent<CardSelector>().CurrentCard = craftable[pos].c;
                }
                pos++;
            }

        }


    }
}
