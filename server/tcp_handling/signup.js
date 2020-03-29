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
    //const host = "localhost:7999";
    const host = "66.253.158.241:7999";

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
                        "Mi_Go": 24,
                        "Mi_Go Worker": 21,
                        "Mi_Go Queen": 6,
                        "Beast Patient": 12,
                        "Chime Maiden": 3,
                        "Brain of Mensis": 9,
                        "Mi_Go Zombie": 13,
                        "Snatcher": 25,
                        "Nightmare Apostle": 15,
                        "Quicksilver Bullets": 2,
                        "Great One's Wisdom": 1,
                        "Blood Starved Beast": 16,
                        "Moon Presence": 27,
                        "Ludwig Holy Blade": 15,
                        "Blood Vial": 9,
                        "Lady Maria": 10,
                        "Pungent Blood Cocktail": 5,
                        "Madman's Knowledge": 20,
                    };

                    var startDecks = [              // The decks they will start with
                        {
                            deckname: "First Deck",
                            "Mi_Go": 20,
                            "Blood Vial": 12
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
                        credits: 100000,                       // int
                        bio: "enter_bio",                   // string
                        verified: false,                     // boolean of whether they verified their account
                        verifyStr: str,                      // string
                        blocked: [],                        // array of user emails I've blocked
                        blockedBy: [],                      // array of user emails I've been blocked by
                        friendRequests: [],                 // array of user emails who want to friend me
                        friends: [],                        // array of user emails who are my friend
                        friendRequestsSent: [],             // array of user emails who I've sent friend requests to
                        reports: [],                        // array of reports I have of { user, time }
                        banLength: 0,                          // length of current tempBan
                        emailToChange: "",                   // Email they are trying to change to
                        reported: [],
                        customArt: ""
                    }).then(result => {
                        console.log(`User with email ${email} and id ${result.insertedId} successfully created`);
                        sock.write(`User successfully created with id ${result.insertedId}. You must verify your email before you can play`);

                        /* Set html to be sent for verification email */
                        var emailText = `<h1>Email Verification</h1>` +
                                    `<p>You have created an account for Eldritch Duels. Before you can ` +
                                    `login, you must verify your account by clicking on the link below.</p>` +
                                    `<a href="http://${host}/verify/${str}">Verify Account</a>`;

                        var mailOptions = {             // Options for verification email
                            from: myEmail,
                            to: email,
                            subject: 'Verify Account',
                            html: emailText
                        };

                        transporter.sendMail(mailOptions, (error, info) => {    // Send verification email
                            if (error) {
                                console.log(error);
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

exports.signup = signup