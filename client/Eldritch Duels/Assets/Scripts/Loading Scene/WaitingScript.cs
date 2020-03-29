using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using eldritch;

public class WaitingScript : MonoBehaviour
{
    public string nextSceneName = "DuelScene";

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
    /* void Update()
    {
        // Since async doesn't want to work, just check if there is data every frame
        if (Global.client.Available != 0)
        {
            byte[] data = new byte[256];
            Int32 bytes = Global.stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            const string expectedBeginning = "match found: ";
            if (responseData.StartsWith(expectedBeginning))
            {
                try
                {
                    string[] lines = responseData.Split('\n');
                    Debug.Log("resp " + responseData);
                    if (lines.Length == 3 || lines.Length == 4)
                    {
                        Global.enemyUsername = lines[0].Substring(expectedBeginning.Length);
                        Debug.Log("Enemy user: " + Global.enemyUsername);

                        Global.matchID = lines[1];
                        Debug.Log("Match ID: " + Global.matchID);

                        if (lines.Length == 4) {
                            if (lines[2].Contains("my turn"))
                                Global.DuelMyTurn = true;
                            else
                                Debug.Log("Unknown line: " + lines[2]);
                        }

                        SceneManager.LoadScene(nextSceneName);
                        Global.inQueue = false;
                    }
                    else
                        foreach (string s in lines)
                            Debug.Log(s);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }finally{
                    SceneManager.LoadScene(nextSceneName);
                }
            }
            else{
                Debug.Log(responseData);
                if(responseData.Contains("my turn")){
                    Global.DuelMyTurn = true;
                    SceneManager.LoadScene(nextSceneName);
                }
            }
        }
    } */
}
