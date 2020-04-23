using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using eldritch;

public class Feedback : MonoBehaviour
{
    // Start is called before the first frame update
    public Button sendFeedbackButton;
    public InputField feedbackText;
    public string feedbackline;
    public bool isDev = false;
    public string userFeedback;
    void Start()
    {
        sendFeedbackButton.onClick.AddListener(sendFeedback);
        getCollection devcheck = new getCollection("isDeveloper", Global.getID(), Global.getToken());
        string res = Global.NetworkRequest(devcheck);
        if (res.Equals("true"))
        {
            isDev = true;
        }
        if (isDev)
        {
            userFeedback = getFeedback();
        }
    }

    // Update is called once per frame
    void Update()
    {
        feedbackline = feedbackText.text;
    }

    public void sendFeedback()
    {
        sendfeedback fb = new sendfeedback(feedbackline, Global.getToken(), Global.getID(), "sendFeedback");
        string result = Global.NetworkRequest(fb);
        Debug.Log(result);
    }

    public string getFeedback()
    {
        getCollection getreq = new getCollection("getFeedback", Global.getID(), Global.getToken());
        return Global.NetworkRequest(getreq);
    }
}
