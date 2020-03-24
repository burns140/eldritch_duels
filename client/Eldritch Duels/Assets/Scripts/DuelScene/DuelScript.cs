﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using eldritch;
using eldritch.cards;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public struct PlayerState{
    public int mana;
    public int hp;
    public List<Card> library;
    public List<Card> inHand;
    public List<Card> onField;
    
}

public struct DuelRequest {
    public string cardName;

    public string targetName;

    public int myArrPosition;

    public int theirArrPosition;
}

public class DuelScript : MonoBehaviour 
{
    #region UI & Script Variables
    // Cards in the deck UI
    public GameObject cardBack1; 
    public GameObject cardBack2;
    public GameObject cardBack3;
    public GameObject cardBack4;

    public Text gameText; // Display game instructions and hints
    private bool isMyTurn; // Check if it is my turn

    public GameObject handAreaPanel; // Hand Area UI
    public GameObject myPlayAreaPanel; // My Play Area UI
    public GameObject oppPlayAreaPanel; // Opponent's Play Area UI

    public GameObject card; // Card object to create instances

    private Queue<GameObject> deckList = new Queue<GameObject>(); // Store cards in Deck
    private List<GameObject> handList = new List<GameObject>(); // Store cards in Hand
    private List<GameObject> myPlayList = new List<GameObject>(); // Store cards in my Play Area
    private List<GameObject> oppPlayList = new List<GameObject>(); // Store cards in opponent Play Area

    public Button recallButton; // Recall button on the UI
    public GameObject myProfilePic; // My profile pic on the UI
    public GameObject oppProfilePic; // Opponent's profile pic on the UI

    public Text myManaText; // Text to show my current Mana
    public Text oppManaText; // Text to show opponent's current Mana
    public Text myHPText; // Text to show my current HP
    public Text oppHPText; // Text to show opponent's current HP
    public Image myHPImage; // Image to show my current HP

    public Image oppHPImage; // Image to show opponent's current HP

    public Sprite[] availablePictures; // Available profile pics
    private float myCurrentHP; // My Current HP
    private float oppCurrentHP; // Opp Current HP
    private const int MAX_HEALTH = 30; // Max Health for a user
    private const int MAX_MANA = 1; // Max Mana for a user

    private const int MAX_HAND = 8; //max hand size
    private const int MAX_FIELD_SIZE = 7;

    public PlayerState OpponentState; //opp hp, mana

    public PlayerState MyState; //my hp, mana, library

    #endregion

