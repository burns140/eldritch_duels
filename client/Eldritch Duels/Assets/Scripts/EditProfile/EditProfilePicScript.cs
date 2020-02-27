using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        EditProfileRequest req = new EditProfileRequest("editProfile", Global.getID(), Global.getToken(), bio, avatar, username);
        string json = JsonConvert.SerializeObject(req);
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
        
        Global.stream.Write(data, 0, data.Length);
        data = new Byte[256];
        string responseData = string.Empty;

        Int32 bytes = stream.Read(data, 0, data.Length);
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

        int originalPic = 0; // TODO - Get picnum from global variable

        var dropdownInstance = dropdown.GetComponent<Dropdown>();
        dropdownInstance.value = originalPic; // Select option on frontend


    }

    private void displayBio(){
        // Display original bio on frontend

        string originalBio = "Enter Bio"; // TODO Get bio from global variable

        var bioInstance = bioInput.GetComponent<InputField>();
        bioInstance.text = originalBio;
    }

    private void displayScreenName(){
        // Display original screenName on frontend

        string originalScreenname = "Enter Screenname"; // TODO - Get screenname from global variable

        var screennameInstance = screenNameInput.GetComponent<InputField>();
        screennameInstance.text = originalScreenname;
    }

    public class EditProfileRequest {
        public string cmd;
        public string id;
        public string token;
        public string bio;
        public string avatar;
        public string username;

        EditProfile (string cmd, string id, string token, string bio, string avatar, string username) {
            // constructor
        }
    }

}
