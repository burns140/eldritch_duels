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
        private bool hasFly = false;
        private bool hasStealth;
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

        private void addEffect(Effect e){
            this.effects.Add(e);
        }

        private void removeEffect(Effect e){
            for(int i = 0; i<effects.Count;i++){
                if(effects[i].GetName().Equals(e.GetName())){
                    effects.RemoveAt(i);
                    return;
                }
            }
        }
        private int countEfect(Effect e){
            int c = 0;
            foreach(Effect ei in effects){
                if(ei.GetName().Equals(e.GetName())){
                    c++;
                }
            }
            return c;
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
                this.effects = new List<Effect>();
                this.cost = 0 + "";
                this.rarity = CardRarity.NULL;
                this.cardMat = null;
                this.cardType = CardType.NULL;
                this.hasFly = false;
                this.hasStealth = false;
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
                            c.HasFly = this.hasFly;
                            c.HasStealth = this.hasStealth;
                            foreach(Effect e in effects){
                                c.AddAbility(e);
                            }
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
                    this.effects = new List<Effect>();
                    this.cost = 0 + "";
                    this.rarity = CardRarity.NULL;
                    this.cardMat = null;
                    this.cardType = CardType.NULL;
                    this.hasFly = false;
                    this.hasStealth = false;
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

                //card effects
                GUILayout.BeginArea(new Rect(this.position.width/2,10,this.position.width/2-50,this.position.height-100));
                GUI.Label(new Rect(10,20,100,20), "EFFECTS:");
                //Deal damage
                GUI.Label(new Rect(10,50,100,20), ("Deal Damage: " + countEfect(new DealDamage())));
                if(GUI.Button(new Rect(120,50,20,20), "+")){
                    addEffect(new DealDamage());
                }
                if(GUI.Button(new Rect(150,50,20,20), "-")){
                    removeEffect(new DealDamage());
                }

                //Gain health
                GUI.Label(new Rect(10,80,100,20), ("Gain Health: " + countEfect(new GainHealth())));
                if(GUI.Button(new Rect(120,80,20,20), "+")){
                    addEffect(new GainHealth());
                }
                if(GUI.Button(new Rect(150,80,20,20), "-")){
                    removeEffect(new GainHealth());
                }

                //draw card
                GUI.Label(new Rect(10,110,100,20), ("Draw Card: " + countEfect(new DrawCard())));
                if(GUI.Button(new Rect(120,110,20,20), "+")){
                    addEffect(new DrawCard());
                }
                if(GUI.Button(new Rect(150,110,20,20), "-")){
                    removeEffect(new DrawCard());
                }
                GUI.Label(new Rect(10,140,100,20), ("Gain Mana: " + countEfect(new AddMana())));
                if(GUI.Button(new Rect(120,140,20,20), "+")){
                    addEffect(new AddMana());
                }
                if(GUI.Button(new Rect(150,140,20,20), "-")){
                    removeEffect(new AddMana());
                }

                //fly
                if(GUI.Button(new Rect(10,170,100,20), "Fly: " + hasFly)){
                    hasFly = !hasFly;
                }
                //stealth
                if(GUI.Button(new Rect(10,200,100,20), "Stealth: " + hasStealth)){
                    hasStealth = !hasStealth;
                }
                
                    
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
                                this.hasFly = cards[pos].HasFly;
                                this.hasStealth = cards[pos].HasStealth;
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
