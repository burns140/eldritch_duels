using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;

public class ServerConnect : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Global.SetUpConnection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
