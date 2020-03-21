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
    #endregion

    void Awake() // Awake is called when the script instance is being loaded
    {
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
        isMe = true; // If it's my account 
        // isMe = false; // If it's not my account
    }

    private bool getBlockedMe(){ // check if I the user has blocked me
        // @TODO Check if I am blocked by this user
        // @TODO Let me know if server didn't work as expected @STEPHEN
        bool failed = false;
        if(failed){
            StartCoroutine(showError("Could not retreive all info, please go back")); // set error message
            return true; // just so that nothing loads
        }
        else{
            // If I'm blocked then hide buttons & return true;
            // Button[] gameObjects = buttonPanel.GetComponentsInChildren<Button>(); // Get buttons from panel
            // foreach(Button o in gameObjects){ 
            //     Destroy(o.gameObject); // Destroy buttons on UI
            // }
            // ErrorText.text = "You have been blocked by this user")); // set error message
            // ErrorText.GetComponent<Text>().enabled = true; 
            //return true;
            // If I'm not blocked then
            // return false;
        }
        return false; 
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
                    picIndex = 0; // get my pic index @STEPHEN/@KEVING
                    // @TODO Let me know if server didn't work as expected @STEPHEN
                    bool myIndexFailed = false;
                    if(myIndexFailed){
                        StartCoroutine(showError("Could not retreive my profile pic")); // set error message
                    }
                    else{
                        profilePic.GetComponent<Image>().sprite = pictures[picIndex]; // Set profile pic on UI
                    }
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

    private void displayBio(){ // get bio & display it
        
        if(isMe){
            // @TODO Get my bio from server @STEPHEN/@KEVING
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool myBioFailed = false;
            if(myBioFailed){
                StartCoroutine(showError("Could not retreive my bio")); // set error message
            }
            else{
                bioText =  "bio"; // Get my bio from server @STEPHEN/@KEVING
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
                bioText =  "bio"; // Get user's bio from server @STEPHEN
            }
        }

        bio.text = bioText; // Set bio on UI
    }

    private void displayScreenName(){ // get username & display it

        if(isMe){
            // @TODO Get my username from server @STEPHEN/@KEVING
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool myUsernameFailed = false;
            if(myUsernameFailed){
                StartCoroutine(showError("Could not retreive my username")); // set error message
            }
            else{
                usernameText = "username"; 
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
                usernameText = "username";
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
            alreadyFriend = true; // Store if the user is my friend
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
                    alreadyReceivedRequest = true; // Store if the user has sent me friend request
                    if(alreadyReceivedRequest){
                        AddButton.GetComponentInChildren<Text>().text = "Handle Request"; // set button text
                        AddButton.GetComponentInChildren<Button>().interactable = true;
                    }
                    else{
                        // @TODO Get from server if I have sent a friend request to the user @STEPHEN
                        alreadySentRequest = true; // Store if I sent the user a friend request
                        if(alreadySentRequest){
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
                // @TODO Let me know if server didn't work as expected @STEPHEN
                bool friendFailed = false;
                if(friendFailed){
                    StartCoroutine(showError("Could not add user as friend, please try again")); // set error message
                }
                else{
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
            alreadyBlocked = false; // Check if the user is blocked by me from server
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

    public void handleBlock(){ // block/unblock user & handle button
        
        if(alreadyBlocked){ // unblock user
            // @TODO Unblock user through server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool unBlockFailed = false;
            if(unBlockFailed){
                StartCoroutine(showError("Could not unblock the user, please try again")); // set error message
            }
            else{
                BlockButton.GetComponentInChildren<Text>().text = "Block"; // set button text
                alreadyBlocked = false; // since we unblocked the user, set alreadyBlocked to false
            }
        }
        else{ // block user
            // @TODO Block user through server @STEPHEN
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
            alreadyReported = false; // Check if the user is reported by me from server
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

    public void handleReport(){ // report a user & handle button
        
        if(!alreadyReported){  // cannot report again
            // @TODO Report user through server @STEPHEN
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

    }

    public void loadEditProfile(){ // Load Edit Profile Scene
        requestsPanelHolder.SetActive(false); // hide friend requests list
        friendsPanelHolder.SetActive(false); // hide friends list
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
