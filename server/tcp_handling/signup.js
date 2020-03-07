const bcrypt = require('bcrypt');
const MongoClient = require('../mongo_connection');

const signup = (data, sock) => {
    /* Parse data */
    const email = data.email;
    const username = data.name;
    const password = data.password;
    const hash = bcrypt.hashSync(password, 10);     // Hash password

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').find({                               // Query for account with matching email
                email: email
            }).limit(1).count().then(result => {
                if (result != 0) {                                      // Fail if the email already exists
                    console.log(`User with email ${email} already exists`);
                    sock.write('User with that email already exists');
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
                        bio: "enter_bio"
                    }).then(result => {
                        console.log(`User with email ${email} and id ${result.insertedId} successfully created`);
                        sock.write(`User successfully created with id ${result.insertedId}`);
                        return;
                    }).catch(err => {
                        console.log(err);
                        sock.write(err);
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