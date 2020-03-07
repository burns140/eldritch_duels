const assert = require('assert');
const bcrypt = require('bcrypt');
const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;

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
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            const db = client.db('eldritch_data');

            /* Find a user with the given id and set the three given values */
            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                {
                    $set: { username: username, avatar: avatar, bio: bio }
                }
            ).then(result => {
                if (result.modifiedCount != 1) {            // No document was modified, so error
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
                    db.collection('users').updateOne(
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

exports.deleteAccount = deleteAccount;
exports.editProfile = editProfile;
exports.changePassword = changePassword;
exports.changeEmail = changeEmail;
