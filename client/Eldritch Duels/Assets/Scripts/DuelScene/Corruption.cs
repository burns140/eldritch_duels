using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Corruption : MonoBehaviour
{
    public GameObject panel;

    public void TogglePanel()
    {
        Debug.Log(panel.activeSelf);
        panel.SetActive(!panel.activeSelf);
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
