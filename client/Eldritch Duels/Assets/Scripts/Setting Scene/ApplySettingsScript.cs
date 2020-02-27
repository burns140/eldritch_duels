using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplySettingsScript : MonoBehaviour
{

    // PLAYER PREF to store settings
    private const string RESOLUTION_PREF_KEY = "resolution";
    private const string WINDOW_PREF_KEY = "window";

    public Button applyButton;
    public Toggle isFullScreen;
    public Toggle isWindowed;
    public Toggle isWindowedBorderless;
    private int width = 1920;
    private int height = 1080;
    private string reskey = "1920x1080";
    private string windowkey = "fullScreen";
    
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

    public void ActiveToggle(){

        if(isWindowed.isOn){
            Debug.Log("Windowed is selected");
            windowkey = "windowed";
            callWindowed();
        }
        else if(isWindowedBorderless.isOn){
            Debug.Log("WindowedBorderless is selected");
            windowkey = "windowedBorderless";
            callWindowedBorderless();
        }
        else{
            Debug.Log("FullSceen is selected");
            windowkey = "fullScreen";
            callFullScreen();
        }
    }

    void callWindowed(){
        Screen.SetResolution(width, height, FullScreenMode.Windowed);
    }

    void callFullScreen(){
        Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
    }

    void callWindowedBorderless(){
        Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
    }

    public void OnSubmit(){ 
        Debug.Log("Clicked on Apply Settings");
        ActiveToggle();
        PlayerPrefs.SetString(RESOLUTION_PREF_KEY,reskey);
        PlayerPrefs.SetString(WINDOW_PREF_KEY,windowkey);
        Debug.Log(Screen.fullScreen);
        Debug.Log(Screen.currentResolution);
    }

    
}