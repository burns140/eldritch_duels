const MongoClient = require('mongodb').MongoClient;
const ObjectID = require('mongodb').ObjectID;
const assert = require('assert');
const dbconfig = require('../dbconfig.json');
const verify = require('../verifyjwt');

const getAllDecks = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
            /* Return all decks for this user */
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                var temparr = [];
                var decks = result.decks;
                for (var el of decks) {
                    temparr.push(el.name);
                }
                console.log(temparr.toString());
                sock.write(temparr.toString());
                client.close();
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

const getDeck = (data, sock) => {
    const id = data.id;
    const deckname = data.name;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            
            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                var temparr = [];
                var decks = result.decks;

                for (var el of decks) {
                    if (el.name = deckname) {
                        for (var key of Object.keys(el)) {
                            if (key != deckname) {
                                temparr.push(`${key}-${coll[key]}`);
                            }
                        }
                    }
                }
                
                console.log(temparr.toString());
                sock.write(temparr.toString());
                client.close();
                return;
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
                return;
            });
        });
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

const saveDeck = (data, sock) => {
    const id = data.id;
    const newdeck = data.deck;
    const deckname = data.name;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                resDecks = result.decks;
                
                for (var i = 0; i < decks.length; i++) {
                    if (resDecks[i].name == deckname) {
                        resDecks.splice(i, 1);
                    }
                }

                var obj = {
                    deckname: deckname
                }

                for (el of newdeck) {
                    let temparr = el.split("-");
                    obj[temparr[0]] = temparr[1];
                }

                resDecks.push(obj);

                /* Add the deck */
                db.collection('users').updateOne(
                    { _id: ObjectID(id) },
                    {
                        $set: { decks: resDecks }
                    }
                ).then(result => {
                    console.log(`Deck w/ name ${deckname} added successfully`);
                    sock.write('Deck added succesfully');
                    client.close();
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    client.close();
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

const deleteDeck = (data, sock) => {
    const id = data.id;
    const deckname = data.deckname;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                {
                    $pull: { decks: { name: deckname } }
                }
            ).then(result => {
                console.log(`Deck ${deckname} successfully deleted`);
                sock.write('deck successfully deleted');
                client.close();
                return;
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
                return;
            });
        });
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

exports.getAllDecks = getAllDecks;
exports.saveDeck = saveDeck;
exports.deleteDeck = deleteDeck;