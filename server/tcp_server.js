const assert = require('assert');
const bcrypt = require('bcrypt');
const net = require('net');
const Signup = require('./tcp_handling/signup.js');
const Login = require('./tcp_handling/login.js');
const Decks = require('./tcp_handling/decks.js');
const Collection = require('./tcp_handling/collection.js');

/* Create server */
const host = 'localhost';
const port = process.env.port || 8000;
var server = net.createServer(onClientConnected);

server.listen(port, host, () => {
    console.log(`server listening on ${server.address().address}, port ${server.address().port}`);
});


function onClientConnected(sock) {
    let remoteAddress = `${sock.remoteAddress}:${sock.remotePort}`;
    console.log(`new client connectioned: ${remoteAddress}`);

    /* Determine what needs to be done */
    sock.on('data', (data) => {
        const obj = JSON.parse(data);               // Turn data into a JSON object
        try {       
            console.log(obj);
            switch (obj.cmd) {
                case "signup":
                    Signup.signup(obj, sock);
                    break;
                case "login":
                    Login.login(obj, sock);
                    break;
                case "getDecks":
                    Decks.getAllDecks(obj, sock);
                    break;
                case "saveDeck":
                    Decks.saveDeck(obj, sock);
                    break;
                case "deleteDeck":
                    Decks.deleteDeck(obj, sock);
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
            }
        } catch (err) {
            console.log(`tcp: ${err}`);
        }
    });

    /* There was a problem */
    sock.on('error', (err) => {
        console.log(`sock: ${err}`);
    });

    /* Connection closed gracefully */
    sock.on('close', () => {
        console.log('connection closed');
    })
}
