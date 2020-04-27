const assert = require('assert').strict;
const Match = require('../classes/Match.js');
const MongoClient = require('../mongo_connection');
const ObjectId = require('mongodb').ObjectId;
const PlayerQueue = require('../classes/PlayerQueue');

var queue = new PlayerQueue();

/** 
 *  id => socket
 *  @type {Object.<string|number, import('net').Socket}
 * */
let clientConnections = {};

// these are used for operations in this file
/**
 * this is the data handler (from tcp_server.js)
 * @typedef {(data: string)} DataHandler
 * @type {DataHandler} 
 * */
let dataHandler = null;

/** 
 *  @type {number}
 *  the time between calls to matchPlayer
 * */
let timeBetweenAutoMatch = 1000;

// we'll be using timers to repeatedly call the matchPlayers() method
//   this is so that when players with sufficiently different elos join
//   there will be time for players with similar elos to join
/** @type {NodeJS.Timeout} */
let matchPlayersInterval;

/**
 * sets up values for this project
 * @param {(data: string)} dh - data handler for the socket
 * @param {number} ms - milliseconds between each call to match players
 */
exports.init = function (dh, ms) {
    dataHandler = dh;
    timeBetweenAutoMatch = ms || timeBetweenAutoMatch;

    clearTimeout(matchPlayersInterval);
    matchPlayersInterval = setTimeout(matchPlayers, timeBetweenAutoMatch);
}

/**
 * places player in queue for a match
 * @param {any} obj
 * @param {import('net').Socket} sock
 */
function enterQueue(obj, sock) {
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
    sock._temp_remove = removePlayer;

    matchPlayers();
}

/** finds a match between two players */
function matchPlayers() {
    queue.matchPlayers().then(matches => {
        if (queue.size >= 2)
            setTimeout(matchPlayers, timeBetweenAutoMatch);

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
                let match = new Match(dataHandler);

                // get sockets
                /** @type {import("net").Socket[]}  */
                let connections = matches.map(id => clientConnections[id]);

                for (let i = 0; i < matches.length; i++) {
                    // add player to match
                    match.addPlayer(matches[i], connections[i]);

                    // player cannot be removed after they are matched
                    connections[i].removeListener('close', connections[i]._temp_remove);
                    delete connections[i]._temp_remove;

                    // player is now not using the old data handler
                    connections[i].removeListener('data', dataHandler);
                }

                // notify the waiting users
                let matchID = `${users[0]} vs ${users[1]}`;
                connections[0].write(`match found: ${users[1]}\n${matchID}\n`);
                connections[1].write(`match found: ${users[0]}\n${matchID}\n`);

                

                return true;
            });
        }).catch(e => console.log(e));
    });
}

function checkMatchBan(data, sock) {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').findOne(
                { _id: ObjectId(id) }
            ).then(result => {
                if (result == null) {
                    throw new Error('no user found');
                }

                if (result.matchmakeBan > 0) {
                    if (Date.now() > result.matchmakeBan) {
                        db.collection('users').updateOne(
                            { _id: ObjectId(id) },
                            { $set: { matchmakeBan: 0 } }
                        ).then(result => {
                            if (result.modifiedCount != 1) {
                                throw new Error('failed to update ban length');
                            }
                            sock.write('can queue')
                        }).catch(err => {
                            console.log(err);
                            sock.write(err.toString());
                        });
                    } else {
                        console.log('matchmakeBan');
                        sock.write('matchmake ban');
                    }
                } else {
                    sock.write('can queue');
                    console.log('can queue');
                }
            })
        })
    } catch (err) {
        console.log(err);
        sock.write(err.toString());
    }
}

exports.enterQueue = enterQueue;
exports.checkMatchBan = checkMatchBan;