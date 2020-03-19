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
    public Sprite[] pictures; // List of available pictures
    public Text bio; // bio text on the UI
    private string bioText; // store bio text from server

    public Image profilePic; // profile pic on the UI
    public Text username; // username text on the UI
    private string usernameText; // store username text from server

    public Button AddButton; // add/remove friend button on UI
    private bool alreadyFriend; // boolean to check if user is already a friend
    public Button BlockButton; // block user Button on UI
    private bool alreadyBlocked; // boolean to check if user is blocked by you
    public Button ReportButton; // report user button on UI
    private bool alreadyReported; // boolean to check if user is reported by you
    

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        displayPic();
        displayBio();
        displayScreenName(); 
        setAddButton();
        setReportButton();
        setBlockButton();
    }

    private void displayPic(){
        int originalPic=0; // Get user's profile pic from server

        profilePic.GetComponent<Image>().sprite = pictures[originalPic]; // Set profile pic on UI
    }

    private void displayBio(){
        bioText =  "bio"; // Get user's bio from server

        bio.text = bioText; // Set bio on UI
    }

    private void displayScreenName(){
        usernameText = "username"; // Get user's username from server

        username.text = usernameText; // Set username on UI
    }

    private void setAddButton(){
        alreadyFriend = true; // Check if the user is my friend from server

        if(alreadyFriend){
            AddButton.GetComponentInChildren<Text>().text = "Unfriend";
        }
    }

    public void handleAddFriend(){
        if(alreadyFriend){ // unfriend user
            
            AddButton.GetComponentInChildren<Text>().text = "Add Friend";
            alreadyFriend = false;
        }
        else{ // add user as friend

            AddButton.GetComponentInChildren<Text>().text = "Unfriend";
            alreadyFriend = true;
        }
    }

    private void setBlockButton(){
        alreadyBlocked = true; // Check if the user is blocked by you from server

        if(alreadyBlocked){
            BlockButton.GetComponentInChildren<Text>().text = "Unblock";
        }
    }

    public void handleBlock(){
        if(alreadyBlocked){ // unblock user

            BlockButton.GetComponentInChildren<Text>().text = "Block";
            alreadyBlocked = false;
        }
        else{ // block user

            BlockButton.GetComponentInChildren<Text>().text = "Unblock";
            alreadyBlocked = true;
        }
    }

    private void setReportButton(){
        alreadyReported = false; // Check if the user is reported by you from server

        if(alreadyReported){
            ReportButton.GetComponentInChildren<Text>().text = "Reported";
            ReportButton.interactable = false; // cannot undo a report
        }
    }

    public void handleReport(){
        if(!alreadyReported){ // report user

            ReportButton.GetComponentInChildren<Text>().text = "Reported";
            ReportButton.interactable = false; // cannot undo a report
            alreadyReported = true; 
        }
    }
}
