using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using eldritch;
using UnityEngine.SceneManagement;

public class SearchFriendsScript : MonoBehaviour
{
    #region UI & script variables
    public GameObject lastPlayedPanel; // Last played users Panel in UI
    public GameObject searchPanel; // Search users result panel in UI
    public GameObject inputField; // Search input field in UI
    public GameObject searchButton; // Search button in UI
    public GameObject buttonPrefab; // Button prefab in UI
    
    public Text ErrorText; // Show error text

    private List<string> searchPlayersList = new List<string>(); // To store all searched users
    private List<string> lastPlayedList = new List<string>(); // To store last 3 opponents played
    
    private const string EMAIL_PREF_KEY = "email"; // EMAIL PREF KEY to store email of user
    
    #endregion

    List<string> usernames;
    List<string> emails;

    public class getAllFriendsRequest {
        public string id;
        public string token;
        public string cmd;

        public getAllFriendsRequest(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    void Awake() // Awake is called when the script instance is being loaded
    {
        getAllFriendsRequest req = new getAllFriendsRequest(Global.getID(), Global.getToken(), "getAllUsernames");
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[1024];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

        string[] pairs = responseData.Split(',');
        usernames = new List<string>();            // List with all usernames
        emails = new List<string>();

        foreach (string str in pairs) {
            string[] user_email = str.Split('-');
            usernames.Add(user_email[0]);
            emails.Add(user_email[1]);
        }

        // @TODO Get the 3 last played users from server
        string user1 = "user1"; // temp users
        string user2 = "user2";
        string user3 = "user3";

        // Add 3 last played users to lastPlayedList for displaying on frontend
        lastPlayedList.Add(user1);
        lastPlayedList.Add(user2);
        lastPlayedList.Add(user3); 

        // Add temporary users to search results list
        /*searchPlayersList.Add("Hola");
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
        searchPlayersList.Add("HolaAmigo");*/

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
      
    public void searchUser(){ // Search button onClick() listener to display search results
        Button[] gameObjects = searchPanel.GetComponentsInChildren<Button>(); // Get previous search results
        foreach(Button o in gameObjects){ 
            Destroy(o.gameObject); // Destroy all previous search results
        }
        
        string search = inputField.GetComponentInChildren<Text>().text; // Get user input text
        if(search.Equals("")){
            StartCoroutine(showError());
        }
        else{
            string myUsername = Global.username; // store my username here
            /*if(searchPlayersList.Contains(myUsername)){ 
                searchPlayersList.Remove(myUsername); // do not display my username in the search
            }*/
            /*foreach(string value in searchPlayersList){
                if(value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0){
                    GameObject searched = (GameObject)Instantiate(buttonPrefab); // Create search result button
                    searched.GetComponentInChildren<Text>().text = value; // Set text to the searched username 
                    searched.SetActive(true);
                    searched.transform.SetParent(searchPanel.transform, false); // Add username buttons to search panel
                }
            }*/
            foreach(string value in usernames){
                if(value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0){
                    if(value == myUsername){
                        continue;// do not display my username in the search
                    }
                    GameObject searched = (GameObject)Instantiate(buttonPrefab); // Create search result button
                    searched.GetComponentInChildren<Text>().text = value; // Set text to the searched username 
                    searched.SetActive(true);
                    searched.transform.SetParent(searchPanel.transform, false); // Add username buttons to search panel
                }
            }
        }
    }

    IEnumerator showError(){ // set up error message
        ErrorText.text = "*please input something";
        ErrorText.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(3f); 
        ErrorText.GetComponent<Text>().enabled = false;
    }

    public void buttonClicked(Button btn){ // clicked on a user
        Debug.Log(btn.GetComponentInChildren<Text>().text);
        string username = btn.GetComponentInChildren<Text>().text;
        string email = "";
        Debug.Log("Search clicked on: "+username);

        int i = 0;

        for (; i < usernames.Count; i++) {
            if (usernames[i].Equals(username)) {
                email = emails[i];
                Debug.Log("Sending email: "+email);
                break;
            }
        }
        // for(int j=0; j<usernames.Count; j++){
        //     Debug.Log("username "+j+":"+usernames[j]);
        // }
        // for(int k=0; k<emails.Count; k++){
        //     Debug.Log("email "+k+":"+emails[k]);
        // }

        PlayerPrefs.SetString(EMAIL_PREF_KEY,email); // Save the clicked email to EMAIL PREF KEY
        loadUserProfile(); // Go to the user's profile page scene
    }

    private void loadUserProfile(){ // load the clicked user's profile
        // this will just change scene to profile scene
        SceneManager.LoadScene("ProfileScene");
    }

    public void goBack(){ // Load Lobby Scene
        SceneManager.LoadScene("Lobby");    
    }

    // Start is called before the first frame update
    //void Start() {}

    // Update is called once per frame
    //void Update(){}
}
