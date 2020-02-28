using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using eldritch;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine.SceneManagement;

public class EditProfilePicScript : MonoBehaviour
{

    public Dropdown dropdown;
    public Sprite[] pictures;
    public InputField screenNameInput;
    public InputField bioInput;
    private string bio;
    private string screenname;
    private int picnum=0;

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
        bio = bioInput.text;
        Debug.Log("This is the new bio: "+bio);
        screenname = screenNameInput.text;
        Debug.Log("This is the new screenname: "+screenname);
        
        // TODO - Update these on the database: bio, screenname, picnum

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

        dropdown.AddOptions(picItems);
    }

    public void handlePicName(int val){
        picnum = val;
        Debug.Log("Selected Profile Pic option: "+picnum.ToString());
    }

    private void displayPic(){
        // Display original pic on frontend

        int originalPic = Global.avatar; // TODO - Get picnum from global variable

        var dropdownInstance = dropdown.GetComponent<Dropdown>();
        dropdownInstance.value = originalPic; // Select option on frontend


    }

    private void displayBio(){
        // Display original bio on frontend

        string originalBio = Global.bio; // TODO Get bio from global variable

        var bioInstance = bioInput.GetComponent<InputField>();
        bioInstance.text = originalBio;
    }

    private void displayScreenName(){
        // Display original screenName on frontend

        string originalScreenname = Global.username; // TODO - Get screenname from global variable

        var screennameInstance = screenNameInput.GetComponent<InputField>();
        screennameInstance.text = originalScreenname;
    }

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
