public void copySharedDeck() {
    CopySharedDeckRequest req = new CopySharedDeckRequest(deckname, "copySharedDeck", myid, validToken);
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}


public void blockUser() {
    BlockUserRequest req = new BlockUserRequest(otherEmail, myEmail, "blockUser", myid, validToken);
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}

public void getBlockedUsers() {
    Request req = new Request(myid, validToken, "getBlockedUsers");
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}

public void getBlockedBy() {
    Request req = new Request(myid, validToken, "getBlockedByUsers");
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}

public void unblockUser() {
    BlockUserRequest req = new BlockUserRequest(myemail, otherEmail, "unblockUser", myid, validToken);
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}


public void resendVerificationEmail() {
    ResendVerifyRequest req = new ResendVerifyRequest(myemail, "resendVerify", myid, validToken);
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}


public void newSignup() {
    User user1 = new User("signup", myemail, password, username);
    string json = JsonConvert.SerializeObject(user1);
    string res = sendNetworkRequest(json);
}


public void shareDeck() {
    ShareDeckRequest req = new ShareDeckRequest(myemail, deckname, "shareDeck", myid, validToken);
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}

public void login() {
    User user = new User("login", myemail, password, username);
    string json = JsonConvert.SerializeObject(user);
    string res = sendNetworkRequest(json);
}


public void getCollectionArray() {
    UserInfo info = new UserInfo(myid, validToken, "getCollection");
    string json = JsonConvert.SerializeObject(info);
    string res = sendNetworkRequest(json);
}


public void addCardToCollection() {
    AddCardRequest cardRequest = new AddCardRequest(id, validToken, cardname, "addCardToCollection");
    string json = JsonConvert.SerializeObject(cardRequest);
    string res = sendNetworkRequest(json);
}


public void addDeck() {
    Deck deck = new Deck("saveDeck", deckname, myid, deckArray);
    string json = JsonConvert.SerializeObject(deck);
    string res = sendNetworkRequest(json);
}


public void getAllDecks() {
    GetAllDecksRequest req = new GetAllDecksRequest(myid, validToken, "getAllDecks");
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}


public void getOneDeck() {
    GetOneDeckRequest req = new GetOneDeckRequest(deckname, myid, validToken, "getOneDeck");
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}

public void deleteDeck() {
    DeleteDeckRequest req = new DeleteDeckRequest(deckname, myid, validToken, "deleteDeck");
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}

public void getPass() {
    PassRequest req = new PassRequest(email, "tempPassword");
    string json = JsonConvert.SerializeObject(req);
    string res = sendNetworkRequest(json);
}

// not sure if this one is actually correct
public void tempPass() {
    User user = new User("login", emailtosend, temppass, username);
    string json = JsonConvert.SerializeObject(user);
    string res = sendNetworkRequest(json);
}

/* Code that is used to get and send all network requests */
public string sendNetworkRequest(string obj) {
    Byte[] data = System.Text.Encoding.ASCII.GetBytes(obj);
    NetworkStream stream = Global.client.GetStream();

    stream.Write(data, 0, data.Length);
    data = new Byte[256];
    string responseData = string.Empty;

    Int32 bytes = stream.Read(data, 0, data.Length);
    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

    return responseData;
}