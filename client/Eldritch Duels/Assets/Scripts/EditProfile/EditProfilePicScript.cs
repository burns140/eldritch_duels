using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using eldritch;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EditProfilePicScript : MonoBehaviour
{
    public Dropdown dropdown; // Picture Dropdown on the UI
    public Sprite[] pictures; // List of available pictures
    public InputField screenNameInput; // Screenname field on the UI
    public InputField bioInput; // Bio field on the UI
    private string bio; // save new bio to this string
    private string screenname; // save new screenname to this string
    private int picnum=0; // default profile pic is the first option

    // Start is called before the first frame update
    void Start()
    {
        dropdownSetup();
        displayPic();
        displayBio();
        displayScreenName();
    }

    public void onSave(){
        Debug.Log("Clicked on Save Profile");
        bio = bioInput.text; // save new bio
        Debug.Log("This is the new bio: "+bio);
        screenname = screenNameInput.text; // save new screenname
        Debug.Log("This is the new screenname: "+screenname);

        // Sending request to server to update bio, screen name, & profile pic
        EditProfileRequest req = new EditProfileRequest("editProfile", Global.getID(), Global.getToken(), bio, picnum, screenname);
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;

        Int32 bytes = Global.stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

    }

    private void dropdownSetup(){
        dropdown.ClearOptions();
        List<Dropdown.OptionData> picItems = new List<Dropdown.OptionData>();

        foreach(var pic in pictures){
            var picOption = new Dropdown.OptionData(pic.name, pic);
            picItems.Add(picOption); 
        }

        dropdown.AddOptions(picItems); // Adding all available picture to the Dropdown UI
    }

    public void handlePicName(int val){
        picnum = val; // Get selected picture index from the dropdown
        Debug.Log("Selected Profile Pic option: "+picnum.ToString());
    }

    private void displayPic(){

        int originalPic = Global.avatar; // Get original picnum from global variable

        var dropdownInstance = dropdown.GetComponent<Dropdown>();
        dropdownInstance.value = originalPic; // Select original picture option on the UI


    }

    private void displayBio(){

        string originalBio = Global.bio; // Get original bio from global variable

        var bioInstance = bioInput.GetComponent<InputField>();
        bioInstance.text = originalBio; // Display original bio on the UI
    }

    private void displayScreenName(){

        string originalScreenname = Global.username; // Get original screenname from global variable

        var screennameInstance = screenNameInput.GetComponent<InputField>();
        screennameInstance.text = originalScreenname; // Display original screenname on the UI
    }

    // Class for server request to send Profile changes
    public class EditProfileRequest {
        public string cmd;
        public string id;
        public string token;
        public string bio;
        public int avatar;
        public string username;

        public EditProfileRequest (string cmd, string id, string token, string bio, int avatar, string username) {
            this.cmd = cmd;
            this.id = id;
            this.token = token;
            this.bio = bio;
            this.avatar = avatar;
            this.username = username;
        }
    }

}
