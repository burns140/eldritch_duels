using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Net.Sockets;
using eldritch;

public class profilepicture
{
    public byte[] picture;
    public string token;
    public string id;
    public string cmd;

    public profilepicture(byte[] picture, string token, string id, string cmd)
    {
        this.picture = picture;
        this.token = token;
        this.id = id;
        this.cmd = cmd;
    }
}

public class Upload : MonoBehaviour
{
    public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
    public UnityEngine.UI.Button upload;
    // Start is called before the first frame update
    void Start()
    {
        upload.onClick.AddListener(openBrowser);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void openBrowser()
    {
        FileBrowser.OnSuccess getpath = setpath;
        FileBrowser.OnCancel none = cancelled;
        Debug.Log("Opening browser");
        FileBrowser.ShowLoadDialog(getpath, none);

    }

    public void setpath(string path)
    {
        Debug.Log(path);
        Debug.Log("File select success");
        if (FileBrowser.Success)
        {
            if (ImageExtensions.Contains(Path.GetExtension(path).ToUpperInvariant()))
            {
                //SEND REQUEST WITH IMAGE
                //UPDATE UI
                Debug.Log("Valid image!");

                byte[] imagebytes = File.ReadAllBytes(path);

                profilepicture pfp = new profilepicture(imagebytes, Global.getToken(), Global.getID(), "uploadProfilePicture");

                string json = JsonConvert.SerializeObject(pfp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                NetworkStream stream = Global.client.GetStream();

                stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responseData = string.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            }
            else
            {
                //MAKE ERROR MESSAGE
                Debug.Log("Invalid file!");
            }
        }
        else
        {
            Debug.Log("Browser error");
        }
    }

    public void cancelled()
    {
        Debug.Log("File select cancelled");
    }

}
