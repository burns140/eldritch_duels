using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eldritch;

public class deleted
{
    public string token;
    public string id;
    public string cmd;


    public deleted(string cmd, string token, string id)
    {
        this.token = token;
        this.cmd = cmd;
        this.id = id;
    }
}

public class delete : MonoBehaviour
{
    public UnityEngine.UI.Button deletebutton;
    // Start is called before the first frame update
    void Start()
    {
        deletebutton.onClick.AddListener(clicked);
    }

    public void clicked()
    {
        deleted user = new deleted("delete", Global.getToken(), Global.getID());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
