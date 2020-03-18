using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SearchFriendsScript : MonoBehaviour
{
    public GameObject lastPlayedPanel; // Last played users Panel in UI
    public GameObject searchPanel; // Search users result panel in UI
    public GameObject inputField; // Search input field in UI
    public GameObject searchButton; // Search button in UI
    public GameObject buttonPrefab; // Button prefab in UI
    private List<string> searchPlayersList = new List<string>(); // To store all searched users
    private List<string> lastPlayedList = new List<string>(); // To store last 3 opponents played

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // @TODO Get the 3 last played users from server
        string user1 = "user1"; // temp users
        string user2 = "user2";
        string user3 = "user3";

        // Add 3 last played users to lastPlayedList for displaying on frontend
        lastPlayedList.Add(user1);
        lastPlayedList.Add(user2);
        lastPlayedList.Add(user3); 

        // Add temporary users to search results list
        searchPlayersList.Add("Hola");
        searchPlayersList.Add("Amigo");
        searchPlayersList.Add("Que Paso");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");
        searchPlayersList.Add("HolaAmigo");

        // Set buttons to show those players on frontend
        GameObject userButton1 = (GameObject)Instantiate(buttonPrefab);
        userButton1.GetComponentInChildren<Text>().text = user1;
        GameObject userButton2 = (GameObject)Instantiate(buttonPrefab);
        userButton2.GetComponentInChildren<Text>().text = user2;
        GameObject userButton3 = (GameObject)Instantiate(buttonPrefab);
        userButton3.GetComponentInChildren<Text>().text = user3;

        // Add players buttons to the panel in UI
        userButton1.transform.SetParent(lastPlayedPanel.transform, false); 
        userButton2.transform.SetParent(lastPlayedPanel.transform, false); 
        userButton3.transform.SetParent(lastPlayedPanel.transform, false); 

    }
    
    // Search button onClick() listener to display search results
    public void searchUser(){
        Button[] gameObjects = searchPanel.GetComponentsInChildren<Button>(); // Get previous search results
        foreach(Button o in gameObjects){ 
            Destroy(o.gameObject); // Destroy all previous search results
        }
        
        string search = inputField.GetComponentInChildren<Text>().text; // Get user input text

        foreach(string value in searchPlayersList){
            if(value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0){
                GameObject searched = (GameObject)Instantiate(buttonPrefab); // Create search result button
                searched.GetComponentInChildren<Text>().text = value; // Set text to the searched username 
                searched.transform.SetParent(searchPanel.transform, false); // Add username buttons to search panel
            }
        }
    }

    // Start is called before the first frame update
    //void Start() {}

    // Update is called once per frame
    //void Update(){}
}
