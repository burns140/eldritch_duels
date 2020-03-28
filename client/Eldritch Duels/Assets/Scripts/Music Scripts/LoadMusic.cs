using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMusic : MonoBehaviour
{
    // Play music globally
    private static LoadMusic instance;
    public static LoadMusic Instance{
        get {return instance;}
    }
    
    // Awake is called when the script instance is being loaded
    void Awake(){
        
        if(instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }
        else{
            instance = this;
        }
        
        DontDestroyOnLoad(this.gameObject);
        
    }

}
