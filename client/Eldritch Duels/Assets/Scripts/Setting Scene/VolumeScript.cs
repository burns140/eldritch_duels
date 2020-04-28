using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeScript : MonoBehaviour
{
    public Slider slider; // Slider UI
    private const string MUSIC_PREF_KEY = "music"; // PLAYER PREF KEY to store music volume
    public float wantedVolume;
    public void SetVolume(float Volume){
        LoadMusic.Instance.gameObject.GetComponent<AudioSource>().volume = Volume;
        //AudioListener.volume = Volume; // Dynamically change volume according to slider
        Debug.Log("Selected volume is: "+Volume);
    }

    void Awake(){
       
        wantedVolume=PlayerPrefs.GetFloat(MUSIC_PREF_KEY, 0.3f); // Get the music volume from PLAYER PREFS    
    
        slider.value = wantedVolume; // Move slider on UI to the saved volume
        Debug.Log("This is the saved volume: "+wantedVolume);
    }

    public void saveVolume(){
        Debug.Log("New saved volume is: "+slider.value);
        PlayerPrefs.SetFloat(MUSIC_PREF_KEY, slider.value); // Save new music volume to PlayerPref
    }
    public void cancelVolume(){
        LoadMusic.Instance.gameObject.GetComponent<AudioSource>().volume = wantedVolume;
        //AudioListener.volume = wantedVolume; // Reset volume
        slider.value = wantedVolume; // Reset slider position on UI
    }
    
}
