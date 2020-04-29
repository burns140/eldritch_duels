using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using eldritch;

public class ProfanityFilterScript : MonoBehaviour
{
    public Toggle ProfanityFilter;

    private const string PROFANITY_PREF_KEY = "profanity";

    // Start is called before the first frame update
    void Start()
    {
        string res = PlayerPrefs.GetString(PROFANITY_PREF_KEY, "true"); // Get the saved profanity filter setting from PLAYER PREFS
        if (res.Equals("true"))
        {
            Global.profanityFilter = true;
            ProfanityFilter.isOn = true;
        }
        else
        {
            Global.profanityFilter = false;
            ProfanityFilter.isOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnChangeValue()
    {
        if (ProfanityFilter.isOn)
        {
            Global.profanityFilter = true;
        }
        else
        {
            Global.profanityFilter = false;
        }
    }
}