    #region Awake
    // Awake is called when the script instance is being loaded.
    void Start(){
        setUpDeck(); // Set up card list from deck being used
        setUpProfilePics(); // Set up profile pics for both users
        setUpHealthMana(); // Set up health & mana to full for both users
        StartCoroutine(initCoroutines());
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void Update()
    {
        checkDeckCount(); // Check & update card back quantity on deck UI
    }

    // Check current number of cards in deck
    private void checkDeckCount(){
        if(deckList.Count<30){
            cardBack1.SetActive(false); // Hide first card in deck
        }
        if(deckList.Count<20){
            cardBack2.SetActive(false); // Hide second card in deck
        }
        if(deckList.Count<10){
            cardBack3.SetActive(false); // Hide third card in deck
        }
        if(deckList.Count<1){
            cardBack4.SetActive(false); // Hide fourth/last card in deck
        }
    }
    #endregion

    #region Bookkeeping Before Playing
    IEnumerator initCoroutines(){
        yield return StartCoroutine(initalDraw()); // Set up hand to have 6 cards
        yield return StartCoroutine(testPlayArea()); // Test moving cards from hand to my play area
        yield return StartCoroutine(testOppArea()); // Test add cards to opponent play area
    }

    // Called at start of game to fill hand to 6 cards from deck
    IEnumerator initalDraw(){
        int handCount=1;
        while(handCount<=6){ // To add 6 cards to hand
            Card b = DuelFunctions.DrawCard(ref MyState);
            GameObject c = (GameObject)Instantiate(card);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            c.name = b.CardName;
            c.transform.SetParent(handAreaPanel.transform, false); // Add card to hand
            handList.Add(c); // Add card to hand list
            MyState.inHand.Add(b);
            handCount++; 
            yield return new WaitForSeconds(0.5f); 
        }
    }

    // Test Adding to My Play Area
    IEnumerator testPlayArea(){
        int playCount=1;
        while(playCount<=6){ // To add 6 cards to my play area
            Card b = Library.GetCard("Test 0");
            GameObject c = (GameObject)Instantiate(card);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            c.name = b.CardName;
            c.transform.SetParent(myPlayAreaPanel.transform, false); // Add card to my play area
            myPlayList.Add(c); // Add card to my play list
            playCount++; 
            yield return new WaitForSeconds(0.5f); 
        }
    }

    // Test Adding to Opp Play Area
    IEnumerator testOppArea(){
        int oppPlayCount=1;
        while(oppPlayCount<=6){ // To add 6 cards to opp play area
            Card b = Library.GetCard("Test 0");
            GameObject c = (GameObject)Instantiate(card);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            c.transform.SetParent(oppPlayAreaPanel.transform, false); // Add card to opponent play area
            oppPlayList.Add(c); // Add card to opp play list
            oppPlayCount++; 
            yield return new WaitForSeconds(0.5f); 
        }
    }

    // Called at start of game to set up card list from deck being used
    private void setUpDeck(){
        MyState.library = DuelFunctions.ShuffleLibrary(DuelFunctions.GetLibrary());        
        int deckCount=0;
        while(deckCount< MyState.library.Count){ // add cards to deck
            Card b =MyState.library[deckCount];
            GameObject c = (GameObject)Instantiate(card);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            deckList.Enqueue(c); // Add card to deck list
            deckCount++; 
        }
    }
    
    // Set up health and mana text for both users to max values
    private void setUpHealthMana(){
        MyState.hp = MAX_HEALTH;
        MyState.mana = MAX_MANA;
        OpponentState.hp = MAX_HEALTH;
        OpponentState.mana = MAX_MANA;

        myHPText.text = MAX_HEALTH + " HP";
        myManaText.text = MAX_MANA + " MANA";
        oppHPText.text = MAX_HEALTH + " HP";
        oppManaText.text = MAX_MANA + " MANA";
    }

    // Set up profile pics on the UI
    private void setUpProfilePics(){
        bool hasMyPicIndex=true; // Check if my pic is uploaded or has index (@KEVIN G)
        bool hasOppPicIndex=true; // Check if opponent's pic is uploaded or has index (@KEVIN G)
        int myPicIndex=0; // @TODO Get my profile pic index from server (@Stephen)
        int oppPicIndex=1; // @TODO Get opponent's profile pic index from server (@Stephen)

        if(hasMyPicIndex){
            myProfilePic.GetComponent<Image>().sprite = availablePictures[myPicIndex]; // Set image from list
        }
        else{
            // @TODO GET MY UPLOADED PICTURE FROM SERVER (@KEVIN G)
            // Sprite newImage = ;
            // myProfilePic.GetComponent<Image>().sprite = newImage;
        }
        myProfilePic.SetActive(true); // Unhide the image UI

        if(hasOppPicIndex){
            oppProfilePic.GetComponent<Image>().sprite = availablePictures[oppPicIndex]; // Set image from list
        }
        else{
            // @TODO GET OPP UPLOADED PICTURE FROM SERVER (@KEVIN G)
            // Sprite newImage = ;
            // oppProfilePic.GetComponent<Image>().sprite = newImage;
        }

        oppProfilePic.SetActive(true); // Unhide the image UI

        
    }
    
    #endregion

    #region Place Cards
    // Called at the beginning of every turn
    private void drawCard(){
        if(deckList.Count>0){
            Card b = DuelFunctions.DrawCard(ref MyState);
            if(b == null)
                return;
            GameObject c = (GameObject)Instantiate(card);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            handList.Add(c); // Move 1 card from deck to hand
            MyState.inHand.Add(b);
        }
    }

    // Play card from my hand to my playing area
    private void playMyCard(Card played){
        if(!DuelFunctions.CanCast(played, MyState)){
            return;
        }
        if(MyState.onField.Count >= MAX_FIELD_SIZE){
            return;
        }
        Card b = DuelFunctions.RemoveFromHand(played, ref MyState);
        GameObject c = (GameObject)Instantiate(card);
        c.GetComponent<Image>().sprite = null;
        c.GetComponent<Image>().material = played.CardImage;
        if(played.SpellType == CardType.SPELL){
           ResolveAbilities(played, true);
        }else{
            MyState.onField.Add(b);
            myPlayList.Add(c);
            ResolveAbilities(played, true);
        }
        MyState.mana -= played.CardCost;

        //TODO update ui

        //TODO tell oppenent to resolve card
    }

    //server sends request that opp played a card
    //this method ties the server request with the
    //client side
    public void ServerPlayOppCard(string cardName){
        Card toPlay = Library.GetCard(cardName);
        playOppCard(toPlay);
    }

    // Play opponent's card
    private void playOppCard(Card played){
        OpponentState.onField.Add(played);
        GameObject c = (GameObject)Instantiate(card);
        c.GetComponent<Image>().sprite = null;
        c.GetComponent<Image>().material = played.CardImage;
        if(played.SpellType == CardType.SPELL){
            ResolveAbilities(played, false);
        }else{
            oppPlayList.Add(c);
            OpponentState.onField.Add(played);
            ResolveAbilities(played, false);

        }

        OpponentState.mana -= played.CardCost;

        //TODO update ui
    }


    private void ResolveAbilities(Card played, bool iPlayed){
        foreach(Effect e in played.Abilities){
            switch(e.GetTargetType()){
                case EffectTarget.OPPONENT:
                    e.execute(ref OpponentState);
                    break;
                case EffectTarget.SELF:
                    e.execute(ref MyState);
                    break;
                case EffectTarget.CARD:
                    //TODO select card and execute
                    break;
                default:
                    break;

            }
        }
    }

    #endregion

    #region Attack & Update Health
    // Play card animations when I attack 
    private void myAttack(){
        foreach(GameObject value in myPlayList){
            // @TODO some attack animations (@BRANDON)
            float hit=0; // @TODO get attack value of the card (@DHAIRYA)
            // @TODO send attack value to server (@KEVIN M)
            updateOppHealth(hit); // After each card's attack, update opponent's health
        }
    }

    // After my card attacks opponent
    private void updateOppHealth(float hit){
        oppCurrentHP -= hit; // Decrease attack from HP
        oppHPImage.fillAmount = oppCurrentHP/MAX_HEALTH; // Update opponent's HP on UI
    }

    // After opponent's card attacks me
    private void updateMyHealth(){
        float hit=0; // @TODO get attack value from server (@KEVIN M)
        myCurrentHP -= hit; // Decrease attack from HP
        myHPImage.fillAmount = myCurrentHP/MAX_HEALTH; // Update my HP on UI
    }
    #endregion

    #region End Turns
    // Check cards in hand
    private void checkHand(){

    }

    // End my turn
    private void endMyTurn(){
        isMyTurn = false; // No longer my turn
        string endString = "YOUR TURN"; // Send this to server
    }


    // End opponent's play
    private void endOppTurn(){
        isMyTurn = true; // Now it's my turn
    }
    #endregion

    #region End Game
    // End game
    private void endGame(){

    }
    #endregion
}
