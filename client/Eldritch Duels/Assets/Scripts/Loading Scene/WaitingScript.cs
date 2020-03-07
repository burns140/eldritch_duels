using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using eldritch;

public class WaitingScript : MonoBehaviour
{
    public string nextSceneName = "Scenes/DuelScene";

    // Start is called before the first frame update
    void Start()
    {
        // Old asyncronous solution
        // if it worked, it would be better

        /*byte[] data = new byte[256];
        Global.stream.BeginRead(data, 0, data.Length, (IAsyncResult ar) =>
        {
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
            if (responseData.StartsWith("match found"))
            {
                try
                {
                    Global.enemyUsername = responseData.Substring(responseData.IndexOf(':') + 2);
                    Debug.Log(global.enemyUsername);
                    SceneManager.LoadScene(nextSceneName);
                    Global.inQueue = false;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            else
                Debug.Log(responseData);
        }, null);*/
    }

    // Update is called once per frame
    void Update()
    {
        // Since async doesn't want to work, just check if there is data every frame
        if (Global.client.Available != 0)
        {
            byte[] data = new byte[256];
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            if (responseData.StartsWith("match found"))
            {
                try
                {
                    Global.enemyUsername = responseData.Substring(responseData.IndexOf(':') + 2);
                    Debug.Log(Global.enemyUsername);
                    SceneManager.LoadScene(nextSceneName);
                    Global.inQueue = false;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            else
                Debug.Log(responseData);
        }
    }
}
