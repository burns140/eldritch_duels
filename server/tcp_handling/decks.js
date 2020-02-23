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
                console.log(result.decks);
                let obj = {
                    "decks": result.decks
                }
                sock.write(obj);
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

const saveDeck = (data, sock) => {
    const id = data.id;
    const deck = data.deck;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                /* Confirm that this deck doesn't have the same name as another on this account */
                for (el of result.decks) {
                    if (el.name == deck.name) {
                        console.log(`Deck with name ${el.name} already exists for id ${id}`);
                        sock.write('Deck with that name already exists');
                        client.close();
                        return;
                    }
                }

                /* Add the deck */
                db.collection('users').updateOne(
                    { _id: ObjectID(id) },
                    {
                        $push: { decks: deck }
                    }
                ).then(result => {
                    console.log(`Deck w/ name ${deck.name} added successfully`);
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