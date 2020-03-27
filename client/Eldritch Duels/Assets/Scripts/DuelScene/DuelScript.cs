using Newtonsoft.Json;
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

    #endregion

    #region Awake
    // Awake is called when the script instance is being loaded.
    void Awake(){
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
        yield return (StartCoroutine(testPlays()));
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
            Card b = Library.GetCard("Test 0");
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
            Card b = Library.GetCard("Test 0");
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
        playOppCard("Test 0");
        playOppCard("Test 0");
        yield return new WaitForSeconds(2);
        foreach(Transform t in oppPlayAreaPanel.transform){
            addOppAttacker(t.gameObject.name);
        }
        

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

    // Play card from my hand to my playing area
    public bool playMyCard(string cardName){
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

                //TODO resolve abilities

                return true;
                

                return true;
            }
        }

        return false;
    }

    public bool recallCard(string cardName){
        if(myState.onField.Count >= DuelFunctions.MAX_FIELD || true){
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
            return true;
            foreach(Transform c in myPlayAreaPanel.transform){
                if(c.gameObject.name.Equals(cardName)){
                    Destroy(c.gameObject);
                    GameObject cnew = (GameObject)Instantiate(myCard);
                    cnew.GetComponent<Image>().sprite = null;
                    cnew.GetComponent<Image>().material = recalled.CardImage;
                    cnew.name = recalled.CardName;
                    cnew.transform.SetParent(handAreaPanel.transform, false); // Add card to my play area
                    handList.Add(cnew); // Add card to my play list
                    break;
                }
            }
            for(int i = 0; i< myPlayList.Count;i++){
                if(myPlayList[i].name.Equals(cardName)){
                    myPlayList.RemoveAt(i);
                    break;
                }
            }
        }
        return true;
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

        //resolve abilities
        
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

    }

    //format and sent attacker to opponent
    private void confirmAttackers(){

    }



    private void sendDataToOpp(string formatted){
        //TODO

    }

    private void recievedDataFromOpp(string formatted){
        
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
                    //DESTROY myCard
                }else if(blocker.DefencePower < ab.attackCard.AttackPower){
                    //DESTORY OPP CARD
                }else{
                    //DESTROY BOTH CARDS
                }
            }
        }

        attackers.Clear();
    }
    //string format "attacker 1,attacker 2,..."
    public void AddOppAttackers(string attackCards){
        string[] attackers = attackCards.Split(',');
        foreach(string s in attackers){
            addOppAttacker(s); //add opp attacker
        }
    }
    private void addOppAttacker(string attacker){
        for(int i = 0; i < oppPlayAreaPanel.transform.childCount;i++){
            if(oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Draggable>() != null && //check child is a card placeholder
            !oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Draggable>().isAttacking){ //check card is not already set to attacker
                AttackBlock attackBlock; //create blank attacker blocker struct
                attackBlock.attacker = oppPlayAreaPanel.transform.GetChild(i).gameObject; //add teh attacker ui object
                attackBlock.attackCard = Library.GetCard(attacker); //get the attacker card
                attackBlock.blocker = null;
                oppPlayAreaPanel.transform.GetChild(i).gameObject.GetComponent<Draggable>().isAttacking = true; //set card to is attacking
                attackers.Add(attackBlock); //add attacker to list
                break;
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
                    //DESTROY BLOCKER
                }else if (blocker.DefencePower > ab.attackCard.AttackPower){
                    //DESTROY ATTACKER
                }else{
                    //DESTROY BOTH
                }
            }
        }

        attackers.Clear();
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
    #endregion

    #region End Turns
    // Check cards in hand
    private void checkHand(){

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
    }
    #endregion

    #region End Game
    // End game
    private void endGame(){
        
    }
    #endregion
}