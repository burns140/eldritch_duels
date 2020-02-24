const assert = require('assert');
const bcrypt = require('bcrypt');
const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../dbconfig.json');

const signup = (data, sock) => {

    /* Parse data */
    const email = data.email;
    const username = data.username;
    const password = data.password;
    const hash = bcrypt.hashSync(password, 10);     // Hash password

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
                        "testcard": 0,
                        "realcard": 2,
                        "testing": 1,
                        "fake": 0
                    };
                    db.collection('users').insertOne({
                        user_name: username,                // string
                        password: temparr,                  // string
                        email: email,                       // string
                        achievements: [],                   // int[]
                        collection: startCollection,        // Map<cardname, amount>
                        decks: [],                          // list<Tuple<string, string[]>>
                        avatar: [],                         // byte[]
                        level: 0,                           // int
                        wins: 0,                            // int
                        losses: 0,                          // int
                        recent_games: [],                   // bool[]
                        currency: 500                       // int
                    }).then(result => {
                        console.log(`User with email ${email} and id ${result.insertedId} successfully created`);
                        sock.write(`User successfully created with id ${result.insertedId}`);
                        client.close();
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