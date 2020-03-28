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

[System.Serializable]
public struct PlayerState{
    public int mana;
    public int hp;
    public List<Card> inHand;
    public List<Card> onField;
    public List<Card> oppField;
    public List<Card> library;
}

[System.Serializable]
public struct AttackBlock{
    public GameObject attacker;
    public GameObject blocker;
    public Card attackCard;
}

public enum Phase{
    MAIN,
    ATTACK,
    BLOCK,
    WAITING,
    DISCARD,
    END
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
    
    public bool isMyTurn = false; // Check if it is my turn

    public GameObject handAreaPanel; // Hand Area UI
    public GameObject myPlayAreaPanel; // My Play Area UI
    public GameObject oppPlayAreaPanel; // Opponent's Play Area UI

    public GameObject myCard; // my Card object to create instances
    public GameObject oppCard; // opponent Card object to create instances

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
    public Phase currentPhase = Phase.MAIN;
    public int currentTurn = 1;

    public Text phaseText = null;
    

    [SerializeField]
    private PlayerState myState;
    [SerializeField]
    private PlayerState oppState;

    private const string WON_PREF_KEY = "whowon"; // PREF KEY to store who won

    #endregion

    #region Awake
    // Awake is called when the script instance is being loaded.
    void Awake(){
        setUpDeck(); // Set up card list from deck being used
        setUpProfilePics(); // Set up profile pics for both users
        setUpHealthMana(); // Set up health & mana to full for both users
        StartCoroutine(initCoroutines());

        Thread T = new Thread((new ThreadStart(Listener)));
        T.Start();
    }

    private void Listener() {
        readStreamAsync();
    }

    public async void readStreamAsync() {
        while (true) {
            Byte[] data = new byte[1024];
            int read_bytes = await Global.stream.ReadAsync(data, 0, 1024);
            string trimmed = System.Text.Encoding.ASCII.GetString(data).Trim();
            Debug.Log($"Trimmed: {trimmed}");
            receivedDataFromOpp(trimmed);
        }

    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void Update()
    {
        checkDeckCount(); // Check & update card back quantity on deck UI
        //update hp and mana
        setMyHealth(myState.hp);
        setOppHealth(oppState.hp);
        myHPText.text = myState.hp + " HP";
        myManaText.text = myState.mana + " MANA";
        oppHPText.text = oppState.hp + " HP";
        oppManaText.text = oppState.mana + " MANA";

        if(oppState.hp <= 0){
            endGame(true);
        }else if(myState.hp <= 0){
            endGame(false);
        }

    }

    public void NextPhase(){
        if(phaseText == null){
            return;
        }if(!isMyTurn && currentPhase != Phase.BLOCK){
            currentPhase = Phase.WAITING;
            phaseText.text = "WAITING";
        }
        if(currentPhase == Phase.MAIN){
            currentPhase = Phase.ATTACK;
            phaseText.text = "CONFIRM";
        }else if(currentPhase == Phase.ATTACK && isMyTurn){
            currentPhase = Phase.WAITING;
            phaseText.text = "WAITING";
            //TODO send attackers and tell opp to block
        }else if(!isMyTurn && currentPhase == Phase.BLOCK){
            phaseText.text = "CONFIRM";

        }else if(currentPhase == Phase.BLOCK && !isMyTurn){
            currentPhase = Phase.END;
            //TODO send blockers and resolve 
        }
    }

    public void EndTurnNow(){
        if(isMyTurn && currentPhase != Phase.WAITING){
            endMyTurn();
        }
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
        //yield return (StartCoroutine(testPlays()));
        //yield return StartCoroutine(testPlayArea()); // Test moving cards from hand to my play area
        //yield return StartCoroutine(testOppArea()); // Test add cards to opponent play area
    }

    // Called at start of game to fill hand to 6 cards from deck
    IEnumerator initalDraw(){
        int handCount=1;
        while(handCount<=6){ // To add 6 cards to hand
            Card b = DuelFunctions.DrawCard(ref myState);
            if(b == null)
                break;
            GameObject c = (GameObject)Instantiate(myCard);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            c.name = b.CardName;
            c.transform.SetParent(handAreaPanel.transform, false); // Add card to hand
            myState.inHand.Add(b);
            handList.Add(c); // Add card to hand list
            handCount++; 
            yield return new WaitForSeconds(0.5f); 
        }
    }

    // Test Adding to My Play Area
    IEnumerator testPlayArea(){
        int playCount=1;
        while(playCount<=6){ // To add 6 cards to my play area
            Card b = Library.GetCard("Mi_Go");
            GameObject c = (GameObject)Instantiate(myCard);
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
            Card b = Library.GetCard("Mi_Go");
            GameObject c = (GameObject)Instantiate(oppCard);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            c.transform.SetParent(oppPlayAreaPanel.transform, false); // Add card to opponent play area
            oppPlayList.Add(c); // Add card to opp play list
            oppPlayCount++; 
            yield return new WaitForSeconds(0.5f); 
        }
    }
    //test playing and recalling cards for user and opp
    IEnumerator testPlays(){
        
        Debug.Log("Test Opp Play Card");
        //opp plays cards
        playOppCard("Mi_Go");
        playOppCard("Mi_Go");
        yield return new WaitForSeconds(2);
        
        

    }

    // Called at start of game to set up card list from deck being used
    private void setUpDeck(){
        Debug.Log("Init Deck");
        myState.library = DuelFunctions.ShuffleLibrary(DuelFunctions.GetLibrary());
        int deckCount=0;
        while(deckCount<myState.library.Count){ // To add 30 card from my deck to my list
            Card b = myState.library[deckCount];
            GameObject c = (GameObject)Instantiate(myCard);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            deckList.Enqueue(c); // Add card to deck list
            Destroy(c);
            deckCount++; 
        }
    }
    
    // Set up health and mana text for both users to max values
    private void setUpHealthMana(){
        myState.hp = MAX_HEALTH;
        myState.mana = MAX_MANA;
        oppState.hp = MAX_HEALTH;
        oppState.mana = MAX_MANA;


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
            Card b = DuelFunctions.DrawCard(ref myState);
            myState.inHand.Add(b);
            GameObject c = (GameObject)Instantiate(myCard);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            c.name = b.CardName;
            c.transform.SetParent(handAreaPanel.transform, false); // Add card to my play area
            handList.Add(c); // Add card to my play list
        }
    }

