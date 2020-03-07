const bcrypt = require('bcrypt');
const dbconfig = require('../dbconfig.json');
const jwt = require('jsonwebtoken');
const MongoClient = require('../mongo_connection');

const login = (data, sock) => {
    /* Parse data */
    const email = data.email;
    const password = data.password;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').findOne({                               // Query for account with matching email
                email: email
            }).then(result => {
                if (!result) {
                    sock.write('Account with that email doesn\'t exist');     // No result
                    return;
                } else {
                    var foundPass = false;
                    var rightpass = '';
                    passwords = result.password;
                    for (var i of passwords) {
                        if (bcrypt.compareSync(password, i)) {
                            foundPass = true;
                            rightpass = i;
                        }
                    }
                    if (foundPass) {                // Password matched hash
                        const token = jwt.sign({
                            data: {
                                id: result._id,
                                email: result.email
                            }
                        }, dbconfig.jwt_key);
                        sock.write(`${token}:${result._id.toString()}:${result.avatar}:${result.username}:${result.bio}`);
                        console.log('login successful; token returned');
                        if (passwords.length > 1) {
                            var tempPass = [];
                            tempPass.push(rightpass);
                            console.log(`${email}: ${tempPass}`)
                            db.collection('users').updateOne(
                                { email: email },
                                { $set: { password: tempPass } }
                            ).then(result => {
                                if (result.modifiedCount != 0) {
                                    console.log('successfully removed bad password');
                                    return;
                                } else {
                                    console.log('old password not successfully removed');
                                    return;
                                }
                            }).catch(err => {
                                console.log(err);
                            });
                        } else {
                            return;
                        }
                    } else {                                                            // Password didn't match hash
                        sock.write('Incorrect password');
                        return;
                    }
                }
            }).catch(err => {
                console.log(err);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err.msg);
    }
}

exports.login = login;