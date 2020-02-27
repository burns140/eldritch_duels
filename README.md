# Eldritch Duels

CS407 Senior project. Lovecraftian card game.

## Installations

You need a C# package called Newtonsoft.Json. You can go [here](https://www.softwaretestinghelp.com/create-json-objects-using-c/) to walk through an installation.

You will need Unity version 2019.2.18f1. Download [Unity Hub](https://unity3d.com/get-unity/download).

## Dependencies

Install using NPM.
```
npm install
```

## Sending Requests to server

### Server Side

The server will parse requests that it receives into a JSON object. You can see the tests folder for an example on how to use the Newtonsoft package to send those objects using TCP. 

### Data needed for requests

Note: id always references the user's id, which is returned upon signup as the only value and login in the form "token:id"

Signup: {cmd: "signup", email, username, password}

Login: {cmd: "login", email, password}

Get Collection: {cmd: "getCollection, token, id}

Add Card to Collection: {cmd: "addCardToCollection", id, token, cardname}

**More will be added as work is completed**

## Usage

The following is an example of how to format a request to signup a user

```csharp
public class User {
    private string email;
    private string password;
    private string username;
    private string cmd;

    public User(string cmd, string email, string username, string password) {
        this.email = email;
        this.cmd = cmd;
        this.password = password;
        this.username = username;
    }
}

class Signup {
    User user = new User("signup", "email@email.com", "username", "password");
    string json = JsonConvert.SerializeObject(user);

    ...

    Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
    stream.Write(data, 0, data.Length);
    data = new Byte[256];
    string responseData = string.Empty;
    Int32 bytes = stream.Read(data, 0, data.Length);
    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    Console.WriteLine("Received: {0}", responseData);
}
```

The following is an example of receiving a JSON object and treating it as a C# object. For input "{"testcard": 0} it will output "testcard: 0". 

```csharp
responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

var obj = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseData);
foreach(var key in obj.Keys) {
    string str = $"{key}: {obj[key]}";
    Debug.WriteLine(str);
}
```

