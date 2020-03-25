using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatButtonScript : MonoBehaviour
{

    public GameObject ChatPanel; // The chat panel in the UI

    public void OnClick() // Enable/Disable Chat in the UI
    {
        if(ChatPanel.activeSelf){
            ChatPanel.SetActive(false);
        }
        else{
            ChatPanel.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
