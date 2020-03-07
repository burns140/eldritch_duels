const bcrypt = require('bcrypt');
const dbconfig = require('../dbconfig.json');
const jwt = require('jsonwebtoken');
const AllPlayerList = require('../classes/AllPlayerList.js');
const server = require('../tcp_server.js');


//const MongoClient = require('../mongo_connection');

const login = (data, sock) => {
    const email = data.email;       // input email
    const password = data.password; // input password

    try {
        //MongoClient.get().then(client => {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            const db = client.db('eldritch_data');

            /* Query for an account with a matching email */
            db.collection('users').findOne({                               
                email: email
            }).then(result => {
                if (!result) {
                    sock.write('Account with that email doesn\'t exist');     // No result
                    return;
                } else {
                    var foundPass = false;          // flag to track whether the array contains the correct pass
                    var rightpass = '';             // determines which password is correct
                    passwords = result.password;

                    /* Iterate through array of user's passwords. If one matches, flag it and note the correct one */
                    for (var i of passwords) {
                        if (bcrypt.compareSync(password, i)) {
                            foundPass = true;
                            rightpass = i;
                            break;
                        }
                    }
                    if (foundPass) {                // Password matched hash

                        /* Return a token */
                        const token = jwt.sign({
                            data: {
                                id: result._id,
                                email: result.email
                            }
                        }, dbconfig.jwt_key);

                        const idString = result._id.toString();

                        if (!result.verified) {
                            sock.write("Not verified, can't login");
                            return; 
                        }
                        
                        sock.write(`${token}:${idString}:${result.avatar}:${result.username}:${result.bio}`); // Write token and profile info back
                        console.log('login successful; token returned');

                        var playList = server.getPlayList();        // Get list of all ids currently connected

                        /* Check if player is already logged in somewhere */
                        if (playList.isLoggedIn(idString)) {
                            var prevSocket = playList.getSocketByKey(idString);
                            try {
                                //prevSocket.end('Another device has logged in with this account');
                                console.log('forcing socket to close');
                            } catch (err) {
                                console.log(err);
                            }
                            playList.removePlayer(idString);
                        }

                        /* Add username: socket pair to a map in case we need to force
                           close the connection later */
                        playList.addPlayer(idString, sock);
                        console.log(playList);
                        
                        
                        /* If password array length is greater than 1, the user had requested a temporary password.
                           The program is designed so that their new password will be the password they logged in with 
                           most recently after requesting a temp password */
                        if (passwords.length > 1) {
                            var tempPass = [];
                            tempPass.push(rightpass);           // Create new array with the only element being the new password
                            console.log(`${email}: ${tempPass}`)

                            /* Set the password of the user that just logged in to the password they used */
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
