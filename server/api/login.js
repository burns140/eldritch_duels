const express = require('express');
const MongoClient = require('mongodb').MongoClient;
const router = express.Router();
const dbconfig = require('../dbconfig.json');
const assert = require('assert');
const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');

router.use(express.json());
router.post('/', (req, res) => {
    const email = req.body.email;
    const password = req.body.password;
    
    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');

            db.collection('users').findOne({                               // Query for account with matching email
                email: email
            }).then(result => {
                if (!result) {
                    res.status(404).send('Account with that email doesn\'t exist');     // No result
                    client.close();
                    return;
                } else {
                    if (bcrypt.compareSync(password, result.password)) {                // Password matched hash
                        const token = jwt.sign({
                            data: {
                                id: result.id,
                                email: result.email
                            }
                        }, dbconfig.jwt_key, {expiresIn: '1d'});
                        res.status(200).json({token: token});
                        console.log('login successful; token returned');
                        client.close();
                        return;
                    } else {                                                            // Password didn't match hash
                        res.status(404).send('Incorrect password');
                        client.close();
                        return;
                    }
                }
            }).catch(err => {
                console.log(err);
                client.close();
                return;
            })
        });
    } catch (err) {
        console.log(err);
        res.status(400).send(err);
    }
});

module.exports = router;