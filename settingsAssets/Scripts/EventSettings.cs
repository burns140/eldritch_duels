using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventSettings : MonoBehaviour
{
    private const string WINDOW_PREF_KEY = "window";
    private const string RESOLUTION_PREF_KEY = "resolution";
	public Button SettingsButton;
    public string scene;
    private int width=1920;
	private int height=1080;
    private string window = "fullScreen";


    // Start is called before the first frame update
    void Start()
    {
        // Get the saved settings from PlayerPrefs
        string res = PlayerPrefs.GetString(RESOLUTION_PREF_KEY);
        Debug.Log("I'm at SampleScene "+res);
        window = PlayerPrefs.GetString(WINDOW_PREF_KEY);
        Debug.Log("I'm at SampleScene "+window);
        

        if(res=="1920x1080"){
			width = 1920;
			height = 1080;
        }
        else if(res=="1280x720"){
        	width = 1280;
        	height = 720;
        }
        else if(res=="800x600"){
        	width = 800;
        	height = 600;
        }
        else{
            width = 1920;
            height = 1080;
        }
        setResWindow();

        Button button = SettingsButton.GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
    }

    public void TaskOnClick()
    {
    	Debug.Log("You clicked the SettingsButton!");
        StartCoroutine(LoadYourAsyncScene());
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
    }   

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void setResWindow(){

    	if(window=="windowed"){
    		Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
        else if(window=="fullScreen"){
        	Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
        }
        else if(window=="windowedBorderless"){
        	Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
        }
        else{
            Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
        }
    }


}
