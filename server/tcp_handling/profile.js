const bcrypt = require('bcrypt');
const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;
const generator = require('generate-password');
const myEmail = 'eldritch.duels@gmail.com';
const dbconfig = require('../dbconfig.json');
const nodemailer = require('nodemailer');
const Binary = require('mongodb').Binary;


const TEMP_BAN_MARKS = [1, 3, 5, 10, 12] 
const TEMP_BAN_LENGTHS_MINS = [1, 10, 300, 1440, -1];


var transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: myEmail,
        pass: dbconfig.emailpass
    }
});

/**
 * Change configurable profile information
 * @param {object} data 
 * @param {object} sock 
 */
const editProfile = (data, sock) => {
    const username = data.username;     // username
    const avatar = data.avatar;         // avatar
    const bio = data.bio;               // bio
    const id = data.id;                 // user id


    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the given id and set the three given values */
            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                {
                    $set: { username: username, avatar: avatar, bio: bio }
                }
            ).then(result => {
                if (result.matchedCount != 1) {            // No document was modified, so error
                    console.log('modified not 1');
                    sock.write(`Failed to update profile correctly`);
                } else {
                    console.log('successfully updated');    // Success
                    sock.write('Profile successfully updated');
                }
                return;
            }).catch(err => {
                console.log(err);
                sock.write(`Failed to update profile`);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Delete a user's account
 * @param {object} data 
 * @param {object} sock 
 */
const deleteAccount = (data, sock) => {
    const id = data.id;     // user's id

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* If a user exists with given id, delete it */
            db.collection('users').deleteOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result.deletedCount != 1) {
                    console.log('account not found');
                    sock.write(`Failed to delete profile`);
                } else {
                    console.log('successfully deleted');
                    sock.write('Profile successfully deleted');
                }
                return;
            }).catch(err => {
                console.log(err);
                sock.write(`Failed to delete profile`);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Change a user's password
 * @param {object} data 
 * @param {object} sock 
 */
const changePassword = (data, sock) => {
    const id = data.id;             // user's id
    var newPass = data.pass;        // new password in plaintext
    newPass = bcrypt.hashSync(newPass, 10);     // Hash password


    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            var pass = [`${newPass}`];      // Set password array to only contain this new password

            /* Update account to have password array be the new array above */
            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                {
                    $set: { password: pass }
                }
            ).then(result => {
                if (result.modifiedCount != 1) {
                    console.log('modified not 1');
                    sock.write(`Failed to update password correctly`);
                } else {
                    console.log('successfully updated');
                    sock.write('password successfully updated');
                }
                return;
            }).catch(err => {
                console.log(err);
                sock.write(`Failed to update password`);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Change the email linked to a user's account
 * @param {object} data 
 * @param {object} sock 
 */
const changeEmail = (data, sock) => {
    const id = data.id;
    const newemail = data.email;
    const host = "localhost:7999";

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Check to see whether a user already exist with the requested new email */
            db.collection('users').find({
                email: newemail
            }).limit(1).count().then(result => {
                if (result != 0) {
                    console.log(`User with email ${newemail} already exists`); // Cannot update email because new email already exists
                    sock.write('User with that email already exists');
                    return;
                } else {

                    /* Find user with the given id and update their email */
                    /* db.collection('users').updateOne(
                        { _id: ObjectID(id) },
                        {
                            $set: { email: newemail }
                        }
                    ).then(result => {
                        if (result.modifiedCount != 1) {
                            console.log('modified not 1');
                            sock.write(`failed to update email`);
                        } else {
                            console.log('successfully updated');
                            sock.write('Email updated successfully');
                        }
                        return;
                    }).catch(err => {
                        console.log(err);
                        sock.write(`Failed to update email`);
                        return;
                    }); */

                    var str = generator.generate({  // The random string that will be appended to their verification link
                        length: 30,
                        numbers: true,
                        symbols: false
                    });

                    /* Set html to be sent for verification email */
                    var emailText = `<h1>Email Verification</h1>` +
                    `<p>You have requested to change you email for Eldritch Duels. To change your email, ` +
                    `click on the verification link below.</p>` +
                    `<a href="http://${host}/verify/${str}">Verify Account</a>`;

                    var mailOptions = {             // Options for verification email
                        from: myEmail,
                        to: newemail,
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

                    db.collection('users').updateOne(
                        { _id: ObjectID(id) },
                        {
                            $set: { verifyStr: str, emailToChange: newemail }
                        }
                    ).then(result => {
                        if (result.modifiedCount != 1) {
                            console.log('modified not 1');
                            sock.write(`failed to update email`);
                        } else {
                            console.log('successfully updated');
                            sock.write('Email sent successfully');
                        }
                        return;
                    }).catch(err => {
                        console.log(err);
                        sock.write(`Failed to update email`);
                        return;
                    });
                }
            }).catch(err => {
                console.log(err);
                sock.write('Failed to update email');
                return;
            })

            
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * View information on another user's profile
 * @param {object} data 
 * @param {object} sock 
 */
const viewProfile = (data, sock) => {
    const theirEmail = data.theirEmail;
    const errString = `couldn't retrieve profile`;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Get the other user's avatar, bio, and username */
            db.collection('users').findOne(
                { email: theirEmail }
            ).then(result => {
                if (!result) {
                    throw new Error(errString);
                }
                var temparr = [];
                temparr.push(`avatar-${result.avatar}`);
                temparr.push(`bio-${result.bio}`);
                temparr.push(`username-${result.username}`);
                sock.write(temparr.toString());
                console.log('profile found successfully');
            }).catch(err => {
                console.log(err);
                sock.write(err.toString());
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err).toString();
    }
}

/**
 * Report a player. Add this report to their array. If they have reached a mark, temp ban them.
 * @param {object} data 
 * @param {object} sock 
 */
const reportPlayer = (data, sock) => {
    var theirEmail = data.theirEmail;
    var myEmail = data.myEmail;
    
    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
    
            var report = {
                user: myEmail,
                date: Date.now()
            }
    
            console.log(report);
    
            /* Add this new report to their reports array */
            db.collection('users').updateOne(
                { email: theirEmail },
                { $push: { reports: report } }
            ).then(result => {
                if (result.modifiedCount != 1) {
                    throw new Error(`couldn't find user with that email OR user already reported by you`);
                }
    
                /* Add this user to my reported array */
                db.collection('users').updateOne(
                    { email: myEmail },
                    { $push: { reported: theirEmail } }
                ).then(result => {
                    if (result.modifiedCount != 1) {
                        throw new Error(`couldn't add them to my reported`);
                    }
    
                    /* Find the user who has been reported to see if they should be banned */
                    db.collection('users').findOne(
                        { email: theirEmail }
                    ).then(result => {
                        if (result == null) {
                            throw new Error(`couldn't find user with that email`);
                        }
    
                        var reports = result.reports;
    
                        for (var i = 0; i < TEMP_BAN_MARKS.length; i++) {
                            if (TEMP_BAN_MARKS[i] == reports.length) {
                                
                                /* The user should be temp banned. Update their banLength field */
                                db.collection('users').updateOne(
                                    { email: theirEmail },
                                    { $set: { banLength: Date.now() + TEMP_BAN_LENGTHS_MINS[i] * 60 * 1000 } }
                                    //{ $set: { banLength: TEMP_BAN_LENGTHS_MINS[i] * 60 * 1000 } }
                                ).then(result => {
                                    if (result.modifiedCount != 1) {
                                        throw new Error(`could not update ban field`);
                                    }
                                    console.log('temp ban added successfully');
                                }).catch(err => {
                                    console.log(err);
                                    sock.write(err);
                                    return;
                                });
                                break;
    
                            }
                        }
    
                        console.log('user reported successfullly');
                        sock.write('user reported successfully');
                    }).catch(err => {
                        console.log(err);
                        sock.write(err);
                        return;
                    });
                }).catch(err => {
                    console.log(err);
                    sock.write(err.toString());
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
            });
    
                
        });
    } catch (err) {
        console.log(err);
        sock.write(err.toString());
    }
    
}

const getMyReportedPlayers = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Get the other user's avatar, bio, and username */
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                    throw new Error('couldn\'t find user');
                }

                console.log('getting my reported users');
                if (result.reported.toString() == "") {
                    sock.write("noreportedusers");
                } else {
                    sock.write(result.reported.toString()); 
                }
                return;
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err.toString());
    }
}

const setCustomAvatar = (data, sock) => {
    const id = data.id;
    const pic = data.pic;

    const insertdata = Binary(pic);

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                { $set: { customArt: insertdata, avatar: -1 } }
            ).then(result => {
                if (result.matchedCount != 1) {
                    throw new Error('no user modified');
                }
                sock.write('Custom avatar uploaded');
                console.log('custom avatar uploaded')
            }).catch(err => {
                console.log(err);
                sock.write(err.toString());
            });
        })
    } catch (err) {
        sock.write(err.toString());
        console.log(err);
    }
}

const getCustomAvatar = (data, sock) => {
    const email = data.email;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').findOne(
                { email: email },
            ).then(result => {
                if (result == null) {
                    throw new Error('no user found');
                }
                sock.write(result.customArt);
                console.log('returned custom art');
            }).catch(err => {
                console.log(err);
                sock.write(err.toString());
            });
        })
    } catch (err) {
        sock.write(err.toString());
        console.log(err);
    }
}

exports.reportPlayer = reportPlayer;
exports.getMyReportedPlayers = getMyReportedPlayers;
exports.deleteAccount = deleteAccount;
exports.editProfile = editProfile;
exports.changePassword = changePassword;
exports.changeEmail = changeEmail;
exports.viewProfile = viewProfile;
exports.setCustomAvatar = setCustomAvatar;
exports.getCustomAvatar = getCustomAvatar;
