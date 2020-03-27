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
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ProfileScript : MonoBehaviour
{
    #region UI & Script Variables
    public Sprite[] pictures; // List of available pictures
    public Text bio; // bio text on the UI
    private string bioText; // store bio text from server

    public Image profilePic; // profile pic on the UI
    public Text username; // username text on the UI
    private string usernameText; // store username text from server

    public GameObject AddButton; // add/remove friend button on UI
    private bool alreadyFriend=false; // boolean to check if user is already a friend
    private bool alreadySentRequest=false; // boolean to check if I have already sent a friend request to the user
    private bool alreadyReceivedRequest=false; // boolean to check if I have already received a friend request from the user
    public GameObject BlockButton; // block user Button on UI
    private bool alreadyBlocked=false; // boolean to check if user is blocked by you
    public GameObject ReportButton; // report user button on UI
    private bool alreadyReported=false; // boolean to check if user is reported by you

    public GameObject buttonPanel; // Panel with all buttons on UI

    public Button GoBackButton; // go back button
    public GameObject EditProfileButton; // edit profile button
    public GameObject FriendsButton; // button to show friends list
    public GameObject FriendRequestsButton; // button to show friend requests
    private bool isMe=false; // True if it's my profile
    private bool hasPicIndex=false; // Check if picture was from default pictures
    
    public GameObject friendButtonPrefab; // Button prefab for friends buttons in UI
    public GameObject requestButtonPrefab; // Button prefab for friend requests buttons in UI
    public GameObject friendsPanelHolder; // hide/unhide holder of friendsPanel
    public GameObject friendsPanel; // panel to display friends list in UI
    public GameObject requestsPanelHolder; // hide/unhide holder of requestsPanel
    public GameObject requestsPanel; // panel to display friend requests list in UI
    public GameObject handleRequestPanel; // panel to handle a request in UI
    public GameObject userRequestButton; // button to go to profile of user who sent the request
    public GameObject acceptRequestButton; // button to accept request in UI
    public GameObject rejectRequestButton; // button to reject request in UI
    public GameObject cancelRequestButton; // button to hide handle request panel in UI 
    public Text ErrorText; // Display error message on UI
    private const string EMAIL_PREF_KEY = "email"; // EMAIL PREF KEY to store user email
    #endregion

    public class getBlockedRequest {
        public string id;
        public string token;
        public string cmd;

        public getBlockedRequest(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    public class genericRequest {
        string id;
        string token;
        string cmd;

        public genericRequest(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    public class getProfileRequest {
        public string theirEmail;
        public string token;
        public string cmd;

        public getProfileRequest(string email, string token, string cmd) {
            this.theirEmail = email;
            this.token = token;
            this.cmd = cmd;
        }
    }

    public class FriendsRequest {
        public string myEmail;
        public string theirEmail;
        public string token;
        public string cmd;

        public FriendsRequest(string myEmail, string theirEmail, string token, string cmd) {
            this.myEmail = myEmail;
            this.theirEmail = theirEmail;
            this.token = token;
            this.cmd = cmd;
        }
    }

    string email;
    string returnedUsername;
    string returnedBio;
    string[] friends;
    List<string> friendList;
    List<string> myBlockedUsers;

    void Awake() // Awake is called when the script instance is being loaded

    {
        email = PlayerPrefs.GetString(EMAIL_PREF_KEY); // Get the user email from PLAYER PREFS;
        isItMe();
        if(isMe){
            EditProfileButton.SetActive(true);
            FriendsButton.SetActive(true);
            FriendRequestsButton.SetActive(true);
        }
        if(!getBlockedMe()){ // Hide profile if I am blocked by user
            displayPic();
            displayBio();
            displayScreenName(); 
            setAddButton();
            setReportButton();
            setBlockButton();
        }
    }

    private void isItMe(){ // check if it's my profile
        // @TODO Check if it's me (@STEPHEN/@KEVING)
        if (email.Equals(Global.getEmail())) {
            isMe = true; // If it's my account 
        } else {
            isMe = false;
        }
        // isMe = false; // If it's not my account
    }

    private bool getBlockedMe(){ // check if I the user has blocked me
        // @TODO Check if I am blocked by this user
        getBlockedRequest req = new getBlockedRequest(Global.getID(), Global.getToken(), "getBlockedByUsers");
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[1024];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

        List<string> blockedByUsersEmails = new List<string>();
        blockedByUsersEmails = responseData.Split(',').ToList();           // array of emails of users who've blocked me

        // @TODO Let me know if server didn't work as expected @STEPHEN
        bool failed = false;
        if(failed){
            StartCoroutine(showError("Could not retreive all info, please go back")); // set error message
            return true; // just so that nothing loads
        }
        else{
            if (blockedByUsersEmails.Contains(email)) {             // I'm blocked by them
                // TODO
                Button[] gameObjects = buttonPanel.GetComponentsInChildren<Button>(); // Get buttons from panel
                foreach(Button o in gameObjects){ 
                    Destroy(o.gameObject); // Destroy buttons on UI
                }
                ErrorText.text = "You have been blocked by this user"; // set error message
                ErrorText.GetComponent<Text>().enabled = true; 
                return true;
            }
            
            return false;
        } 
    }

    private void displayPic(){ // get profile pic & display it
        hasPicIndex = true; // Check if my pic is uploaded or has index (@KEVING)
        // @TODO Let me know if server didn't work as expected @KEVING
        bool failed = false;
        if(failed){
            StartCoroutine(showError("Could not retreive all info")); // set error message
        }
        else{
            if(hasPicIndex){
                int picIndex; // Store pic index
                if(isMe){ 
                    picIndex = Global.avatar; // get my pic index 
                    profilePic.GetComponent<Image>().sprite = pictures[picIndex]; // Set profile pic on UI
                }
                else{
                    picIndex = 0; // get user's pic index @STEPHEN
                    // @TODO Let me know if server didn't work as expected @STEPHEN
                    bool userIndexFailed = false;
                    if(userIndexFailed){
                        StartCoroutine(showError("Could not retreive user's profile pic")); // set error message
                    }
                    else{
                        profilePic.GetComponent<Image>().sprite = pictures[picIndex]; // Set profile pic on UI
                    }
                }
            }
            else{ // The picture was uploaded
                Sprite newImage;
                if(isMe){
                    // @TODO GET UPLOADED PICTURE FROM SERVER (@KEVING)
                    // newImage = ;
                    // @TODO Let me know if server didn't work as expected @KEVIN
                    bool newImgFailed = false;
                    if(newImgFailed){
                        StartCoroutine(showError("Could not retreive my profile pic")); // set error message
                    }
                    else{
                        //profilePic.GetComponent<Image>().sprite = newImage;
                    }
                }
                else{
                    // @TODO GET UPLOADED PICTURE FROM SERVER (@KEVING)
                    // newImage = ;
                    // @TODO Let me know if server didn't work as expected @KEVIN
                    bool userImgFailed = false;
                    if(userImgFailed){
                        StartCoroutine(showError("Could not retreive my profile pic")); // set error message
                    }
                    else{
                        //profilePic.GetComponent<Image>().sprite = newImage;
                    }
                }
            }
        }
    }

    private void getInfo() {
        getProfileRequest req = new getProfileRequest(email, Global.getToken(), "viewProfile");
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[1024];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

        string[] info = responseData.Split(',');
        returnedBio = info[1].Split('-')[1];
        returnedUsername = info[2].Split('-')[1];
    }

    private void displayBio(){ // get bio & display it
        getInfo();
        if(isMe){
            // @TODO Get my bio from server @STEPHEN/@KEVING
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool myBioFailed = false;
            if(myBioFailed){
                StartCoroutine(showError("Could not retreive my bio")); // set error message
            }
            else{
                bioText =  returnedBio; // Get my bio from server @STEPHEN/@KEVING
            }
        }
        else{
            // @TODO Get user's bio from server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool userBioFailed = false;
            if(userBioFailed){
                StartCoroutine(showError("Could not retreive user's bio")); // set error message
            }
            else{
                bioText =  returnedBio; // Get user's bio from server @STEPHEN
            }
        }

        bio.text = bioText; // Set bio on UI
    }

    private void displayScreenName(){ // get username & display it
        getInfo();

        if(isMe){
            // @TODO Get my username from server @STEPHEN/@KEVING
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool myUsernameFailed = false;
            if(myUsernameFailed){
                StartCoroutine(showError("Could not retreive my username")); // set error message
            }
            else{
                usernameText = returnedUsername; 
            }
        }
        else{
            // @TODO Get user's username from server @STEPHEN/@KEVING
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool userUsernameFailed = false;
            if(userUsernameFailed){
                StartCoroutine(showError("Could not retreive user's username")); // set error message
            }
            else{
                usernameText = returnedUsername;
            }
        }

        username.text = usernameText; // Set username on UI
    }

    private void setAddButton(){ // set up the add friend/unfriend button

        if(isMe){
            AddButton.SetActive(false); // cannot add myself as friend
        }
        else{
            // @TODO Get from server if user is already my friend @STEPHEN

            genericRequest req = new genericRequest(Global.getID(), Global.getToken(), "getAllFriends");
            string json = JsonConvert.SerializeObject(req);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);
            data = new Byte[1024];
            string responseData = string.Empty;
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            friends = responseData.Split(',');          // Array of friend emails
            friendList = friends.ToList();
            if (friendList.Contains(email)) {
                alreadyFriend = true;
            } else {
                alreadyFriend = false;
            }


            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not retreive if user is your friend")); // set error message
                AddButton.GetComponentInChildren<Button>().interactable = false; // couldn't retrieve info so disable button
            }
            else{
                if(alreadyFriend){
                    AddButton.GetComponentInChildren<Text>().text = "Unfriend"; // set button text
                    AddButton.GetComponentInChildren<Button>().interactable = true;
                }
                else{
                    // @TODO Get from server if user has sent me friend request @STEPHEN
                    req = new genericRequest(Global.getID(), Global.getToken(), "getFriendRequests");
                    json = JsonConvert.SerializeObject(req);
                    data = System.Text.Encoding.ASCII.GetBytes(json);
                    Global.stream.Write(data, 0, data.Length);
                    data = new Byte[1024];
                    responseData = string.Empty;
                    bytes = Global.stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                    List<string> friendRequestList = responseData.Split(',').ToList();
                    if (friendRequestList.Contains(email)) {
                        alreadyReceivedRequest = true; // Store if the user has sent me friend request
                    } else {
                        alreadyReceivedRequest = false;
                    }

                    if (alreadyReceivedRequest){
                        AddButton.GetComponentInChildren<Text>().text = "Handle Request"; // set button text
                        AddButton.GetComponentInChildren<Button>().interactable = true;
                    }
                    else{
                        // @TODO Get from server if I have sent a friend request to the user @STEPHEN
                        req = new genericRequest(Global.getID(), Global.getToken(), "getFriendRequestsSent");
                        json = JsonConvert.SerializeObject(req);
                        data = System.Text.Encoding.ASCII.GetBytes(json);
                        Global.stream.Write(data, 0, data.Length);
                        data = new Byte[1024];
                        responseData = string.Empty;
                        bytes = Global.stream.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                        List<string> friendRequestSentList = responseData.Split(',').ToList();

                        if (friendRequestSentList.Contains(email)) {
                            alreadySentRequest = true; // Store if I sent the user a friend request
                        } else {
                            alreadySentRequest = false;
                        }
                        if (alreadySentRequest){
                            AddButton.GetComponentInChildren<Text>().text = "Request Sent"; // set button text
                            AddButton.GetComponentInChildren<Button>().interactable = false;
                        }
                    }
                }
            }
        }
    }

    public void handleAddFriend(){ // friend/unfriend/handleRequest button
        if(alreadyFriend){ // unfriend user
            // @TODO Unfriend user through server @STEPHEN

            FriendsRequest req = new FriendsRequest(Global.getEmail(), email, Global.getToken(), "removeFriend");
            string json = JsonConvert.SerializeObject(req);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);
            data = new Byte[1024];
            string responseData = string.Empty;
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            friendList.Remove(email);
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool unFriendFailed = false;
            if(unFriendFailed){
                StartCoroutine(showError("Could not unfriend user, please try again")); // set error message
            }
            else{
                AddButton.GetComponentInChildren<Text>().text = "Add Friend"; // set button text
                AddButton.GetComponentInChildren<Button>().interactable = true;
                alreadyFriend = false;  // since we unfriended the user, set alreadyFriend to false
                alreadySentRequest = false; // just to make sure
                alreadyReceivedRequest = false; // just to make sure
            }
        }
        else{ // user is not a friend
            if(alreadyReceivedRequest){ // The user has sent me a friend request
                handleRequestPanel.SetActive(true); // unhide handle request panel
                userRequestButton.SetActive(false); // hide username button
            }
            else{ // sent a friend reqiest to the user
                // @TODO Add user as friend through server @STEPHEN

                FriendsRequest req = new FriendsRequest(Global.getEmail(), email, Global.getToken(), "sendFriendRequest");
                string json = JsonConvert.SerializeObject(req);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                Global.stream.Write(data, 0, data.Length);
                data = new Byte[1024];
                string responseData = string.Empty;
                Int32 bytes = Global.stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                // @TODO Let me know if server didn't work as expected @STEPHEN
                bool friendFailed = false;
                if(friendFailed){
                    StartCoroutine(showError("Could not add user as friend, please try again")); // set error message
                }
                else {
                    AddButton.GetComponentInChildren<Text>().text = "Unfriend"; // set button text
                    AddButton.GetComponentInChildren<Button>().interactable = true;
                    alreadyFriend = true; // since we added user as friend, set alreadyFriend to true
                    alreadySentRequest = false; // just to be sure
                    alreadyReceivedRequest = false; // just to be sure
                }
            } 
        }
    }

    private void setBlockButton(){ // set up the block/unblock button
        
        if(isMe){
            BlockButton.SetActive(false); // cannot block myself
        }
        else{
            // @TODO Get from server if I have already blocked the user

            genericRequest req = new genericRequest(Global.getID(), Global.getToken(), "getBlockedUsers");
            string json = JsonConvert.SerializeObject(req);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);
            data = new Byte[1024];
            string responseData = string.Empty;
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            myBlockedUsers = responseData.Split(',').ToList();

            if (myBlockedUsers.Contains(email)) {
                alreadyBlocked = true;
            } else {
                alreadyBlocked = false; // Check if the user is blocked by me from server
            }
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not retreive if you have blocked the user")); // set error message
                BlockButton.GetComponentInChildren<Button>().interactable = false; // couldn't retrieve info so disable button
            }
            else{
                if(alreadyBlocked){
                    BlockButton.GetComponentInChildren<Text>().text = "Unblock"; // set button text
                }
            }
        }
        
    }

    public class BlockRequest {
        public string id;
        public string token;
        public string cmd;
        public string myEmail;
        public string toBlockEmail;

        public BlockRequest(string id, string token, string cmd, string myEmail, string toBlockEmail) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
            this.myEmail = myEmail;
            this.toBlockEmail = toBlockEmail;
        }
    }

    public void handleBlock(){ // block/unblock user & handle button
        
        if(alreadyBlocked){ // unblock user
            // @TODO Unblock user through server @STEPHEN

            BlockRequest req = new BlockRequest(Global.getID(), Global.getToken(), "unblockUser", Global.getEmail(), email);
            string json = JsonConvert.SerializeObject(req);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);
            data = new Byte[1024];
            string responseData = string.Empty;
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            bool unBlockFailed;

            if (responseData.Equals("failed to unblock user") || responseData.Equals("failed to update blockedby")) {
                unBlockFailed = true;
            } else {
                unBlockFailed = false;
            }
            // @TODO Let me know if server didn't work as expected @STEPHEN
            if (unBlockFailed){
                StartCoroutine(showError("Could not unblock the user, please try again")); // set error message
            }
            else{
                BlockButton.GetComponentInChildren<Text>().text = "Block"; // set button text
                alreadyBlocked = false; // since we unblocked the user, set alreadyBlocked to false
            }
        }
        else{ // block user
            // @TODO Block user through server @STEPHEN

            BlockRequest req = new BlockRequest(Global.getID(), Global.getToken(), "blockUser", Global.getEmail(), email);
            string json = JsonConvert.SerializeObject(req);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);
            data = new Byte[1024];
            string responseData = string.Empty;
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool blockFailed = false;
            if(blockFailed){
                StartCoroutine(showError("Could not block the user, please try again")); // set error message
            }
            else{
                BlockButton.GetComponentInChildren<Text>().text = "Unblock"; // set button text
                alreadyBlocked = true; // since we blocked the user, set alreadyBlocked to true
            }
        }
    }

    private void setReportButton(){ // set up the report button
        if(isMe){
            ReportButton.SetActive(false); // cannot report myself
        }
        else{
            // @TODO Get from server if I have already reported the user @STEPHEN

            genericRequest req = new genericRequest(Global.getID(), Global.getToken(), "getMyReportedPlayers");
            string json = JsonConvert.SerializeObject(req);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            Global.stream.Write(data, 0, data.Length);
            data = new Byte[1024];
            string responseData = string.Empty;
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            List<string> reportedList = responseData.Split(',').ToList();
            if (reportedList.Contains(email)) {
                alreadyReported = true;
            } else {
                alreadyReported = false;
            }

            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not retreive if you have reported the user")); // set error message
                ReportButton.GetComponentInChildren<Button>().interactable = false; // couldn't retrieve info so disable button
            }
            else{
                if(alreadyReported){
                    ReportButton.GetComponentInChildren<Text>().text = "Reported"; // set button text
                    ReportButton.GetComponentInChildren<Button>().interactable = false; // cannot undo a report
                }
            }
        }
    }

    public class ReportRequest {
        public string id;
        public string cmd;
        public string myEmail;
        public string theirEmail;
        public string token;

        public ReportRequest(string id, string cmd, string myEmail, string theirEmail, string token) {
            this.id = id;
            this.cmd = cmd;
            this.myEmail = myEmail;
            this.theirEmail = theirEmail;
            this.token = token;
        }
    }
    public void handleReport(){ // report a user & handle button
        genericRequest req = new genericRequest(Global.getID(), Global.getToken(), "getMyReportedPlayers");
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[1024];
        string responseData = string.Empty;
        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

        List<string> reportedList = responseData.Split(',').ToList();
        if (reportedList.Contains(email)) {
            alreadyReported = true;
        } else {
            alreadyReported = false;
        }

            if (!alreadyReported){  // cannot report again
                // @TODO Report user through server @STEPHEN
                ReportRequest req2 = new ReportRequest(Global.getID(), "reportPlayer", Global.getEmail(), email, Global.getToken());
                json = JsonConvert.SerializeObject(req2);
                data = System.Text.Encoding.ASCII.GetBytes(json);
                Global.stream.Write(data, 0, data.Length);
                data = new Byte[1024];
                responseData = string.Empty;
                bytes = Global.stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);


            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not report the user, please try again")); // set error message
            }
            else{
                ReportButton.GetComponentInChildren<Text>().text = "Reported"; // set button text
                ReportButton.GetComponentInChildren<Button>().interactable = false; // cannot undo a report
                alreadyReported = true; // user reported so lets set alreadyReported to true
            }
        }
    }

    IEnumerator showError(string value){ // Set the error message
        ErrorText.text = value;
        ErrorText.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(3f); 
        ErrorText.text = "";
        ErrorText.GetComponent<Text>().enabled = false;
    }

    public void goBack(){ // Load Previous Scene
        if(isMe){
            SceneManager.LoadScene("Lobby");
        }
        else{
            SceneManager.LoadScene("SearchFriendsScene");
        }
    }

    public void loadEditProfile(){ // Load Edit Profile Scene
        requestsPanelHolder.SetActive(false); // hide friend requests list
        friendsPanelHolder.SetActive(false); // hide friends list
        PlayerPrefs.SetString(EMAIL_PREF_KEY,Global.getEmail()); // store my email in PLAYER PREFS
        SceneManager.LoadScene("EditProfileScene");
    }

    private List<string> setUpFriendsList(){ // Set up friends list
        List<string> friendsList = new List<string>(); // To store all my friends
        
        Button[] gameObjects = friendsPanel.GetComponentsInChildren<Button>(); // Get previous friend buttons
        foreach(Button o in gameObjects){
            Destroy(o.gameObject); // Destroy all previous friend buttons
        }
        
        // Add temporary users to friends list
        friendsList.Add("Hola");
        friendsList.Add("Amigo");
        friendsList.Add("Que Paso");
        friendsList.Add("HolaAmigo");
        friendsList.Add("HolaAmigo");
        friendsList.Add("HolaAmigo");
        friendsList.Add("HolaAmigo");
        friendsList.Add("HolaAmigo");
        friendsList.Add("HolaAmigo");

        // @TODO Get from server if I have friends @STEPHEN
        // List<string> getFriends = new List<string>;
        // getFriends = //get list from server
        // @TODO Let me know if server didn't work as expected @STEPHEN
        // bool failed = false;
        // if(failed){ // server request failed
        //     StartCoroutine(showError("Could not retreive friends list")); // set error message
        // }
        // else{
        //     // get list from server
        //     // foreach(string value in getFriends){
        //     //     friendsList.Add(value); // add friends to friends list
        //     // }
        // } 
        foreach(string value in friendsList){
            GameObject friendObject = (GameObject)Instantiate(friendButtonPrefab); // Create friend user button
            friendObject.GetComponentInChildren<Text>().text = value; // Set text to the friend username 
            friendObject.SetActive(true);
            friendObject.transform.SetParent(friendsPanel.transform, false); // Add friend username buttons to friends panel
        }

        return friendsList;
    }

    public void loadFriendsList(){ // handle button & hide/unhide friends list
        requestsPanelHolder.SetActive(false); // hide friend requests list
        List<string> getList = setUpFriendsList();
        if(getList.Count == 0){
            StartCoroutine(showError("No friends found, please search users & add friends!")); // show error
            friendsPanelHolder.SetActive(false); // hide friends list
        }
        else if(friendsPanelHolder.activeSelf){
            friendsPanelHolder.SetActive(false); // hide friends list
        }
        else{
            friendsPanelHolder.SetActive(true); // unhide friends list
        }
    }


    public void loadUserProfile(Button btn){ // Go to the user's profile page scene
        PlayerPrefs.SetString(EMAIL_PREF_KEY,btn.GetComponentInChildren<Text>().text); // store user email in PLAYER PREFS
        Debug.Log(btn.GetComponentInChildren<Text>().text);
    }

    private List<string> setUpFriendRequestsList(){ // Set up friend requests list
        List<string> friendRequestsList = new List<string>(); // To store all my friend requests
        
        Button[] gameObjects = requestsPanel.GetComponentsInChildren<Button>(); // Get previous request buttons
        foreach(Button o in gameObjects){ 
            Destroy(o.gameObject); // Destroy all previous friend request buttons
        }

        // Add temporary users to friend requests list
        friendRequestsList.Add("Hola");
        friendRequestsList.Add("Amigo");
        friendRequestsList.Add("Que Paso");
        friendRequestsList.Add("HolaAmigo");
        friendRequestsList.Add("HolaAmigo");
        friendRequestsList.Add("HolaAmigo");
        friendRequestsList.Add("HolaAmigo");
        friendRequestsList.Add("HolaAmigo");
        friendRequestsList.Add("HolaAmigo");

        // @TODO Get from server if I have friend requests @STEPHEN
        // List<string> getRequests = new List<string>;
        // getRequests = // get list from server
        // @TODO Let me know if server didn't work as expected @STEPHEN
        // bool failed = false;
        // if(failed){ // server request failed
        //     StartCoroutine(showError("Could not retreive friend requests")); // set error message
        // }
        // else{
        //     // get list from server
        //     // foreach(string value in getRequests){
        //     //     friendRequestsList.Add(value); // add users to friend requests list
        //     // }
        // }
        foreach(string value in friendRequestsList){
            GameObject requestObject = (GameObject)Instantiate(requestButtonPrefab); // Create friend user button
            requestObject.GetComponentInChildren<Text>().text = value; // Set text to the friend username 
            requestObject.SetActive(true);
            requestObject.transform.SetParent(requestsPanel.transform, false); // Add friend username buttons to friends panel
        }

        return friendRequestsList;
    }

    public void loadRequestsList(){ // Handle button & hide/unhide requests list    
        friendsPanelHolder.SetActive(false); // hide friend list
        List<string> getList = setUpFriendRequestsList();
        if(getList.Count == 0){
            StartCoroutine(showError("No friend requests yet!")); // show error
            requestsPanelHolder.SetActive(false); // hide friend requests list
        }
        else if(requestsPanelHolder.activeSelf){
            ErrorText.GetComponent<Text>().enabled = false;
            requestsPanelHolder.SetActive(false); // hide friend requests list
        }
        else{
            ErrorText.text = "Click on username to handle friend request";
            ErrorText.GetComponent<Text>().enabled = true;
            requestsPanelHolder.SetActive(true); // unhide friend requests list
        }
    }

    public void requestButtonClicked(Button btn){ // click on a friend request
        Debug.Log(btn.GetComponentInChildren<Text>().text); 
        requestsPanelHolder.SetActive(false); // hide friend requests list
        handleRequestPanel.SetActive(true);
        ErrorText.text = "Click on username to go to their profile";
        ErrorText.GetComponent<Text>().enabled = true;
        userRequestButton.GetComponentInChildren<Text>().text = btn.GetComponentInChildren<Text>().text; // set username
    }

    public void acceptRequest(){ // accept a friend request
        if(isMe){
            // @TODO accept the friend request & add to friend list on server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            // bool myAcceptFailed = false;
            // if(myAcceptFailed){ // server request failed
            //     StartCoroutine(showError("Could not accept friend request, please try again")); // set error message
            // }
            // else{
            handleRequestPanel.SetActive(false); // hide handle request panel UI
            StartCoroutine(showError("Friend Request Accepted")); // set message
            loadRequestsList(); // to update the requests list
            //}
        }
        else{
            // @TODO accept the friend through server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            // bool userAcceptFailed = false;
            // if(userAcceptFailed){ // server request failed
            //     StartCoroutine(showError("Could not accept friend request, please try again")); // set error message
            // }
            // else{
            handleRequestPanel.SetActive(false); // hide handle request panel UI
            StartCoroutine(showError("Friend Request Accepted")); // set message
            AddButton.GetComponentInChildren<Text>().text = "Unfriend"; // set button text
            AddButton.GetComponentInChildren<Button>().interactable = true;
            alreadyFriend = true; // now the user is a friend, set to true
            alreadyReceivedRequest = false; // just to be sure
            alreadySentRequest = false; // just to be sure
            //}
        }
        
    }

    public void rejectRequest(){ // reject a friend request
        if(isMe){
            // @TODO reject the friend request & remove from friend list on server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            // bool myRejectFailed = false;
            // if(myRejectFailed){ // server request failed
            //     StartCoroutine(showError("Could not reject request, please try again")); // set error message
            // }
            // else{
            handleRequestPanel.SetActive(false); // hide handle request panel UI
            StartCoroutine(showError("Friend Request Rejected")); // set message
            loadRequestsList(); // to update the requests list
            //}
        }
        else{
            // @TODO reject the friend request on server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            // bool userRejectFailed = false;
            // if(userRejectFailed){ // server request failed
            //     StartCoroutine(showError("Could not reject request, please try again")); // set error message
            // }
            // else{
            handleRequestPanel.SetActive(false); // hide handle request panel UI
            StartCoroutine(showError("Friend Request Rejected")); // set message
            AddButton.GetComponentInChildren<Text>().text = "Add Friend"; // set button text
            AddButton.GetComponentInChildren<Button>().interactable = true;
            alreadyFriend = false; // now the user is not a friend, set to false
            alreadyReceivedRequest = false; // just to be sure
            alreadySentRequest = false; // just to be sure
            //}
        }
        
    }

    public void cancelRequest(){ // hide handle request panel UI
        ErrorText.text = "";
        ErrorText.GetComponent<Text>().enabled = false;
        handleRequestPanel.SetActive(false);
        if(isMe) {
            requestsPanelHolder.SetActive(true); // unhide requests list
        }
    }
}
