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
using System.Linq;

public struct PlayerState{
    public int mana;
    public int hp;
    public List<Card> library;
    public List<Card> inHand;
    public List<Card> onField;
    
}

public struct DuelRequest {
    public string cardName;     // Name of the played card

    public string targetName;   // Name of the target card

    public int myArrPosition;   // Position in the array of my card I've played

    public string targetArrPosition; // Position in the array of the target card as well as whose array it's in 
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

    private const string[] needTargeter = { "Destroy", "Add defender", "Add fly", "Add stealth" };

    private const int MAX_HAND = 8; //max hand size
    private const int MAX_FIELD_SIZE = 7;

    public PlayerState OpponentState; //opp hp, mana

    public PlayerState MyState; //my hp, mana, library

    static bool done;
    static readonly object locker = new object();

    #endregion

    #region Awake
    // Awake is called when the script instance is being loaded.
    void Start(){
        setUpDeck(); // Set up card list from deck being used
        setUpProfilePics(); // Set up profile pics for both users
        setUpHealthMana(); // Set up health & mana to full for both users
        StartCoroutine(initCoroutines());
        System.Threading.Thread T = new System.Threading.Thread((new System.Threading.ThreadStart(Listener)));
        T.Start();
    }

    /* Need to spawn a separate thread to listen to the socket so that
       the client can still interact with the game while the other player is taking their turn.
       Need to kill the thread when the match dies. */
    private void Listener() {
        readStreamAsync();   
        
    }

    public static async Task readStreamAsync() {
        while (true) {
            Byte[] data = new Byte[256];
            int read_bytes = await Global.stream.ReadAsync(data, 0, 256);
            string trimmed = System.Text.Encoding.ASCII.GetString(data).Trim();
            DuelRequest dreq = JsonConvert.DeserializeObject(trimmed);
            playOppCard(dreq);
        }
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

        /* Do I have enough mana */
        if(!DuelFunctions.CanCast(played, MyState)){
            return;
        }

        /* Is my field full */
        if(MyState.onField.Count >= MAX_FIELD_SIZE){
            return;
        }

        Card targetCard = null;

        /* If any of my abilities require me to target something, bring up
           a targeter when I go to resolve that and then break. Currently,
           this only will work if one of the abilities requires a target
           but that can be changed once we confirm it works */
        for (int i = 0; i < played.Abilities.Count; i++) {
            string ability = played.Abilities[i].GetName();
            if (needTargeter.Contains(ability)) {
                // TODO: CREATE TARGETER @DHAIRYA
                targetCard = null;
            }
        }

        Card b = DuelFunctions.RemoveFromHand(played, ref MyState);     // Remove card from my hand
        GameObject c = (GameObject)Instantiate(card);
        DuelRequest duelRequest = new DuelRequest();        // Instantiate struct that will be sent to other player
        duelRequest.cardName = played.CardName;
        duelRequest.targetName = "";
        duelRequest.targetArrPosition = "";
        duelRequest.myArrPosition = -1;
        if (targetCard != null) {
            duelRequest.targetName = targetCard.CardName;

            // TODO: GET THE ARRAY POSITION OF THE TARGETED CREATURE
            // Example: "me-3"
            // Example: "opponent-2"
            duelRequest.targetArrPosition = "";
        }

        // TODO: GET THE ARRAY POSITION OF MY NEWLY PLACED CREATURE
        // Example: 4
        duelRequest.myArrPosition = -1;

        c.GetComponent<Image>().sprite = null;
        c.GetComponent<Image>().material = played.CardImage;
        if(played.SpellType == CardType.SPELL){
           ResolveAbilities(played, true, duelRequest, targetCard);
        }else{
            MyState.onField.Add(b);
            myPlayList.Add(c);
            ResolveAbilities(played, true, duelRequest, targetCard);
        }
        MyState.mana -= played.CardCost;

        //TODO update ui

        //TODO tell oppenent to resolve card
    }

    /* private void playMyCard(GameObject played){
        if(!DuelFunctions.CanCast(played, MyState)){
            return;
        }
        if(MyState.onField.Count >= MAX_FIELD_SIZE){
            return;
        }

        Card b = DuelFunctions.RemoveFromHand((Card) played, ref MyState);
        GameObject c = (GameObject)Instantiate(card);
        c.GetComponent<Image>().sprite = null;
        c.GetComponent<Image>().material = played.CardImage;
        DuelRequest myMove = new DuelRequest();
        myMove.cardName = played.CardName;
        myMove.myArrPosition = -1; // TODO: Get the array position of my card @DHAIRYA
        if (played.SpellType != CardType.SPELL) {
            MyState.onField.Add(b);
            myPlayList.Add(c);
        }

        MyState.mana -= played.CardCost;

        //TODO update ui

        //TODO tell oppenent to resolve card
    } */

    //server sends request that opp played a card
    //this method ties the server request with the
    //client side
/*    public void ServerPlayOppCard(DuelRequest req){
        Card toPlay = Library.GetCard(cardName);
        playOppCard(toPlay);
    }

*/
    // Play opponent's card
    /*private void playOppCard(Card played){
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
    }*/

    private void playOppCard(DuelRequest req) {
        Card toPlay = Library.GetCard(cardName);
        if (toPlay.SpellType != CardType.SPELL) {
            OpponentState.onField.Add(toPlay);
        }
        GameObject c = (GameObject)Instantiate(card);
        c.GetComponent<Image>().sprite = null;
        c.GetComponent<Image>().material = toPlay.CardImage;
        Card targetCard = null;
        if (req.targetName != null) {
            string[] split = req.targetName.Split('-');
            if (String.Equals(split[0], "mine")) {
                targetCard = (Card) oppPlayAreaPanel[Int32.Parse(split[1])];
            } else {
                targetCard = (Card) myPlayAreaPanel[Int32.Parse(split[1])];
            }
        }
        ResolveAbilities(toPlay, false, req, targetCard);
    }


    private void ResolveAbilities(Card played, bool iPlayed, DuelRequest selected, Card targetCard){
        foreach(Effect e in played.Abilities){
            switch(e.GetTargetType()){
                case EffectTarget.OPPONENT:
                    e.execute(ref OpponentState);
                    break;
                case EffectTarget.SELF:
                    e.execute(ref MyState);
                    break;
                case EffectTarget.CARD:
                    if (targetCard != null) {
                        e.execute(targetCard);
                    } else {
                        e.execute(played);
                    }
                    //TODO select card and execute if
                    break;
                default:
                    break;

            }
            if (iPlayed) {
                string json = JsonConvert.SerializeObject(selected);
                sendNetworkRequest(json);
            }
            
        }
    }

    public string sendNetworkRequest(string obj) {
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(obj);
        Global.stream.Write(data, 0, data.Length);
    }

    #endregion

    #region Attack & Update Health
    // Play card animations when I attack 
    /* private void myAttack(){
        foreach(GameObject value in myPlayList){
            // @TODO some attack animations (@BRANDON)
            float hit=0; // @TODO get attack value of the card (@DHAIRYA)
            float defense = 0; // @TODO get defense value of targeted card (@DHAIRYA)
            if (hit >= defense) {
                destroyMinion(value);
            }
            // @TODO send attack value to server (@KEVIN M)
            updateOppHealth(hit); // After each card's attack, update opponent's health
        }
    } */

    private void myAttack(GameObject attacker, GameObject defender) {
        float attack = 0; // TODO: GET attacker ATTACK @Dhairya
        float defense = 0; // TODO: GET defender DEFENSE @Dhairya

        if (attack >= defense) {
            DuelFunctions.destroyMinion(defender);
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
