const bcrypt = require('bcrypt');
const MongoClient = require('../mongo_connection');
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
    const email = data.email;       // user's email
    const username = data.name;     // username
    const password = data.password; // password
    const hash = bcrypt.hashSync(password, 10);     // Hash password
    const host = "localhost:7999";

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Check if a user with that email already exists. If not, create account and send
               verification email. The user will not be able to play game until account is verified */
            db.collection('users').find({                               
                email: email
            }).limit(1).count().then(result => {
                if (result != 0) {                                      // Fail if the email already exists
                    console.log(`User with email ${email} already exists`);
                    sock.write('User with that email already exists');
                    return;
                } else {                                                // Create user using our user schema
                    var temparr = [];
                    temparr.push(hash);


                    var startCollection = {         // The collection they start with
                        "Test 0": 30,
                        "Test 1": 30,
                        "Test 2": 24,
                        "Test 3": 21,
                        "Test 4": 6,
                        "Test 5": 12,
                        "Test 6": 3,
                        "Test 7": 9,
                        "Test 8": 13,
                        "Test 9": 25,
                        "Test 10": 15,
                        "Test 11": 2,
                        "Test 12": 1,
                        "Test 13": 16,
                        "Test 14": 27,
                        "Test 15": 15,
                        "Test 16": 9,
                        "Test 17": 10,
                        "Test 18": 5,
                        "Test 19": 20,
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
                        verifyStr: str,                      // string
                        blocked: [],                        // array of user emails I've blocked
                        blockedBy: [],                      // array of user emails I've been blocked by
                        friendRequests: [],                 // array of user emails who want to friend me
                        friends: [],                        // array of user emails who are my friend
                        friendRequestsSent: [],             // array of user emails who I've sent friend requests to
                        reports: [],                        // array of reports I have of { user, time }
                        banLength: 0                          // length of current tempBan
                    }).then(result => {
                        console.log(`User with email ${email} and id ${result.insertedId} successfully created`);
                        sock.write(`User successfully created with id ${result.insertedId}. You must verify your email before you can play`);

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
                        return;
                    });
                }
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

exports.signup = signup;
