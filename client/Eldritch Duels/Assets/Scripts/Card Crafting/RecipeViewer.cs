using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eldritch.cards {
    public class RecipeViewer : MonoBehaviour
    {
        private int page = 0;
        private int maxPage = 0;
        List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();
        public GameObject control;
        public void Start()
        {
            craftingRecipes = Library.GetAllRecipes();
            maxPage = craftingRecipes.Count/12;
            Debug.Log(craftingRecipes.Count + " recipes");
            updateUI();
        }

        private void updateUI()
        {
            int pos = 0;
            foreach(Transform child in control.transform)
            {
                child.gameObject.SetActive(false);
                if(pos < craftingRecipes.Count)
                {
                    child.gameObject.GetComponent<ShowRecipe>().SetRecipe(craftingRecipes[pos]);
                    child.gameObject.SetActive(true);
                }
                pos++;
            }
        }
        
        public void PageRight()
        {
            page++;
            if (page > maxPage)
                page = 0;
            updateUI();
        }

        public void PageLeft()
        {
            page--;
            if (page < 0)
                page = maxPage;
            updateUI();
        }
    }
}
