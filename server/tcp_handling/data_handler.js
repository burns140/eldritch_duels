﻿const Signup = require('./signup.js');
const Login = require('./login.js');
const Decks = require('./decks.js');
const Collection = require('./collection.js');
const Verify = require('../verifyjwt.js')
const Email = require('./sendemail.js');
const Profile = require('./editprofile.js');
const Queue = require('./queue.js');

const noTokenNeeded = ["signup", "login", "tempPass"];

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
                    case "saveDeck":                    // Save a deck to an account
                        Decks.saveDeck(obj, sock);
                        break;
                    case "deleteDeck":                  // Delete a deck from an account
                        Decks.deleteDeck(obj, sock);
                        break;
                    case "getOneDeck":                  // Get a single deck object from an account
                        Decks.getDeck(obj, sock);
                        break;
                    case "getCollection":               // Get the entire collection for an account
                        Collection.getCollection(obj, sock);
                        break;
                    case "addCardToCollection":         // Add a card to an account's collection
                        Collection.addCard(obj, sock);
                        break;
                    case "removeCardFromCollection":    // Remove a card from an account's collection
                        Collection.removeCard(obj, sock);
                        break;
                    case "tempPass":                    // Request an email with a temporary password
                        Email.resetPassword(obj, sock);
                        break;
                    case "editProfile":                 // Edit a user's profile
                        Profile.editProfile(obj, sock);
                        break;
                    case "changePassword":              // Change a user's password
                        Profile.changePassword(obj, sock);
                        break;
                    case "changeEmail":                 // Change a user's email
                        Profile.changeEmail(obj, sock);
                        break;
                    case "deleteAccount":               // Delete a user's account
                        Profile.deleteAccount(obj, sock);
                        break;
                    case "resendVerify":                // resend verification email
                        Email.resendVerification(obj, sock);
                        break;
                    case "shareDeck":
                        Email.resendVerification(obj, sock);
                        break;
                    case "enterQueue":                 // Enter matchmaking queue
                        Queue.enterQueue(obj, sock, onClientConnected);
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

module.exports = onClientConnected;