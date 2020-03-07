const bcrypt = require('bcrypt');
const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;

const editProfile = (data, sock) => {
    const username = data.username;
    const avatar = data.avatar;
    const bio = data.bio;
    const id = data.id;


    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                {
                    $set: { username: username, avatar: avatar, bio: bio }
                }
            ).then(result => {
                if (result.modifiedCount != 1) {
                    console.log('modified not 1');
                    sock.write(`Failed to update profile correctly`);
                } else {
                    console.log('successfully updated');
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

const deleteAccount = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').deleteOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result.nRemoved != 1) {
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

const changePassword = (data, sock) => {
    const id = data.id;
    var newPass = data.pass;
    newPass = bcrypt.hashSync(newPass, 10);     // Hash password


    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            var pass = [`${newPass}`];

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

const changeEmail = (data, sock) => {
    const id = data.id;
    const newemail = data.email;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            db.collection('users').find({
                _id: ObjectID(id)
            }).limit(1).count().then(result => {
                if (result != 0) {
                    console.log(`User with email ${newemail} already exists`);
                    sock.write('User with that email already exists');
                    return;
                } else {
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