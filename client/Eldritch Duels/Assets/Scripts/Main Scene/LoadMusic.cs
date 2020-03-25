using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadMusic : MonoBehaviour
{
    // Play music globally
    private static LoadMusic instance;
    public static LoadMusic Instance{
        get {return instance;}
    }
    
    // Awake is called when the script instance is being loaded
    void Awake(){
        Debug.Log("Trying to Load Music");

        if(instance != null && instance != this){
            Destroy(this.gameObject); // Destroy duplicate instances
            return;
        }
        else{
            instance = this;
        }
        
        DontDestroyOnLoad(this.gameObject); // Music continues on next scene
        
    }

}
