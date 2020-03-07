const express = require('express');
const MongoClient = require('mongodb').MongoClient;
const router = express.Router();
const dbconfig = require('../dbconfig.json');
const assert = require('assert');


router.use(express.urlencoded({extended:false}));
router.get('/:verify', (req, res) => {
    const verify = req.params.verify;
    console.log('verifying email');

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');

            /* Find a user with given verification string. That user will be marked
               as verified. */
            db.collection('users').updateOne(
                { verifyStr: verify },
                { $set: { verifyStr: "verified", verified: true } }
            ).then(result => {
                if (result.modifiedCount != 1) {
                    console.log('failed to verify email or email was already verified');
                    res.status(400).send(`failed to verify email or email was already verified`);
                } else {
                    console.log('verified email');
                    res.status(200).send('successfully verified email');
                }
                client.close();
                return;
            }).catch(err => {
                console.log(err);
                res.status(400).send(err);
                client.close();
                return;
            });
        });
    } catch (err) {
        console.log(err);
        res.status(400).send("Could not verify email");
    }
});

module.exports = router;