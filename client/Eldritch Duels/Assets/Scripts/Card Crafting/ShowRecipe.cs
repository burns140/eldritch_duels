using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace eldritch.cards {
    public class ShowRecipe : MonoBehaviour
    {
        public CraftingRecipe recipe;
        public RecipePanel viewer;
        public Image preview;

        private Card b;
        private Card f;
        private Card r;
        public void SetRecipe(CraftingRecipe cr)
        {
            r = Library.GetCard(cr.ResultCard);
            b = Library.GetCard(cr.BaseCard);
            f = Library.GetCard(cr.FodderCard);
            recipe = cr;
            if(r != null)
                preview.material = r.CardImage;
        }

        public void OnClick()
        {
            if (b != null && f != null && r != null)
            {
                viewer.BaseCard.material = b.CardImage;
                viewer.FodderCard.material = f.CardImage;
                viewer.ResultCard.material = r.CardImage;
                viewer.CraftCost.text = "Cost: " + recipe.CraftCost;
                if(Global.EnoughToCraft(recipe)){
                        viewer.Indicator.material = viewer.Green;
                }else{
                    viewer.Indicator.material = viewer.Gray;
                }
            }
            

        }

    }
}
