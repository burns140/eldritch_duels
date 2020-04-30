using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using eldritch;

public class SetMatchType : MonoBehaviour
{
    public bool test;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setMatchType(MatchType t)
    {
        Debug.Log("Set match type from " +  Global.matchType.ToString() + " to " + t.ToString());

        Global.matchType = t;
    }

    public void setMatchTypeAI()
    {
        setMatchType(MatchType.AI);
    }

    public void setMatchTypeCompetetive()
    {
        setMatchType(MatchType.COMPETITIVE);
    }

    public void setMatchTypeCasual()
    {
        setMatchType(MatchType.CASUAL);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
