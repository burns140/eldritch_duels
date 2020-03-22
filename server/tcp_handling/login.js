const bcrypt = require('bcrypt');
const dbconfig = require('../dbconfig.json');
const jwt = require('jsonwebtoken');
const server = require('../tcp_server.js');
const MongoClient = require('../mongo_connection');

//const MongoClient = require('../mongo_connection');

const login = (data, sock) => {
    const email = data.email;       // input email
    const password = data.password; // input password

    try {
        MongoClient.get().then(client => {
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

                        /* Negative ban length mean this is a perma-ban */
                        if (result.banLength < 0) {
                            console.log('account permanently banned');
                            sock.write('This account has been permanently banned');
                        } else if (result.banLength > 0) {

<<<<<<< HEAD
                            /* If the current date is later than the date of the end of the ban, they can play */
                            if (Date.now() > result.banLength) {
                                db.collection('users').updateOne(
                                    { email: email },
                                    { $set: {banLength: 0 } }
                                ).then(result => {
                                    if (result.modifiedCount != 1) {
                                        throw new Error('failed to update ban length');
                                    }
                                    console.log('temp ban lifted');
                                }).catch(err => {
                                    console.log(err);
                                    sock.write(err.toString());
                                });
                            } else {
                                console.log('this account is temp banned');
                                sock.write('This account is temporarily banned');
                                return;
=======
                        /* If the value of this time minus the ban length is greater than the time at which they are banned, they are past the end of ban
                           Reset ban length to 0 to ensure they can login again */
                           if(result.reports != null){
                                var lastReport = result.reports[reports.length - 1];
                                
                                if (result.banLength > 0) {
                                    if (Date.now() - result.banLength > lastReport.date) {
                                        console.log('temp ban completed');
                                        banLength = 0;
                                    } else {
                                        console.log('this account is temp banned');
                                        sock.write('This account is temporarily banned');
                                        return;
                                    }
                                }
>>>>>>> 65807cd7d45c35187a47540fce3ace45a33e30a9
                            }

                        if (!result.verified) {
                            console.log('account not verified');
                            sock.write("Not verified, can't login");
                            return; 
                        }
                        
                        sock.write(`${token}:${idString}:${result.avatar}:${result.username}:${result.bio}`); // Write token and profile info back
                        console.log('login successful; token returned');

                        var playList = server.getPlayList();        // Get list of all ids currently connected

                        /* Check if player is already logged in somewhere */
                        if (playList.isLoggedIn(idString)) {
                            var prevSocket = playList.getSocketByKey(idString);
                            if (prevSocket != sock) {
                                try {
                                    prevSocket.end('Another device has logged in with this account');
                                    console.log('forcing socket to close');
                                } catch (err) {
                                    console.log(err);
                                }
                                playList.removePlayer(idString);
                            }
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
