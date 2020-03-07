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
        Image BaseCard;
        Image FodderCard;
        Image ResultCard;
        Text CraftCost;
        Image CardSelector;

        private int selectMode = 0;
        private Card baseCardSRC;
        private Card fodderCardSRC;
        private Card resultCardSRC;

        public void ChooseBase()
        {
            selectMode = 1;
            CardSelector.gameObject.SetActive(true);
        }

        public void ChooseFodder()
        {
            selectMode = 2;
            CardSelector.gameObject.SetActive(true);
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

        //selector panel UI update
        public void updateSelectionUI()
        {

        }


    }
}
