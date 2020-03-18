const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;

/**
 * Get the name of all decks on a user's account
 * @param {object} data 
 * @param {object} sock 
 */
const getAllDecks = (data, sock) => {
    const id = data.id;         // user id
    const shared = data.shared; // is the deck shared

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Return all deck names for this user by adding each name to an array, 
               and converting that array to a string */
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                var temparr = [];
                var decks;
                if (!shared) {
                    decks = result.decks;
                } else {
                    decks = result.sharedwithme;
                }
                for (var el of decks) {             // Add each deckname to an array
                    temparr.push(el.deckname);
                }
                console.log(temparr.toString());
                if (temparr.toString() == "") {     // Account has no decks
                    sock.write("no decks");
                } else {
                    sock.write(temparr.toString());     // Convert array to string and write it to socket
                }
            }).catch(err => {
                console.log(err);
                sock.write(err);
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Get a specific deck object for a user
 * @param {object} data 
 * @param {object} sock 
 */
const getDeck = (data, sock) => {
    const id = data.id;             // user id
    const deckname = data.name;     // deckname
    const shared = data.shared;     // whether this is a shared deck

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a deck with a certain name, convert it to array, convert array to string,
               write it back */
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                var temparr = [];

                if (!shared) {
                    var decks = result.decks;   
                } else {
                    decks = result.sharedwithme;
                }

                for (var el of decks) {                     // iterate through all decks to find correct deck
                    if (el.deckname == deckname) {
                        for (var key of Object.keys(el)) {  // in that deck, iterate through object to get all key:value pairs
                            if (key != deckname && key != 'fromUser') {
                                temparr.push(`${key}-${el[key]}`);  // write key value pairs into array with element forms "key-value"
                            }
                        }
                    }
                }
                
                
                
                console.log(temparr.toString());
                sock.write(temparr.toString());     // Convert the array to a string and write it back
                return;
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        });
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Save a deck to a user's account
 * @param {object} data 
 * @param {object} sock 
 */
const saveDeck = (data, sock) => {
    const id = data.id;             // user id
    const newdeck = data.deck;      // deck object
    const deckname = data.name;     // deck name

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with matching id and save this deck to their account */
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                resDecks = result.decks;
                
                /* If a deck with this name already exists in this user's
                   deck array, remove it */
                for (var i = 0; i < resDecks.length; i++) {
                    if (resDecks[i].deckname == deckname) {
                        resDecks.splice(i, 1);
                        break;
                    }
                }

                /* Initialize object */
                var obj = {
                    deckname: deckname
                }

                /* For each element key:value of the deck array with string format "key-value" 
                   create an array of form [key, value] and add that field to the deck object to be stored*/
                for (el of newdeck) {
                    let temparr = el.split("-");
                    obj[temparr[0]] = temparr[1];
                }

                resDecks.push(obj);     // Push the newdeck onto the user's deck array

                /* Add the deck */
                db.collection('users').updateOne(
                    { _id: ObjectID(id) },
                    {
                        $set: { decks: resDecks }
                    }
                ).then(result => {
                    console.log(`Deck w/ name ${deckname} added successfully`);
                    sock.write('Deck added successfully');
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Delete a deck from the user's account
 * @param {object} data 
 * @param {object} sock 
 */
const deleteDeck = (data, sock) => {
    const id = data.id;             // user id
    const deckname = data.name;     // deck name
    const shared = data.shared;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find user with given id and, if a deck with the specified deckname
               exists in their deck array, remove it */
            if (!shared) {
                db.collection('users').updateOne(
                    { _id: ObjectID(id) },
                    {
                        $pull: { decks: { deckname: deckname } }
                    }
                ).then(result => {
                    console.log(`Deck ${deckname} successfully deleted`);
                    sock.write('deck successfully deleted');
                    // client.close()
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    // client.close()
                    return;
                });
            } else {
                db.collection('users').updateOne(
                    { _id: ObjectID(id) },
                    {
                        $pull: { sharedwithme: { deckname: deckname } }
                    }
                ).then(result => {
                    console.log(`Deck ${deckname} successfully deleted`);
                    sock.write('deck successfully deleted');
                    // client.close()
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    // client.close()
                    return;
                });
            }
        });
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Share a user's' deck with another user
 * @param {object} data 
 * @param {object} sock 
 */
const shareDeck = (data, sock) => {
    const myId = data.id;
    const shareEmail = data.toEmail;
    const deckname = data.deckname;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find the users that have email matching the receiver or ID matching the sender
               and convert results to an array */
            db.collection('users').find(
                {
                    $or: [ { email: shareEmail }, { _id: ObjectID(myId) } ]
                }
            ).toArray().then(result => {
                var decktopush;

                /* Find the deck to be pushed */
                if (result[0]._id == ObjectID(myId)) {
                    decktopush = findDeck(result[0], result[1], deckname);
                } else {
                    console.log(result[1] + ' and ' + result[0] +  ' and ' + deckname);
                    decktopush = findDeck(result[1], result[0], deckname);
                }

                if (!decktopush) {
                    console.log('that deck doesn\'t exist');
                    sock.write('that deck doesn\'t exist');
                    // client.close()
                    return;
                }

                db.collection('users').updateOne(
                    { email: shareEmail },
                    { $push: { sharedwithme: decktopush } }
                ).then(result => {
                    if (result.modifiedCount != 1) {
                        console.log("failed to share deck");
                        sock.write('failed to share deck');
                    } else {
                        console.log("shared deck");
                        sock.write('deck shared successfully');
                    }
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Copy a shared deck to my decks array, allowing me to edit it.
 * @param {object} data 
 * @param {object} sock 
 */
const copySharedDeck = (data, sock) => {
    const id = data.id;
    const deckname = data.deckname;

    try {
        // MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find the necessary user */
            db.collection('users').findOne(
                {_id: ObjectID(id)}
            ).then(result => {
                var deckToCopy;

                /* Find deck to copy in result array */
                for (deck of result.sharedwithme) {
                    if (deck.deckname == deckname) {
                        deckToCopy = deck;
                        break;
                    }
                }

                var newName = deckToCopy.deckname;
                var timesChanged = 0;
                var i = 0;
                for (el of result.decks) {
                    if (el.deckname == newName) {
                        deckToCopy = el;
                        tempArr = result.decks;
                        while (i < tempArr.length) {
                            if (tempArr[i].deckname == newName) {
                                timesChanged++;
                                newName = deckname + timesChanged.toString();
                                i = -1;
                            }
                            i++;
                        }
                        deckToCopy.deckname = newName;
                        break;
                    }
                }

                /* Push value onto decks array */
                db.collection('users').updateOne(
                    { _id: ObjectID(id) },
                    { $push: { decks: deckToCopy } }
                ).then(result => {
                    if (result.matchedCount != 1) {
                        console.log('failed to copy');
                        sock.write('failed to copy deck');
                    } else {
                        console.log(`successfully copied deck ${deckname}`);
                        sock.write('deck successfully copied');
                    }
                    // client.close()
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    // client.close()
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
                // client.close()
                return;
            });

        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Find the deck that will be shared with another user. Make sure that it has a unique name
 * @param {object} fromUser 
 * @param {object} toUser
 * @param {string} deckname 
 */
function findDeck(fromUser, toUser, deckname) {
    var newName = "";
    var newDeck = {};
    var timesChanged = 0;
    var i = 0;

    for (el of fromUser.decks) {
        if (el.deckname == deckname) {          // Find the deck to be shared
            newDeck = el;
            sharedArr = toUser.sharedwithme;
            newName = deckname

            /* Check to see whether the receiving user has a deck with the same name
               as the one being shared already in their shared array. If yes, we want to append
               a number to the end. Keep doing this until the name is unique. */
            while (i < sharedArr.length) {
                if (sharedArr[i].deckname == newName) {
                    timesChanged++;
                    newName = deckname + timesChanged.toString();
                    i = -1;
                }
                i++;
            }
            break;
        }
    }

    if (newDeck == {}) {
        return false;
    }

    newDeck.deckname = newName;
    newDeck.fromUser = fromUser.email;

    return newDeck;
}

exports.getAllDecks = getAllDecks;
exports.saveDeck = saveDeck;
exports.deleteDeck = deleteDeck;
exports.getDeck = getDeck;
exports.shareDeck = shareDeck;
exports.copySharedDeck = copySharedDeck;
