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
    private bool alreadyFriend; // boolean to check if user is already a friend
    public GameObject BlockButton; // block user Button on UI
    private bool alreadyBlocked; // boolean to check if user is blocked by you
    public GameObject ReportButton; // report user button on UI
    private bool alreadyReported; // boolean to check if user is reported by you

    public GameObject buttonPanel; // Panel with all buttons on UI

    public Button GoBackButton; // go back button
    public GameObject EditProfileButton; // edit profile button
    public GameObject FriendsButton; // show friends list button
    private bool isMe; // True if it's my profile
    private bool hasPicIndex; // Check if picture was from default pictures
    public Text ErrorText; // Display error message on UI
    #endregion

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        isItMe();
        if(isMe){
            EditProfileButton.SetActive(true);
            FriendsButton.SetActive(true);
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

    private void isItMe(){
        // @TODO Check if it's me (@STEPHEN/@KEVING)
        isMe = true; // If it's my account 
        // isMe = false; // If it's not my account
    }

    private bool getBlockedMe(){
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
            return false;
        }
        return false; 
    }

    private void displayPic(){
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
                    bool failed = false;
                    if(failed){
                        StartCoroutine(showError("Could not retreive my profile pic")); // set error message
                    }
                    else{
                        profilePic.GetComponent<Image>().sprite = pictures[picIndex]; // Set profile pic on UI
                    }
                }
                else{
                    picIndex = 0; // get user's pic index @STEPHEN
                    // @TODO Let me know if server didn't work as expected @STEPHEN
                    bool failed = false;
                    if(failed){
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
                    bool failed = false;
                    if(failed){
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
                    bool failed = false;
                    if(failed){
                        StartCoroutine(showError("Could not retreive my profile pic")); // set error message
                    }
                    else{
                        //profilePic.GetComponent<Image>().sprite = newImage;
                    }
                }
            }
        }
    }

    private void displayBio(){
        
        if(isMe){
            // @TODO Get my bio from server @STEPHEN/@KEVING
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not retreive my bio")); // set error message
            }
            else{
                bioText =  "bio"; // Get my bio from server @STEPHEN/@KEVING
            }
        }
        else{
            // @TODO Get user's bio from server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not retreive user's bio")); // set error message
            }
            else{
                bioText =  "bio"; // Get user's bio from server @STEPHEN
            }
        }

        bio.text = bioText; // Set bio on UI
    }

    private void displayScreenName(){

        if(isMe){
            // @TODO Get my username from server @STEPHEN/@KEVING
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not retreive my username")); // set error message
            }
            else{
                usernameText = "username"; 
            }
        }
        else{
            // @TODO Get user's username from server @STEPHEN/@KEVING
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not retreive user's username")); // set error message
            }
            else{
                usernameText = "username";
            }
        }

        username.text = usernameText; // Set username on UI
    }

    private void setAddButton(){

        if(isMe){
            AddButton.SetActive(false); // cannot add myself as friend
        }
        else{
            // @TODO Get from server if I have already blocked the user
            alreadyFriend = true; // Check if the user is my friend from server
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not retreive if user is your friend")); // set error message
                AddButton.GetComponentInChildren<Button>().interactable = false; // couldn't retrieve info so disable button
            }
            else{
                if(alreadyFriend){
                    AddButton.GetComponentInChildren<Text>().text = "Unfriend"; // set button text
                }
            }
        }
    }

    public void handleAddFriend(){
        if(alreadyFriend){ // unfriend user
            // @TODO Unfriend user through server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not unfriend user, please try again")); // set error message
            }
            else{
                AddButton.GetComponentInChildren<Text>().text = "Add Friend"; // set button text
                alreadyFriend = false;  // since we unfriended the user, set alreadyFriend to false
            }
        }
        else{ // add user as friend
            // @TODO Add user as friend through server @STEPHEN
            // @TODO Let me know if server didn't work as expected @STEPHEN
            bool failed = false;
            if(failed){
                StartCoroutine(showError("Could not add user as friend, please try again")); // set error message
            }
            else{
                AddButton.GetComponentInChildren<Text>().text = "Unfriend"; // set button text
                alreadyFriend = true; // since we added user as friend, set alreadyFriend to true
            }
        }
    }

    private void setBlockButton(){ // set up the block button
        
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
            bool failed = false;
            if(failed){
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
            bool failed = false;
            if(failed){
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
        ErrorText.GetComponent<Text>().enabled = false;
    }

    public void goBack(){ // Load Previous Scene

    }

    public void loadEditProfile(){ // Load Edit Profile Scene

    }

    public void loadFriendsList(){ // Load my friends list

    }
}
