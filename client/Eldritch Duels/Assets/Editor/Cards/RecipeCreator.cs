using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using eldritch.cards;
using UnityEditor.SceneManagement;

namespace eldritch.editor {
    public class RecipeCreator : EditorWindow
    {
        private string baseCard = "";
        private string fodderCard = "";
        private string craftCost = "";
        private string resultCard = "";
        [MenuItem("Recipe Creator", menuItem = "Eldritch Duels/Recipe Editor")]
        public static void init()
        {
            EditorWindow win = EditorWindow.GetWindow(typeof(RecipeCreator));
            win.titleContent.text = "Recipe Editor";
        }
        private Vector2 scrollPos = Vector2.zero;
        private float scrollBar = 0;
        public RecipeCreator()
        {
            this.minSize = new Vector2(700, 200);
        }

        private void OnGUI()
        {

            List<CraftingRecipe> recipes = Library.GetAllRecipes();
            
            //get scroll inputs
            scrollBar = GUI.VerticalSlider(new Rect(this.position.width/2 + 10, 10,15,this.position.height-100),scrollBar,0, -(10 + (recipes.Count * 60) - this.position.height + 100));
            scrollPos = new Vector2(0,scrollBar);


            //add recipe list
            GUI.Label(new Rect(this.position.width / 2 + 50, 10, 50, 20), "Base");
            baseCard = GUI.TextField(new Rect(this.position.width / 2 + 100, 10, 100, 20), baseCard);
            GUI.Label(new Rect(this.position.width / 2 + 50, 40, 50, 20), "Fodder");
            fodderCard = GUI.TextField(new Rect(this.position.width / 2 + 100, 40, 100, 20), fodderCard);
            GUI.Label(new Rect(this.position.width / 2 + 50, 70, 50, 20), "Result");
            resultCard = GUI.TextField(new Rect(this.position.width / 2 + 100, 70, 100, 20), resultCard);
            GUI.Label(new Rect(this.position.width / 2 + 50, 100, 50, 20), "Cost");
            string tmp = GUI.TextField(new Rect(this.position.width / 2 + 100, 100, 100, 20), (craftCost + ""));
            try
            {
                craftCost = int.Parse(tmp) + "";
            }catch (Exception e)
            {
                if(tmp != "")
                    craftCost = "0";
            }
            if(GUI.Button(new Rect(this.position.width/2 + 50, 130, 100, 15), "ADD"))
            {
                CraftingRecipe cr;
                cr.BaseCard = baseCard;
                cr.FodderCard = fodderCard;
                cr.ResultCard = resultCard;
                cr.CraftCost = int.Parse(craftCost);
                Library.AddRecipe(cr);
                EditorUtility.SetDirty(this);
                EditorSceneManager.MarkSceneDirty(GameObject.Find("ContentManager").scene);
                PrefabUtility.ApplyPrefabInstance(GameObject.Find("ContentManager"), InteractionMode.AutomatedAction);
            }

            //show all recipes
            GUILayout.BeginArea(new Rect(0,0,this.position.width/2, this.position.height-50));
            //scrollPos = GUI.BeginScrollView(new Rect(0, 0-scrollPos.y, this.position.width / 2, this.position.height - 50), scrollPos, new Rect(0, 0, this.position.width / 2, this.position.height - 50));
            
            for(int i = 0; i < recipes.Count; i++)
            {
                int pos = i;
                GUI.Label(new Rect(10 , 10 + (60 * i) + scrollPos.y, 50, 20), recipes[i].BaseCard);
                GUI.Label(new Rect(70 , 10 + (60 * i)+ scrollPos.y, 20, 20), " + ");
                GUI.Label(new Rect(90 , 10 + (60 * i)+ scrollPos.y, 50, 20), recipes[i].FodderCard);
                GUI.Label(new Rect(150 , 10 + (60 * i)+ scrollPos.y, 20, 20), " = ");
                GUI.Label(new Rect(170 , 10 + (60 * i)+ scrollPos.y, 50, 20), recipes[i].ResultCard);
                GUI.Label(new Rect(150 , 30 + (60 * i)+ scrollPos.y, 100, 20), "Cost: " + recipes[i].CraftCost);
                if(GUI.Button(new Rect(230, 10 + (60 * i)+ scrollPos.y, 100, 15), "REMOVE"))
                {
                    Library.RemoveRecipe(recipes[pos]);
                    EditorUtility.SetDirty(this);
                    EditorSceneManager.MarkSceneDirty(GameObject.Find("ContentManager").scene);
                    PrefabUtility.ApplyPrefabInstance(GameObject.Find("ContentManager"), InteractionMode.AutomatedAction);
                }
            }
            //GUI.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
