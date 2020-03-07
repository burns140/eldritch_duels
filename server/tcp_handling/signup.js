const assert = require('assert');
const bcrypt = require('bcrypt');
const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../dbconfig.json');
const generator = require('generate-password');
const nodemailer = require('nodemailer');
const myEmail = 'eldritch.duels@gmail.com';

var startCollection = {
    "Test 0": 30,
    "Test 1": 30
};

var transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: myEmail,
        pass: dbconfig.emailpass
    }
});


const signup = (data, sock) => {

    /* Parse data */
    const email = data.email;       // user's email
    const username = data.name;     // username
    const password = data.password; // password
    const hash = bcrypt.hashSync(password, 10);     // Hash password
    const host = "localhost:7999";

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');

            /* Check if a user with that email already exists. If not, create account and send
               verification email. The user will not be able to play game until account is verified */
            db.collection('users').find({                               
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


                    var startCollection = {         // The collection they start with
                        "Test 0": 30,
                        "Test 1": 30,
                        //"testing": 1,
                        //"fake": 0
                    };

                    var startDecks = [              // The decks they will start with
                        {
                            deckname: "First Deck",
                            "Test 0": 20,
                            "Test 1": 12
                        }
                    ];

                    var str = generator.generate({  // The random string that will be appended to their verification link
                        length: 30,
                        numbers: true,
                        symbols: false
                    });
                    
                    db.collection('users').insertOne({
                        username: username,                // string
                        password: temparr,                  // string
                        email: email,                       // string
                        achievements: [],                   // int[]
                        collection: startCollection,        // Map<cardname, amount>
                        decks: startDecks,                          // list<Tuple<string, string[]>>
                        sharedwithme: [],
                        avatar: 0,                         // int
                        level: 0,                           // int
                        wins: 0,                            // int
                        losses: 0,                          // int
                        recent_games: [],                   // bool[]
                        currency: 500,                       // int
                        bio: "enter_bio",                   // string
                        verified: false,                     // boolean of whether they verified their account
                        verifyStr: str                      // string
                    }).then(result => {
                        console.log(`User with email ${email} and id ${result.insertedId} successfully created`);
                        sock.write(`User successfully created with id ${result.insertedId}. You must verify your email before you can play`);
                        client.close();

                        /* Set html to be sent for verification email */
                        var emailText = `<h1>Email Verification</h1>` +
                                    `<p>You have created an account for Eldritch Duels. Before you can ` +
                                    `access matchmaking, you must verify your account by clicking on the link below.</p>` +
                                    `<a href="http://${host}/verify/${str}">Verify Account</a>`;

                        var mailOptions = {             // Options for verification email
                            from: myEmail,
                            to: email,
                            subject: 'Verify Account',
                            html: emailText
                        };

                        transporter.sendMail(mailOptions, (error, info) => {    // Send verification email
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

exports.signup = signup;