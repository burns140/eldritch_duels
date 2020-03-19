using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using eldritch.cards;

public class DuelScript : MonoBehaviour 
{
    // Cards in the deck UI
    public GameObject cardBack1; 
    public GameObject cardBack2;
    public GameObject cardBack3;
    public GameObject cardBack4;

    public Text gameText;
    private bool isMyTurn; // Check if it is my turn

    public GameObject handAreaPanel; // Hand Area UI
    public GameObject myPlayAreaPanel; // My Play Area UI
    public GameObject oppPlayAreaPanel; // Opponent's Play Area UI

    public GameObject card; // Card object to create instances

    private Queue<GameObject> deckList = new Queue<GameObject>(); // Store cards in Deck
    private List<GameObject> handList = new List<GameObject>(); // Store cards in Hand
    private List<GameObject> myPlayList = new List<GameObject>(); // Store cards in my Play Area
    private List<GameObject> oppPlayList = new List<GameObject>(); // Store cards in opponent Play Area

    public Button recallButton;

    // Awake is called when the script instance is being loaded.
    void Awake(){
        setUpDeck(); // Set up card list from deck being used
        StartCoroutine(initCoroutines());
    }

    // Update is called once per frame
    void Update()
    {
        checkDeckCount(); // Check & update card back quantity on deck UI
    }

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

    IEnumerator initCoroutines(){
        yield return StartCoroutine(initalDraw()); // Set up hand to have 6 cards
        yield return StartCoroutine(testPlayArea()); // Test moving cards from hand to my play area
        yield return StartCoroutine(testOppArea()); // Test add cards to opponent play area
    }

    // Called at start of game to fill hand to 6 cards from deck
    IEnumerator initalDraw(){
        int handCount=1;
        while(handCount<=6){ // To add 6 cards to hand
            Card b = Library.GetCard("Test 0");
            GameObject c = (GameObject)Instantiate(card);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            c.name = b.CardName;
            c.transform.SetParent(handAreaPanel.transform, false); // Add card to hand
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
        int deckCount=1;
        while(deckCount<=30){ // To add 6 cards to hand
            Card b = Library.GetCard("Test 0");
            GameObject c = (GameObject)Instantiate(card);
            c.GetComponent<Image>().sprite = null;
            c.GetComponent<Image>().material = b.CardImage;
            deckList.Enqueue(c); // Add card to deck list
            deckCount++; 
        }
    }

    // Called at the beginning of every turn
    private void drawCard(){
        if(deckList.Count>0){
            handList.Add(deckList.Dequeue()); // Move 1 card from deck to hand
        }
    }

    // Play card from my hand to my playing area
    private void playMyCard(){
        
    }

    // Check cards in hand
    private void checkHand(){

    }

    // End my turn
    private void EndMyTurn(){

        isMyTurn = false; // No longer my turn
    }

    // Play opponent's card
    private void playOppCard(){
        
    }
    
}
