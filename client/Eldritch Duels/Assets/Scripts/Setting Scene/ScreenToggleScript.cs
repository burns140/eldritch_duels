using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScreenToggleScript : MonoBehaviour
{
	public ToggleGroup toggleGroupInstance;
	public Dropdown dropdownInstance;
	private const string WINDOW_PREF_KEY = "window";
    private const string RESOLUTION_PREF_KEY = "resolution";
	private string window = "fullScreen";


	void Start(){
		
		// Get the saved settings from PlayerPrefs
		string res = PlayerPrefs.GetString(RESOLUTION_PREF_KEY);
        window = PlayerPrefs.GetString(WINDOW_PREF_KEY);
		Debug.Log("I'm at ScreenToggle :"+window);

		var dropdown = dropdownInstance.GetComponent<Dropdown>();

		if(res=="1280x720"){
        	dropdown.value = 1;
        }
        else if(res=="800x600"){
        	dropdown.value = 2;
        }
        else{
            dropdown.value = 0;
        }
		
		if(window=="windowed"){
			var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
			toggles[1].isOn = true;
		}
		else if(window=="windowedBorderless"){
			var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
			toggles[2].isOn = true;
		}
		else{
			window = "fullScreen";
			var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
			toggles[0].isOn = true;
		}
		
		Debug.Log("First Selected: "+toggleGroupInstance.ActiveToggles().FirstOrDefault().name);
		
		/* Checking compatible resolutions
			Resolution[] resolutions = Screen.resolutions;
			foreach(var res in resolutions){
				Debug.Log(res.width+"x"+res.height);
			}
		*/
	}

	public void OnChangeValue(){
		var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
		if(toggles[0].isOn){
			Debug.Log("It's FullScreen now");
		}
		else if(toggles[1].isOn){
			Debug.Log("It's Windowed now");
		}
		else if(toggles[2].isOn){
			Debug.Log("It's Windowed Borderless now");
		}
	}
    
}
