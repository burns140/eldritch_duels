const assert = require('assert');
const bcrypt = require('bcrypt');
const net = require('net');
const Signup = require('./tcp_handling/signup.js');
const Login = require('./tcp_handling/login.js');
const Decks = require('./tcp_handling/decks.js');
const Collection = require('./tcp_handling/collection.js');
const Verify = require('./verifyjwt.js')
const Email = require('./tcp_handling/sendemail.js');
const Profile = require('./tcp_handling/editprofile.js');
const PlayerQueue = require('./classes/PlayerQueue.js');
var queue = new PlayerQueue();

const noTokenNeeded = ["signup", "login", "tempPass"];

/* Create server */
const host = 'localhost';
const port = process.env.port || 8000;
var server = net.createServer(onClientConnected);

/* Start server */
server.listen(port, host, () => {
    console.log(`server listening on ${server.address().address}, port ${server.address().port}`);
});


/* This runs every time a new client connects */
function onClientConnected(sock) {
    let remoteAddress = `${sock.remoteAddress}:${sock.remotePort}`;
    console.log(`new client connectioned: ${remoteAddress}`);
    sock.setKeepAlive(true, 60000);

    /* Determine what needs to be done every time
       data is received from a client */
    sock.on('data', (data) => {        
        try {     
			const obj = JSON.parse(data);               // Turn data into a JSON object		
            console.log(obj);
            if (noTokenNeeded.includes(obj.cmd) || Verify.verify(obj.token, sock)) {        // Check that either no token is needed or the token is valid
                switch (obj.cmd) {
                    case "signup":                      // Signup new account
                        Signup.signup(obj, sock);
                        break;
                    case "login":                       // Login
                        Login.login(obj, sock);
                        break;
                    case "getAllDecks":                 // Get all deck names for an account
                        Decks.getAllDecks(obj, sock);
                        break;
                    case "saveDeck":
                        Decks.saveDeck(obj, sock);      // Save a deck to an account
                        break;
                    case "deleteDeck":
                        Decks.deleteDeck(obj, sock);    // Delete a deck from an account
                        break;
                    case "getOneDeck":                  
                        Decks.getDeck(obj, sock);       // Get a single deck object from an account
                        break;
                    case "getCollection":
                        Collection.getCollection(obj, sock);    // Get the entire collection for an account.
                        break;
                    case "addCardToCollection":
                        Collection.addCard(obj, sock);
                        break;
                    case "removeCardFromCollection":
                        Collection.removeCard(obj, sock);
                        break;
                    case "tempPass":
                        Email.resetPassword(obj, sock);
                        break;
                    case "editProfile":
                        Profile.editProfile(obj, sock);
                        break;
                    case "changePassword":
                        Profile.changePassword(obj, sock);
                        break;
                    case "changeEmail":
                        Profile.changeEmail(obj, sock);
                        break;
                    case "deleteAccount":
                        Profile.deleteAccount(obj, sock);
                        break;
                    case "enterQueue":
                        
                        break;
                    default:
                        sock.write('Not a valid command');
                        break;
                }
            }
        } catch (err) {
            console.log(`tcp: ${err}`);
        }
    });

    /* There was a problem */
    sock.on('error', (err) => {
        console.log('sockerr: ');
        console.log(err);
    });

    /* Connection closed gracefully */
    sock.on('close', () => {
        console.log('connection closed');
    })
}
