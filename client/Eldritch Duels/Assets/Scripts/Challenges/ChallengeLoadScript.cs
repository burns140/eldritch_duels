using eldritch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeLoadScript : MonoBehaviour
{
    
    public GameObject dailyChallengeText, weeklyChallengeText, monthlyChallengeText;
    // Start is called before the first frame update
    void Start()
    {
        Request req = new Request(Global.getID(), Global.getToken(), "getChallengeNames");
        string res = Global.NetworkRequest(req);

        Debug.Log(res);

        string[] names = res.Split(';');
        string daily = "Daily: " + names[0];
        string weekly = "Weekly: " + names[1];
        string monthly = "Monthly: " + names[2];

        dailyChallengeText.GetComponent<Text>().text = daily;
        weeklyChallengeText.GetComponent<Text>().text = weekly;
        monthlyChallengeText.GetComponent<Text>().text = monthly;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
