using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ApplySettingsScript : MonoBehaviour
{
    private const string RESOLUTION_PREF_KEY = "resolution"; // PLAYER PREF KEY to store resolution settings
    private const string WINDOW_PREF_KEY = "window"; // PLAYER PREF KEY to store window mode settings
    private const string PROFANITY_PREF_KEY = "profanity";

    public Button applyButton; // Get the apply button from UI
    public Toggle isFullScreen; // Get the full screen toggle from UI
    public Toggle isWindowed; // Get the windowed toggle from UI
    public Toggle ProfanityFilter; // Get the profanity filter toggle from UI
    // public Toggle isWindowedBorderless; // No longer implementing this in the game
    private int width = 1920; // Default resolution width
    private int height = 1080; // Default resolution height
    private string reskey = "1920x1080"; // Default PLAYER PREF KEY resolution value
    private string windowkey = "fullScreen"; // Default PLAYER PREF KEY window mode value
    private string profanitykey = "true"; // Default PLAYER PREF KEY profanity filter value
    
    // Get the resolution selected from the dropdown on the UI
    public void handleRes(int val){
        if(val == 0){
            width = 1920;
            height = 1080;
            reskey = "1920x1080";
            Debug.Log("1920x1080 selected");
        }
        if(val == 1){
            width = 1280;
            height = 720;
            reskey = "1280x720";
            Debug.Log("1280x720 selected");
        }
        if(val == 2){
            width = 800;
            height = 600;
            reskey = "800x600";
            Debug.Log("800x600 selected");
        }
    }

    // Get the selected windowed mode selected on the UI
    public void ActiveToggle(){

        if(isWindowed.isOn){
            Debug.Log("Windowed is selected");
            windowkey = "windowed";
            callWindowed();
        }
        /*else if(isWindowedBorderless.isOn){
            Debug.Log("WindowedBorderless is selected");
            windowkey = "windowedBorderless";
            callWindowedBorderless();
        }*/ // No Longer implementing this in the game
        else{
            Debug.Log("FullSceen is selected");
            windowkey = "fullScreen";
            callFullScreen();
        }
        if(ProfanityFilter.isOn)
        {
            profanitykey = "true";
        }
        else
        {
            profanitykey = "false";
        }
    }

    void callWindowed(){
        Screen.SetResolution(width, height, FullScreenMode.Windowed); // Set windowed mode & new resolution
    }

    void callFullScreen(){
        Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen); // Set full screen mode & new resolution
    }

    /* No longer implementing this in the game
    void callWindowedBorderless(){
        Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
    }
    */

    // Handle Apply button click
    public void OnSubmit(){ 
        Debug.Log("Clicked on Apply Settings");
        ActiveToggle();
        PlayerPrefs.SetString(RESOLUTION_PREF_KEY,reskey); // Save the new resolution to PLAYER PREF KEY
        PlayerPrefs.SetString(WINDOW_PREF_KEY,windowkey); // Save the new windowed mode to PLAYER PREF KEY
        PlayerPrefs.SetString(PROFANITY_PREF_KEY, profanitykey);
        Debug.Log(Screen.fullScreen);
        Debug.Log(Screen.currentResolution);
        SceneManager.LoadScene("Lobby");
    }

    
}