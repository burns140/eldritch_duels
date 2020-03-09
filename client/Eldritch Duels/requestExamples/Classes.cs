using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestProject1 {

    public class User {
        public string email;
        public string password;
        public string username;
        public string cmd;
        public string token;

        public User(string cmd, string email, string password, string username) {
            this.email = email;
            this.cmd = cmd;
            this.password = password;
            this.username = username;
        }

        public User(string cmd, string email, string password, string username, string token) {
            this.email = email;
            this.cmd = cmd;
            this.password = password;
            this.username = username;
            this.token = token;
        }
    }

    public class UserInfo {
        public string id;
        public string token;
        public string cmd;

        public UserInfo(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    public class AddCardRequest {
        public string id;
        public string token;
        public string cardid;
        public string cmd;

        public AddCardRequest(string id, string token, string cardid, string cmd) {
            this.id = id;
            this.token = token;
            this.cardid = cardid;
            this.cmd = cmd;
        }
    }

    public class GetAllDecksRequest {
        public string id;
        public string token;
        public string cmd;

        public GetAllDecksRequest(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    public class GetOneDeckRequest : Request {
        public string name;

        public GetOneDeckRequest(string deckname, string id, string token, string cmd) : base(id, token, cmd) {
            this.name = deckname;
        }
    }

    public class DeleteDeckRequest : Request {
        public string name;

        public DeleteDeckRequest(string name, string id, string token, string cmd) : base(id, token, cmd) {
            this.name = name;
        }
    }

    public class PassRequest : Request {
        public string email;

        public PassRequest(string email, string cmd) : base(cmd) {
            this.email = email;
        }
    }

    public class ResendVerifyRequest : Request {
        public string email;

        public ResendVerifyRequest(string email, string cmd, string id, string token) : base(id, token, cmd) {
            this.email = email;
        }
    }

    public class ShareDeckRequest : Request {
        public string toEmail;
        public string deckname;

        public ShareDeckRequest(string toEmail, string deckname, string cmd, string id, string token) : base(id, token, cmd) {
            this.toEmail = toEmail;
            this.deckname = deckname;
        }
    }

    public class CopySharedDeckRequest : Request {
        public string deckname;

        public CopySharedDeckRequest(string deckname, string cmd, string id, string token) : base(id, token, cmd) {
            this.deckname = deckname;
        }
    }

    public class BlockUserRequest : Request {
        public string myEmail;
        public string toBlockEmail;

        public BlockUserRequest(string myEmail, string toBlockEmail, string cmd, string id, string token) : base(id, token, cmd) {
            this.myEmail = myEmail;
            this.toBlockEmail = toBlockEmail;
        }
    }

    public class Deck {
        public string cmd;
        public string name;
        public string id;
        public string[] deck;

        public Deck(string cmd, string name, string id, string[] deck) {
            this.cmd = cmd;
            this.name = name;
            this.id = id;
            this.deck = deck;
        }
    }
}