    public bool CanCast(GameObject c){
        if(DuelFunctions.CanCast(Library.GetCard(c.name), myState)){
            return true;
        }
        return false;
    }
    // Play card from my hand to my playing area
    public bool playMyCard(string cardName, GameObject c){
        if(myState.onField.Count >= DuelFunctions.MAX_FIELD){
            return false;
        }
        for(int i = 0; i < myState.inHand.Count;i++){
            if(myState.inHand[i].CardName.Equals(cardName) && DuelFunctions.CanCast(myState.inHand[i], myState)){
                Card played = myState.inHand[i];
                //edit state
                myState.mana -= played.CardCost;
                myState.onField.Add(played);
                myState.inHand.RemoveAt(i);
                Debug.Log("Resolving abilities");
                StartCoroutine(resolveAbilities(played, c));

                //sync with opp
                string data = "play:" + cardName;
                sendDataToOpp(data);
                return true;
                
            }
        }

        return false;
    }

    public bool recallCard(string cardName){
        if(myState.onField.Count >= DuelFunctions.MAX_FIELD || true){ //remove true for demo
            Card recalled = null;
            //update manager
            for(int i = 0; i< myState.onField.Count;i++){
                if(myState.onField[i].CardName.Equals(cardName)){
                    recalled = myState.onField[i];
                    myState.onField.RemoveAt(i);
                    myState.inHand.Add(recalled);
                    break;
                }
            }
            if(recalled == null){
                return false;
            }
            //sync with opp
            string data = "recall:" + cardName;
            sendDataToOpp(data);

            return true;
        }
            return false;
            
    }

    // Play opponent's card
    public void playOppCard(string cardName){
        Card played = Library.GetCard(cardName);
        //update manager
        oppState.onField.Add(played);
        oppState.mana -= played.CardCost;

        //update ui
        GameObject c = (GameObject)Instantiate(myCard);
        c.GetComponent<Image>().sprite = null;
        c.GetComponent<Image>().material = played.CardImage;
        c.name = played.CardName;
        c.transform.SetParent(oppPlayAreaPanel.transform, false); // Add card to opp play area
        oppPlayList.Add(c); // Add card to opp play list

        StartCoroutine(resolveAbilities(played, c));
        
    }
    public void recallOppCard(string cardName){
        //update manager
        for(int i = 0; i < oppState.onField.Count;i++){
            if(oppState.onField[i].CardName.Equals(cardName)){
                oppState.onField.RemoveAt(i);
                break;
            }
        }
        //update ui
        foreach(Transform c in oppPlayAreaPanel.transform){
            if(c.gameObject.name.Equals(cardName)){
                Destroy(c.gameObject);
                break;
            }
        }
        for(int i = 0;i < oppPlayList.Count;i++){
            if(oppPlayList[i].name.Equals(cardName)){
                oppPlayList.RemoveAt(i);
                break;
            }
        }
    }
    
