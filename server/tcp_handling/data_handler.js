const Signup = require('./signup.js');
const Login = require('./login.js');
const Decks = require('./decks.js');
const Collection = require('./collection.js');
const Verify = require('../verifyjwt.js')
const Email = require('./sendemail.js');
const Profile = require('./editprofile.js');
const Queue = require('./queue.js');

const noTokenNeeded = ["signup", "login", "tempPass"];

function onClientConnected(sock) {
    let remoteAddress = `${sock.remoteAddress}:${sock.remotePort}`;
    console.log(`new client connectioned: ${remoteAddress}`);
    sock.setKeepAlive(true, 60000);

    /* Determine what needs to be done */
    sock.on('data', (data) => {
        try {
            const obj = JSON.parse(data);               // Turn data into a JSON object		
            console.log(obj);
            console.log(noTokenNeeded.includes(obj.cmd));
            if (noTokenNeeded.includes(obj.cmd) || Verify.verify(obj.token, sock)) {
                switch (obj.cmd) {
                    case "signup":
                        Signup.signup(obj, sock);
                        break;
                    case "login":
                        Login.login(obj, sock);
                        break;
                    case "getAllDecks":
                        Decks.getAllDecks(obj, sock);
                        break;
                    case "saveDeck":
                        Decks.saveDeck(obj, sock);
                        break;
                    case "deleteDeck":
                        Decks.deleteDeck(obj, sock);
                        break;
                    case "getOneDeck":
                        Decks.getDeck(obj, sock);
                        break;
                    case "getCollection":
                        Collection.getCollection(obj, sock);
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