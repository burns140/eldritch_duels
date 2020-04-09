const net = require('net');
const Signup = require('./tcp_handling/signup.js');
const Login = require('./tcp_handling/login.js');
const Decks = require('./tcp_handling/decks.js');
const Collection = require('./tcp_handling/collection.js');
const Verify = require('./verifyjwt.js')
const Email = require('./tcp_handling/sendemail.js');
const Profile = require('./tcp_handling/profile.js');
const Block = require('./tcp_handling/blockUser.js');
const Friends = require('./tcp_handling/friends.js');
const Achievements = require('./tcp_handling/achievements.js');
const MatchEnd = require('./tcp_handling/matchEnd.js')
const AllPlayerList = require('./classes/AllPlayerList.js');
var playList = new AllPlayerList();
const Queue = require('./tcp_handling/queue.js');

const noTokenNeeded = ["signup", "login", "tempPass", "logout"];
const MongoClient = require('./mongo_connection');

/* Create server */
const host = 'localhost';
const port = process.env.port || 8000;
var server = net.createServer(onClientConnected);

var curDailies = [];        // ids of current daily challenges
var curWeeklies = [];       // ids of current weekly challenges

function dataHandler(data) {
    const sock = this;
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
                case "shareDeck":                  // Share deck with another user
                    Decks.shareDeck(obj, sock);
                    break;
                case "copySharedDeck":             // Copy a shared deck to my decks, allowing me to edit
                    Decks.copySharedDeck(obj, sock);
                    break;
                case "logout":                     // Logout
                    playList.removeSocket(sock);
                    sock.write("logged out");
                    break;
                case "enterQueue":                 // Enter matchmaking queue
                    Queue.enterQueue(obj, sock);
                    break;
                case "blockUser":                   // Block a user
                    Block.blockUser(obj, sock);
                    break;
                case "unblockUser":                 // Unblock a user
                    Block.unblockUser(obj, sock);
                    break;
                case "getBlockedUsers":             // Get array of blocked users for this user
                    Block.getBlockedUsers(obj, sock);
                    break;
                case "getBlockedByUsers":            //Get array of the users who have blocked me
                    Block.getBlockedByUsers(obj, sock);
                    break;
                case "sendFriendRequest":           // Send friend request to a user
                    Friends.sendFriendRequest(obj, sock);
                    break;
                case "acceptFriendRequest":         // Accept a friend request
                    Friends.acceptFriendRequest(obj, sock);
                    break;
                case "rejectFriendRequest":         // Reject a friend request
                    Friends.rejectFriendRequest(obj, sock);
                    break;
                case "removeFriend":                // Remove a friend from my friends list
                    Friends.removeFriend(obj, sock);
                    break;
                case "viewProfile":                 // Get info for a user's profile
                    Profile.viewProfile(obj, sock);
                    break;
                case "reportPlayer":                // Report a player
                    Profile.reportPlayer(obj, sock);
                    break;
                case "getAllFriends":               // Get a user's friends list
                    Friends.getAllFriends(obj, sock);
                    break;
                case "getAllUsernames":             // Get all usernames that exist
                    Friends.getAllUsernames(obj, sock);
                    break;
                case "openPack":                    // Open a pack
                    Collection.openPack(obj, sock);
                    break;
                case "getCredits":
                    Collection.getCredits(obj, sock);
                    break;
                case "updateCredits":
                    Collection.updateCredits(obj, sock);
                    break;
                case "getFriendRequests":
                    Friends.getFriendRequests(obj, sock);
                    break;
                case "getFriendRequestsSent":
                    Friends.getFriendRequestsSent(obj, sock);
                    break;
                case "getMyReportedPlayers":
                    Profile.getMyReportedPlayers(obj, sock);
                    break;
                case "setCustomAvatar":
                    Profile.setCustomAvatar(obj, sock);
                    break;
                case "getCustomAvatar":
                    Profile.getCustomAvatar(obj, sock);
                    break;
                case "getAchievements":
                    Achievements.getAchievements(obj, sock);
                    break;
                case "addWin":
                    MatchEnd.addWin(obj, sock);
                    break;
                case "addLoss":
                    MatchEnd.addLoss(obj, sock);
                    break;
                case "resolveAchievements":
                    MatchEnd.resolveAchievements(obj, sock);
                    break;
                default:                            // Command was invalid
                    sock.write('Not a valid command');
                    console.log('Not a valid command');
                    break;
            }
        }
    } catch (err) {
        console.log(`tcp: ${err}`);
    }
}

// initialize the Queue with the data handler and time for repeating the match making
Queue.init(dataHandler, 1500);

/* This runs every time a new client connects */
function onClientConnected(sock) {
    let remoteAddress = `${sock.remoteAddress}:${sock.remotePort}`;
    console.log(`new client connectioned: ${remoteAddress}`);
    sock.setKeepAlive(true, 60000);

    /* Determine what needs to be done every time
       data is received from a client */
    sock.on('data', dataHandler);

    /* There was a problem */
    sock.on('error', (err) => {
        console.log('sockerr: ');
        console.log(err);
    });

    /* Connection closed gracefully */
    sock.on('close', () => {
        console.log('connection closed');
        playList.removeSocket(sock);
    });

    sock.on('end', () => {
        console.log('connection ended');
        playList.removeSocket(sock);
    });
}

exports.getPlayList = () => {
    return playList;
};

console.log("establishing mongo client");

MongoClient.get().then((client) => {
    console.log("mongo client established");
    
    server.listen(port, host, () => {
        console.log(`server listening on ${server.address().address}, port ${server.address().port}`);
    });
}).catch(e => {
    console.log("Mongo db error");
    console.log(e);
});
