using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckPanelScript : MonoBehaviour
{
    public int deckSizeTest = 30; // Test deck size

    // Card Objects in the deck UI
    public GameObject cardInDeck1; 
    public GameObject cardInDeck2;
    public GameObject cardInDeck3;
    public GameObject cardInDeck4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(deckSizeTest<30){
            cardInDeck1.SetActive(false); // Hide first card in deck
        }
        if(deckSizeTest<20){
            cardInDeck2.SetActive(false); // Hide second card in deck
        }
        if(deckSizeTest<10){
            cardInDeck3.SetActive(false); // Hide third card in deck
        }
        if(deckSizeTest<1){
            cardInDeck4.SetActive(false); // Hide fourth/last card in deck
        }
    }
}