    #endregion

    #region battle management

    

    public List<AttackBlock> attackers = new List<AttackBlock>();
    //add attacker to attack with
    public void AddAttacker(GameObject attacker){
        if(attacker.GetComponent<Draggable>() == null){
            return;
        }
        //check if already in attack list
        foreach(AttackBlock g in attackers){
            if(g.attacker.GetHashCode().Equals(attacker.GetHashCode())){
                return;
            }
        }
        AttackBlock at;
        at.attacker = attacker;
        at.attackCard = null;
        foreach(Card c in myState.onField){
            if(c.CardName.Equals(attacker.name)){
                at.attackCard = c;
                break;
            }
        }
        at.blocker = null;
        attackers.Add(at);
        Debug.Log("# attacking: " + attackers.Count);
    }
    //remove creature to attack with
    public void RemoveAttacker(GameObject attacker){
        if(attacker.GetComponent<Draggable>() == null){
            return;
        }
        for(int i = 0; i < attackers.Count;i++){
            if(attackers[i].attacker.GetHashCode().Equals(attacker.GetHashCode())){
                attackers.RemoveAt(i);
                return;
            }
        }
    }
    
    private GameObject atb; //placehoder for attacker
    private GameObject blockwith; //placeholder for blocker
    //set placeholder for attacker
    public void SetToBlock(GameObject attacker){
        if(atb != null){
            atb.GetComponent<Draggable>().isBlocking = false;
            atb.GetComponent<Image>().color = Color.white;
        }
        this.atb = attacker;
        if(atb != null && blockwith != null){
            makeAttackBlockLink();
        }
    }
    //set placeholder for blocker
    public void SetBlocker(GameObject blocker){
        if(blockwith != null){
            blockwith.GetComponent<Draggable>().isBlocking = false;
            blockwith.GetComponent<Image>().color = Color.white;
        }
        this.blockwith = blocker;
        if(atb != null && blockwith != null){
            makeAttackBlockLink();
        }
    }
    //remove place holder for blocker
    public void RemoveBlocker(GameObject blocker){
        for(int i = 0; i < attackers.Count;i++){
            if(attackers[i].blocker != null && attackers[i].blocker.GetHashCode().Equals(blocker.GetHashCode())){
                AttackBlock ab = attackers[i];
                attackers[i].blocker.GetComponent<Draggable>().isBlocking = false;
                attackers[i].blocker.GetComponent<Image>().color = Color.white;
                ab.blocker = null;
                attackers[i] = ab;
                attackers[i].attacker.GetComponent<Draggable>().isBlocking = false;
                attackers[i].attacker.GetComponent<Image>().color = Color.white;
                break;
            }
        }
    }
    //remove placeholder for attacker
    public void RemoveToBlock(GameObject attacker){
        for(int i = 0; i < attackers.Count;i++){
            if(attackers[i].attacker.GetHashCode().Equals(attacker.GetHashCode())){
                AttackBlock ab = attackers[i];
                attackers[i].blocker.GetComponent<Draggable>().isBlocking = false;
                attackers[i].blocker.GetComponent<Image>().color = Color.white;
                ab.blocker = null;
                attackers[i] = ab;
                attackers[i].attacker.GetComponent<Draggable>().isBlocking = false;
                attackers[i].attacker.GetComponent<Image>().color = Color.white;
                break;
            }
        }
    }

    //sets up link between attacker and blocker
    private void makeAttackBlockLink(){
        for(int i = 0; i < attackers.Count;i++){
            if(attackers[i].attacker.GetHashCode().Equals(atb.GetHashCode())){
                AttackBlock ab = attackers[i];
                ab.blocker = blockwith;
                attackers[i] = ab;
                Debug.Log("Link made");
                break;
            }
        }
        atb = null;
        blockwith = null;
        Debug.Log("links: " + attackers.Count);
    }

    //format and send blocker string to opponent
    private void confirmBlockers(){
        string data = "block:";
        bool first = true;
        foreach(AttackBlock ab in attackers){
            if(first && ab.blocker != null){
                data = data + ab.attacker.name + "-" + ab.blocker.name;
                first = false;

            }else if(ab.blocker != null){
                data = data + "," + ab.attacker.name + "-" + ab.blocker.name;
            }
        }
        currentPhase = Phase.WAITING;
        sendDataToOpp(data);
    }

    //format and sent attacker to opponent
    private void confirmAttackers(){
        string data = "attack:";
        bool first = true;
        foreach(AttackBlock ab in attackers){
            if(first){
                data = data + ab.attacker.name;
                first = false;
            }else{
                data = data + "," + ab.attacker.name;
            }
        }
        currentPhase = Phase.WAITING;
        sendDataToOpp(data);
    }

