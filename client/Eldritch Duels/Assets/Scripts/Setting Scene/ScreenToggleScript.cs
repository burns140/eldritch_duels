using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScreenToggleScript : MonoBehaviour
{
	public ToggleGroup toggleGroupInstance; // Window Mode Toggle Group on the UI
	public Dropdown dropdownInstance; // Resolutio Dropdown on the UI
	private const string WINDOW_PREF_KEY = "window"; // PLAYER PREF KEY to store window mode settings
    private const string RESOLUTION_PREF_KEY = "resolution"; // PLAYER PREF KEY to store resolution settings
	private string window = "fullScreen"; // Default screen mode is full screen

	void Start(){
		
		string res = PlayerPrefs.GetString(RESOLUTION_PREF_KEY, "1920x1080"); // Get the saved resolution from PLAYER PREFS
        window = PlayerPrefs.GetString(WINDOW_PREF_KEY, "fullScreen"); // Get the saved windowed mode form PLAYER PREFS
		Debug.Log("I'm at ScreenToggle :"+window);

		var dropdown = dropdownInstance.GetComponent<Dropdown>(); // Get instance of the resolution dropdown from UI;

		// Set the saved resolution option on the UI
		if(res=="1280x720"){
        	dropdown.value = 1;
        }
        else if(res=="800x600"){
        	dropdown.value = 2;
        }
        else{
            dropdown.value = 0;
        }
		
		// Set the saved window mode toggle on the UI
		if(window=="windowed"){
			var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
			toggles[1].isOn = true;
		}
		/*else if(window=="windowedBorderless"){
			var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
			toggles[2].isOn = true;
		}*/ // No longer implementing this in the game
		else{
			window = "fullScreen";
			var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
			toggles[0].isOn = true;
		}
		
		Debug.Log("First Selected: "+toggleGroupInstance.ActiveToggles().FirstOrDefault().name);
		
		/* Checking compatible resolutions for the device
			Resolution[] resolutions = Screen.resolutions;
			foreach(var res in resolutions){
				Debug.Log(res.width+"x"+res.height);
			}
		*/
	}

	// Check toggle change
	public void OnChangeValue(){
		var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
		if(toggles[0].isOn){
			Debug.Log("It's FullScreen now");
		}
		else{
			Debug.Log("It's Windowed now");
		}
		/*else if(toggles[1].isOn){
			Debug.Log("It's Windowed now");
		}
		else if(toggles[2].isOn){
			Debug.Log("It's Windowed Borderless now");
		}*/ // No longer implementing this in the game
	}
    
}
