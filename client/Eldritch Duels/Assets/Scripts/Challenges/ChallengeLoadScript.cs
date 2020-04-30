using eldritch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeLoadScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Request req = new Request(Global.getID(), Global.getToken(), "getChallengeNames");
        string res = Global.NetworkRequest(req);

        string[] names = res.Split(';');
        string daily = "Daily: " + names[0];
        string weekly = "Weekly: " + names[1];
        string monthly = "Monthly: " + names[2];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
