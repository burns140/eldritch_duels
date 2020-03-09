using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCheckMusic : MonoBehaviour
{
    public AudioClip audioClip; // The audio clip supposed to be playing for the current scene;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Checking if music playing is correct");

        if(LoadMusic.Instance.gameObject.GetComponent<AudioSource>().clip != audioClip){
            LoadMusic.Instance.gameObject.GetComponent<AudioSource>().clip = audioClip; // Change clip
            LoadMusic.Instance.gameObject.GetComponent<AudioSource>().Play(); // Play the new clip
        }
    }

}
