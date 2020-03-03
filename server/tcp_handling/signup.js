const assert = require('assert');
const bcrypt = require('bcrypt');
const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../dbconfig.json');
const generator = require('generate-password');
const nodemailer = require('nodemailer');
const myEmail = 'eldritch.duels@gmail.com';

var transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: myEmail,
        pass: dbconfig.emailpass
    }
});


const signup = (data, sock) => {

    /* Parse data */
    const email = data.email;
    const username = data.name;
    const password = data.password;
    const hash = bcrypt.hashSync(password, 10);     // Hash password
    const host = "localhost:7999";

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');

            db.collection('users').find({                               // Query for account with matching email
                email: email
            }).limit(1).count().then(result => {
                if (result != 0) {                                      // Fail if the email already exists
                    console.log(`User with email ${email} already exists`);
                    sock.write('User with that email already exists');
                    client.close();
                    return;
                } else {                                                // Create user using our user schema
                    var temparr = [];
                    temparr.push(hash);
                    var startCollection = {
                        "Test 0": 30,
                        "Test 1": 30,
                        //"testing": 1,
                        //"fake": 0
                    };

                    var startDecks = [
                        {
                            deckname: "First Deck",
                            "Test 0": 20,
                            "Test 1": 12
                        }
                    ];

                    var str = generator.generate({
                        length: 30,
                        numbers: true,
                        symbols: true
                    });
                    
                    db.collection('users').insertOne({
                        username: username,                // string
                        password: temparr,                  // string
                        email: email,                       // string
                        achievements: [],                   // int[]
                        collection: startCollection,        // Map<cardname, amount>
                        decks: startDecks,                          // list<Tuple<string, string[]>>
                        avatar: 0,                         // int
                        level: 0,                           // int
                        wins: 0,                            // int
                        losses: 0,                          // int
                        recent_games: [],                   // bool[]
                        currency: 500,                       // int
                        bio: "enter_bio",
                        verified: false,                     // boolean of whether they verified their account
                        verifyStr: str
                    }).then(result => {
                        console.log(`User with email ${email} and id ${result.insertedId} successfully created`);
                        sock.write(`User successfully created with id ${result.insertedId}. You must verify your email before you can play`);
                        client.close();
                        var emailText = `<h1>Email Verification</h1>` +
                                    `<p>You have created an account for Eldritch Duels. Before you can ` +
                                    `access matchmaking, you must verify your account by clicking on the link below.</p>` +
                                    `<p>${host}/verify/${str}</p>`;

                        var mailOptions = {
                            from: myEmail,
                            to: email,
                            subject: 'Verify Account',
                            html: emailText
                        };

                        transporter.sendMail(mailOptions, (error, info) => {
                            if (error) {
                                console.log(err);
                            } else {
                                console.log('Email send: ' + info.response);
                            }
                        });

                        return;
                    }).catch(err => {
                        console.log(err);
                        sock.write(err);
                        client.close();
                        return;
                    });
                }
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

exports.signup = signup;