    private IEnumerator resolveAbilities(Card card, GameObject c){
        if(card.Abilities != null){
            if(isMyTurn){ //resolve for me
                foreach(Effect e in card.Abilities){
                    if(e.GetTargetType() == EffectTarget.SELF){
                        e.execute(ref myState);
                    }else if(e.GetTargetType() == EffectTarget.OPPONENT){
                        e.execute(ref oppState);
                    }else if(e.GetTargetType() == EffectTarget.DRAW){
                        drawCard();
                    }
                }
            }else{ //resolve for opponent
                foreach(Effect e in card.Abilities){
                    if(e.GetTargetType() == EffectTarget.OPPONENT){
                        e.execute(ref myState);
                    }else if(e.GetTargetType() == EffectTarget.SELF){
                        e.execute(ref oppState);
                    }
                }
            }
        }
        yield return new WaitForSeconds(.5f);
        if(card.SpellType == CardType.SPELL){
            
            //destroy card
            if(isMyTurn)
                destroyMyCard(c);
            else
                destroyOppCard(c);
        }
    }


    //send string to opponent
    private void sendDataToOpp(string formatted){
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(formatted);
        Global.stream.Write(data, 0, data.Length);
    }

    //parce data received from opp
    private void receivedDataFromOpp(string formatted){
        string[] firstPass = formatted.Split(':');
        switch (firstPass[0]){
            case "play":
                playOppCard(firstPass[1]);
                break;
            case "recall":
                recallOppCard(firstPass[1]);
                break;
            case "attack":
                if(firstPass.Length > 1)
                    AddOppAttackers(firstPass[1]);
                else
                    AddOppAttackers("");
                break;
            case "block":
                if(firstPass.Length > 1)
                    AddOppBlockers(firstPass[1]);
                else
                    AddOppBlockers("");
                break;
            case "end":
                oppAttack();
                break;
            case "surrender":
                endGame(true);
                break;
            case "my turn":
                isMyTurn = true;
                break;
        }
    }

    private void destroyOppCard(GameObject card){
        //find in oppState.onField
        for(int i = 0; i < oppState.onField.Count;i++){
            if(oppState.onField[i].CardName.Equals(card.name)){
                oppState.onField.RemoveAt(i);
                //TODO death animation
                Destroy(card);
                return;
            }
        }
    }
    private void destroyMyCard(GameObject card){
        for(int i = 0; i < myState.onField.Count;i++){
            if(myState.onField[i].CardName.Equals(card.name)){
                myState.onField.RemoveAt(i);
                //TODO death animation
                Destroy(card);
                return;
            }
        }
    }
    #endregion



    #region Attack & Update Health

    




    // Play card animations when I attack 
    private void myAttack(){
        
        foreach(AttackBlock ab in attackers){
            if(ab.blocker == null && ab.attackCard != null){
                updateOppHealth(ab.attackCard.AttackPower);
            }else if(ab.attackCard != null){
                Card blocker = Library.GetCard(ab.blocker.name);
                if(blocker.DefencePower > ab.attackCard.AttackPower){
                    destroyMyCard(ab.attacker);
                }else if(blocker.DefencePower < ab.attackCard.AttackPower){
                    destroyOppCard(ab.blocker);
                }else if(ab.attackCard.AttackPower != 0){
                    destroyMyCard(ab.attacker);
                    destroyOppCard(ab.blocker);
                }
            }
        }

        attackers.Clear();
        endMyTurn();
    }
    //string format "attacker 1,attacker 2,..."
    public void AddOppAttackers(string attackCards){
        if(attackCards.Equals("")){
            return;
        }
        string[] attackers = attackCards.Split(',');
        foreach(string s in attackers){
            addOppAttacker(s); //add opp attacker
        }
    }
    private void addOppAttacker(string attacker){
        for(int i = 0; i < oppPlayAreaPanel.transform.childCount;i++){
            if(oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Draggable>() != null && //check child is a card placeholder
            !oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Draggable>().isAttacking &&
            oppPlayAreaPanel.transform.GetChild(i).gameObject.name.Equals(attacker)){ //check card is not already set to attacker
                AttackBlock attackBlock; //create blank attacker blocker struct
                attackBlock.attacker = oppPlayAreaPanel.transform.GetChild(i).gameObject; //add teh attacker ui object
                attackBlock.attackCard = Library.GetCard(attacker); //get the attacker card
                attackBlock.blocker = null;
                oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Draggable>().isAttacking = true; //set card to is attacking
                attackers.Add(attackBlock); //add attacker to list
                break;
            }
        }
        currentPhase = Phase.BLOCK;
    }

