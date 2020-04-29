using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameSettings : MonoBehaviour
{
    private const string WINDOW_PREF_KEY = "window"; // PLAYER PREF KEY to store window mode
    private const string RESOLUTION_PREF_KEY = "resolution"; // PLAYER PREF KEY to store resolution
    private const string MUSIC_PREF_KEY = "music"; // PLAYER PREF KEY to store music volume
    private int width = 1920; // Default resoultion width
    private int height = 1080; // Default resolution height
    private string window = "fullScreen"; // Default screen mode
    public GameObject bgmusic;

    // Start is called before the first frame update
    void Start()
    {
        
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_PREF_KEY, 0.3f); // Get the music volume from PLAYER PREFS
        //AudioListener.volume = musicVolume; // Set music volume to saved volume
        bgmusic.GetComponent<AudioSource>().volume = musicVolume;
        Debug.Log("Saved Volume: "+musicVolume);
        
        
        string res = PlayerPrefs.GetString(RESOLUTION_PREF_KEY, "1920x1080"); // Get the resolution from PLAYER PREFS
       
        window = PlayerPrefs.GetString(WINDOW_PREF_KEY, "fullScreen"); // Get the windowed mode from PLAYER PREFS

        // Set the width and height based on saved resolution
        if (res == "1920x1080")
        {
            width = 1920;
            height = 1080;
        }
        else if (res == "1280x720")
        {
            width = 1280;
            height = 720;
        }
        else if (res == "800x600")
        {
            width = 800;
            height = 600;
        }
        else
        {
            width = 1920;
            height = 1080;
        }

        setResWindow();
    }


    private void setResWindow()
    {
        if (window == "windowed")
        {
            Screen.SetResolution(width, height, FullScreenMode.Windowed); // Set saved windowed mode & saved resolution
        }
        else if (window == "fullScreen")
        {
            Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen); // Set full screen mode & saved resolution
        }
        /*else if (window == "windowedBorderless")
        {
            Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
        }*/ // No longer implementing this in the game
        else
        {
            Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
        }
    }

}
