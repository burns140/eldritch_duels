const assert = require('assert');
const bcrypt = require('bcrypt');
const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../dbconfig.json');
const jwt = require('jsonwebtoken');


const login = (data, sock) => {
    /* Parse data */
    const email = data.email;
    const password = data.password;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');

            db.collection('users').findOne({                               // Query for account with matching email
                email: email
            }).then(result => {
                if (!result) {
                    sock.write('Account with that email doesn\'t exist');     // No result
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
                        sock.write(`${token}:${result._id.toString()}`);
                        console.log('login successful; token returned');
                        client.close();
                        return;
                    } else {                                                            // Password didn't match hash
                        sock.write('Incorrect password');
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
        sock.write(err);
    }
}

exports.login = login;