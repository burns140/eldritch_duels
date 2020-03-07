const assert = require('assert').strict;
const Match = require('../classes/Match.js');
const MongoClient = require('../mongo_connection');
const ObjectId = require('mongodb').ObjectId;
const PlayerQueue = require('../classes/PlayerQueue');

var queue = new PlayerQueue();
let clientConnections = {}; // id => socket

// pass oldDataHandler to avoid circular dependency
function enterQueue(obj, sock, oldDataHandler) {
    let id = obj.id;
    if (!queue.addPlayer(obj.id, 1000)) {
        sock.write('already queued');
        return;
    }

    // store socket with id as key
    clientConnections[id] = sock;
    console.log(`${id} queued`);
    sock.write('added to queue');

    let removePlayer = function() {
        queue.removePlayer(id);
        delete clientConnections[id];
    }

    // remove the player if they disconnect
    sock.once('close', removePlayer);

    queue.matchPlayers().then(matches => {
        if (matches.length == 0)
            return;

        assert(matches.length == 2, "Match should always be 2 long if it isn't empty");
        
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            // get userdata from matched users
            Promise.all([
                db.collection('users').findOne(ObjectId(matches[0])),
                db.collection('users').findOne(ObjectId(matches[1]))
            ]).then(results => {
                // verify the players exist
                for (let result of results)
                    if (!result)
                        throw new Error("Matched nonexistant player");

                // get usernames
                let users = results.map(u => u.username);

                // create a Match object
                let match = new Match();

                // get sockets
                let connections = matches.map(id => clientConnections[id]);

                for (let i = 0; i < matches.length; i++) {
                    // add player to match
                    match.addPlayer(match[i], connections[i]);

                    // player cannot be removed after they are matched
                    connections[i].removeListener('close', removePlayer);

                    // player is now not using the old data handler
                    connections[i].removeListener('data', oldDataHandler);
                }

                // notify the waiting users
                connections[0].write(`match found: ${users[1]}`);
                connections[1].write(`match found: ${users[0]}`);
            }).catch(e => {
                console.log(e);
            });
        }).catch(e => console.log(e));
    });
}

exports.enterQueue = enterQueue;