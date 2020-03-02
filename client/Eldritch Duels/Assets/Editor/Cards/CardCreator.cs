using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using eldritch.cards;
using System;
using UnityEditor.SceneManagement;

namespace eldritch.editor
{
    public class CardCreator : EditorWindow
    {
        #region params
        private string cardName = "";
        private string attack = "0";
        private string defence = "0";
        private List<Effect> effects = new List<Effect>();
        private string cost = "0";
        private CardRarity rarity = CardRarity.NULL;
        private Material cardMat;
        private CardType cardType = CardType.NULL;
        private int mode = 0;
        private int backMode = 0;
        private Vector2 scrollPos;
        

        #endregion

        [MenuItem("Card Creator", menuItem = "Eldritch Duels/Card Creator")]
        public static void init()
        {
            EditorWindow win = EditorWindow.GetWindow(typeof(CardCreator));
            win.titleContent.text = "Card Editor";
        }

        public CardCreator()
        {
            this.titleContent = new GUIContent("Card Creator");
            this.minSize = new Vector2(600, 200);
            LoadCards();
        }
        private void LoadCards()
        {

        }

        private void SaveCards()
        {

        }
        Editor matEdit;
        private void OnGUI()
        {
            //menu items
            GUILayout.BeginArea(new Rect(0, 0, this.position.width, 20));
            if(GUI.Button(new Rect(10,5,50,15), "New"))
            {
                this.cardName = "";
                this.attack = 0 + "";
                this.defence = 0 + "";
                //this.effects = cards[pos].Effects + "";
                this.cost = 0 + "";
                this.rarity = CardRarity.NULL;
                this.cardMat = null;
                this.cardType = CardType.NULL;
                mode = 1;
            }
            if (GUI.Button(new Rect(70, 5, 50, 15), "Load"))
            {
                scrollPos = Vector2.zero;
                mode = 3;
            }
            if (mode == 1 || mode == 2)
            {
                if (GUI.Button(new Rect(130, 5, 50, 15), "Save"))
                {
                    try
                    {
                        if (cardName != null || cardName != "")
                        {
                            Card c;
                            bool isNew = false;
                            //c = GameObject.Find("ContentManager").GetComponent<ContentLibrary>().GetCard(cardName);
                            c = GameObject.Find("ContentManager").GetComponent<ContentLibrary>().GetCard(cardName);
                            if (c == null)
                            {
                                c = new Card(Library.GetNextID(), this.cardName);
                                isNew = true;
                            }
                            c.CardCost = int.Parse(this.cost);
                            c.AttackPower = int.Parse(this.attack);
                            c.DefencePower = int.Parse(this.defence);
                            c.SpellRarity = this.rarity;
                            c.CardImage = this.cardMat;
                            c.SpellType = this.cardType;
                            Library.AddCard(c);
                            EditorUtility.SetDirty(this);
                            EditorSceneManager.MarkSceneDirty(GameObject.Find("ContentManager").scene);
                            PrefabUtility.ApplyPrefabInstance(GameObject.Find("ContentManager"), InteractionMode.AutomatedAction);
                        }
                        else
                        {
                            throw new Exception();
                        }

                    }catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        backMode = mode;
                        mode = 3201;
                    }

                    

                }
                if (GUI.Button(new Rect(190, 5, 50, 15), "Delete"))
                {
                    Library.RemoveCard(this.cardName);
                    EditorUtility.SetDirty(this);
                    EditorSceneManager.MarkSceneDirty(GameObject.Find("ContentManager").scene);
                    PrefabUtility.ApplyPrefabInstance(GameObject.Find("ContentManager"), InteractionMode.AutomatedAction);
                    this.cardName = "";
                    this.attack = 0 + "";
                    this.defence = 0 + "";
                    //this.effects = cards[pos].Effects + "";
                    this.cost = 0 + "";
                    this.rarity = CardRarity.NULL;
                    this.cardMat = null;
                    this.cardType = CardType.NULL;
                    mode = 1;
                    
                }
            }
            GUILayout.EndArea();
            if (mode == 1 || mode == 2)
            {
                //card params
                GUILayout.BeginArea(new Rect(0, 21, this.position.width / 2, this.position.height - 30));
                if (mode == 1)
                    cardName = GUI.TextField(new Rect(60, 10, 100, 20), cardName);
                else if (mode == 2)
                    GUI.Label(new Rect(60, 10, 100, 20), cardName);
                GUI.Label(new Rect(10, 10, 50, 20), "Name: ");

                attack = GUI.TextField(new Rect(60, 50, 100, 20), attack);
                GUI.Label(new Rect(10, 50, 50, 20), "ATK: ");

                defence = GUI.TextField(new Rect(60, 90, 100, 20), defence);
                GUI.Label(new Rect(10, 90, 50, 20), "DEF: ");

                cost = GUI.TextField(new Rect(60, 130, 100, 20), cost);
                GUI.Label(new Rect(10, 130, 50, 20), "Cost: ");

                GUILayout.BeginArea(new Rect(60, 170, 100, 20));
                cardMat = (Material)EditorGUILayout.ObjectField(cardMat, typeof(Material), true);
                GUILayout.EndArea();
                GUI.Label(new Rect(10, 170, 50, 20), "IMG: ");

                GUI.Label(new Rect(10, 210, 50, 20), "Rarity: ");
                GUILayout.BeginArea(new Rect(60, 210, 100, 20));
                rarity = (CardRarity)EditorGUILayout.EnumPopup("", rarity);
                GUILayout.EndArea();
                GUI.Label(new Rect(10, 250, 50, 20), "Type: ");
                GUILayout.BeginArea(new Rect(60, 250, 100, 20));
                this.cardType = (CardType)EditorGUILayout.EnumPopup("", cardType);
                GUILayout.EndArea();


                GUILayout.EndArea();

                //preview
                
                    
                GUILayout.EndArea();
            }else if(mode == 3)
            {
                GUILayout.BeginArea(new Rect(0, 20, this.position.width, this.position.height - 100));
                GUILayout.BeginScrollView(scrollPos);
                //generate button for each card
                List<Card> cards = Library.GetAllCards();
                int maxRows = ((int)this.position.height - 100) / 30 + 1;
                int perRow = cards.Count / maxRows + 1;
                int pos = 0;
                for(int i = 0; i < perRow; i++)
                {
                    for(int j = 0; j< maxRows; j++)
                    {
                        if(pos < cards.Count)
                        {
                            if(GUI.Button(new Rect(110 * i, 30*j, 100,30), cards[pos].CardName))
                            {
                                this.cardName = cards[pos].CardName;
                                this.attack = cards[pos].AttackPower + "";
                                this.defence = cards[pos].DefencePower + "";
                                //this.effects = cards[pos].Effects + "";
                                this.cost = cards[pos].CardCost + "";
                                this.rarity = cards[pos].SpellRarity;
                                this.cardMat = cards[pos].CardImage;
                                this.cardType = cards[pos].SpellType;
                                mode = 2;

                            }
                        }
                        else
                        {
                            break;
                        }
                        pos++;
                    }
                    if(pos >= cards.Count)
                    {
                        break;
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }else if(mode == 3201)
            {
                GUILayout.BeginArea(new Rect(0, 20, this.position.width, this.position.height - 100));
                GUI.Label(new Rect(0, 10, 200, 20), "INVALID CARD PARAMETERS");
                if(GUI.Button(new Rect(0,40,100,20), "BACK"))
                {
                    mode = backMode;
                }

                GUILayout.EndArea();
            }

        }
        
    }
}