    public void AddOppBlockers(string blockers){
        if(blockers.Equals("")){
            return;
        }
        string[] blocker = blockers.Split(',');
        foreach(string b in blocker){
            addOppBlocker(b);
        }

        StartCoroutine(checker());

        string data = "end";
        sendDataToOpp(data);
        currentPhase = Phase.END;
        myAttack();
    }

    private void addOppBlocker(string blocker){
        string[] abpair = blocker.Split('-');
            for(int a = 0; a < attackers.Count;a++){
                AttackBlock ab = attackers[a];
                if(ab.attacker.name.Equals(abpair[0]) && ab.blocker == null){
                    //find ui card
                    for(int i = 0; i < oppPlayAreaPanel.transform.childCount;i++){
                        if(oppPlayAreaPanel.transform.GetChild(i).gameObject.name.Equals(abpair[1]) &&
                        !oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Draggable>().isBlocking){
                            oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Draggable>().isBlocking = true;
                            oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.gray;
                            ab.blocker = oppPlayAreaPanel.transform.GetChild(i).gameObject;
                            attackers[a] = ab;
                            return;
                        }
                    }
                }
            }
    }
    private void oppAttack(){
        foreach(AttackBlock ab in attackers){
            if(ab.blocker == null){
                updateMyHealth(ab.attackCard.AttackPower);
            }else{
                Card blocker = Library.GetCard(ab.blocker.name);
                if(blocker.DefencePower < ab.attackCard.AttackPower){
                    destroyMyCard(ab.blocker);
                }else if (blocker.DefencePower > ab.attackCard.AttackPower){
                    destroyOppCard(ab.attacker);
                }else if(ab.attackCard.AttackPower != 0){
                    destroyMyCard(ab.blocker);
                    destroyOppCard(ab.attacker);
                }
            }
        }

        attackers.Clear();
        endOppTurn();
    }

    // After my card attacks opponent
    private void updateOppHealth(float hit){
        oppCurrentHP -= hit; // Decrease attack from HP
        oppHPImage.fillAmount = oppCurrentHP/MAX_HEALTH; // Update opponent's HP on UI

    }

    // After opponent's card attacks me
    private void updateMyHealth(float hit=0){
         // @TODO get attack value from server (@KEVIN M)
        myCurrentHP -= hit; // Decrease attack from HP
        myHPImage.fillAmount = myCurrentHP/MAX_HEALTH; // Update my HP on UI
    }

    private void setOppHealth(int health){
        oppCurrentHP = health;
        oppHPImage.fillAmount = oppCurrentHP/MAX_HEALTH;
    }

    private void setMyHealth(int health){
        myCurrentHP = health;
        myHPImage.fillAmount = myCurrentHP/MAX_HEALTH;
    }
    #endregion

    #region End Turns
    // Check cards in hand
    private const int MAX_HAND_SIZE = 7;
    private bool checkHand(){
        if(myState.inHand.Count > MAX_HAND_SIZE){
            currentPhase = Phase.DISCARD;
            return false;
        }
        return true;
    }

    private IEnumerator checker(){
            while(!checkHand()){
                yield return new WaitForSeconds(1);
            }
    }

    public void DiscardCard(GameObject card){
        for(int i = 0; i < myState.inHand.Count;i++){
            if(card.name.Equals(myState.inHand[i].CardName)){
                myState.inHand.RemoveAt(i);
                break;
            }
        }
        Destroy(card);
    }

    // End my turn
    private void endMyTurn(){
        isMyTurn = false; // No longer my turn
        string endString = "YOUR TURN"; // Send this to server
        currentPhase = Phase.WAITING; //wait for opp move
        currentTurn++;
    }


    // End opponent's play
    private void endOppTurn(){
        isMyTurn = true; // Now it's my turn
        currentPhase = Phase.MAIN; //start my turn
        myState.mana = myState.mana + currentTurn /8 + 1; //increase mana
        currentTurn++;
        drawCard();
    }
    #endregion

    #region End Game
    // End game
    private void endGame(bool iWin){
        // Calculate credits
        // Change scene
        if(iWin){
            PlayerPrefs.SetString(WON_PREF_KEY, "you");
        }
        else{
            PlayerPrefs.SetString(WON_PREF_KEY, "opp");
        }
        //PlayerPrefs.SetString(OPP_PROFILE_PREF_KEY, );
        SceneManager.LoadScene("EndDuel");
    }
    #endregion
}