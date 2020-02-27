const assert = require('assert');
const bcrypt = require('bcrypt');
const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../dbconfig.json');
const ObjectID = require('mongodb').ObjectID;


const editProfile = (data, sock) => {
    const username = data.username;
    const avatar = data.avatar;
    const bio = data.bio;
    const id = data.id;


    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');

            db.collections('users').updateOne(
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
                client.close();
                return;
            }).catch(err => {
                console.log(err);
                sock.write(`Failed to update profile`);
                client.close();
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
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');

            db.collections('users').remove(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result.nRemoved != 1) {
                    console.log('account not found');
                    sock.write(`Failed to delete profile`);
                } else {
                    console.log('successfully deleted');
                    sock.write('Profile successfully deleted');
                }
                client.close();
                return;
            }).catch(err => {
                console.log(err);
                sock.write(`Failed to delete profile`);
                client.close();
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

exports.deleteAccount = deleteAccount;
exports.editProfile = editProfile;