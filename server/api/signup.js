const express = require('express');
const MongoClient = require('mongodb').MongoClient;
const router = express.Router();
const dbconfig = require('../dbconfig.json');
const assert = require('assert');
const bcrypt = require('bcrypt');
var fs = require('fs');

router.use(express.json());
router.post('/', (req, res) => {
    console.log('signup');

    console.log(req.body);
    const email = req.body.email;
    const username = req.body.username;
    const password = req.body.password;
    const hash = bcrypt.hashSync(password, 10);


    // TODO: SET DEFAULT AVATAR USING GRIDFS
    // TODO: TEST

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');

            db.collection('users').find({                               // Query for account with matching email
                email: email
            }).limit(1).count().then(result => {
                if (result != 0) {                                      // Fail if the email already exists
                    console.log(`User with email ${email} already exists`);
                    res.status(400).send('User with that email already exists');
                    client.close(); 
                } else {                                                // Create user using our user schema
                    let data = fs.readFileSync('./../resources/generic-avatar.jpg', 'utf-8');
                    //let encodedImage = new Buffer(data, 'binary').toString('base64');
                    console.log('image encoded');
                    db.collection('users').insertOne({
                        user_name: username,                // string
                        password: hash,                     // string
                        email: email,                       // string
                        achievements: [],                   // int[]
                        cards: [],                          // int[]
                        decks: [],                          // list<Tuple<string, int[]>>
                        avatar: [],                         // byte[]
                        level: 0,                           // int
                        wins: 0,                            // int
                        losses: 0,                          // int
                        recent_games: [],                   // bool[]
                        currency: 500                       // int
                    }).then(result => {
                        console.log(`User with email ${email} successfully created`);
                        res.status(201).send('User successfully created');
                        client.close();
                    }).catch(err => {
                        console.log(err);
                        res.status(400).send(err);
                        clilent.close();
                    });
                }
            });
        });
    } catch (err) {
        console.log(err);
    }
});

module.exports = router;