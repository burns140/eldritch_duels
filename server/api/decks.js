const express = require('express');
const MongoClient = require('mongodb').MongoClient;
const ObjectID = require('mongodb').ObjectID;
const router = express.Router();
const assert = require('assert');
const dbconfig = require('../dbconfig.json');
const verify = require('../verifyjwt');

router.use(express.urlencoded({extended: false}));
router.use(verify);
router.use(express.json());
router.get('/get/:id', (req, res) => {
    const id = req.params.id;
    
    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
            /* Return all decks for this user */
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                console.log(result.decks);
                res.status(200).json({"decks": result.decks});
                client.close();
            }).catch(err => {
                console.log(err);
                res.status(400).send(err);
                client.close();
            });
        });
    } catch (err) {
        console.log(err);
        res.status(400).send(err);
    }
});

router.post('/save/:id', (req, res) => {
    const id = req.params.id;
    const deck = req.body.deck;

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
                        res.status(400).send('Deck with that name already exists');
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
                    res.status(200).send('Deck added succesfully');
                    client.close();
                    return;
                }).catch(err => {
                    console.log(err);
                    res.status(400).send(err);
                    client.close();
                    return;
                });
            }).catch(err => {
                console.log(err);
                res.status(400).send(err);
                client.close();
                return;
            });
        });
    } catch (err) {
        console.log(err);
        res.status(400).send(err);
    }
});

router.post('/delete/:id', (req, res) => {
    const id = req.params.id;
    const deckname = req.body.deckname;

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
                res.status(200).send('deck successfully deleted');
                client.close();
                return;
            }).catch(err => {
                console.log(err);
                res.status(400).send(err);
                client.close();
                return;
            });
        });
    } catch(err) {
        console.log(err);
        res.status(400).send(err);
    }
});

module.exports = router;
