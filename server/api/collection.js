const express = require('express');
const MongoClient = require('mongodb').MongoClient;
const ObjectID = require('mongodb').ObjectID;
const router = express.Router();
const assert = require('assert');
const dbconfig = require('../db_config.json');
const verify = require('../verifyjwt');

router.use(express.urlencoded({extended: false}));
router.use(verify);
router.use(express.json());
router.get('/:id', (req, res) => {
    const id = req.params.id;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            
            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                console.log('cards: ' + result.cards);
                res.status(200).json({ cards: result.cards });
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
})