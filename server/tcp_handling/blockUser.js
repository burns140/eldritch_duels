const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;

/**
 * Blocks a user. The blocked user is added to blocking array and the blocking user
 * is added to the blocked users "blockedBy" array. This is helpful for tracking when going
 * to view other's profiles.
 * @param {object} data 
 * @param {object} sock 
 */
const blockUser = (data, sock) => {
    const myEmail = data.myEmail;           // My email
    const userToBlock = data.toBlockEmail;  // Email of the user I'm blocking

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Add the other user to my blocked array
               Use addToSet so that we can only update one time */
            db.collection('users').updateOne(
                { email: myEmail },
                { $addToSet: { blocked: userToBlock } }
            ).then(result => {
                if (result.matchedCount != 1) {
                    console.log('failed to block user');
                    sock.write('failed to block user');
                } else {
                    console.log('user blocked');

                    /* Add me to the other user's blockedBy array */
                    db.collection('users').updateOne(
                        { email: userToBlock },
                        { $addToSet: { blockedBy: myEmail } }
                    ).then(result => {
                        if (result.matchedCount != 1) {
                            console.log('failed to update blockedby');
                            sock.write('failed to block user');
                        } else {
                            console.log('user blocked');
                            sock.write('user blocked successfully');
                        }
                        return;
                    }).catch(err => {
                        console.log(err);
                        sock.write(err);
                        return;
                    });
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
        sock.write(err);
    }
}

/**
 * Unblocks a user. The unblocked user is removed from my "blocked" array and the I 
 * am removed from that user's blockedBy array.
 * @param {object} data 
 * @param {object} sock 
 */
const unblockUser = (data, sock) => {
    const myEmail = data.myEmail;           // My email
    const userToUnblock = data.toBlockEmail;  // Email of the user I'm blocking

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Remove the other user from my blocked array */
            db.collection('users').updateOne(
                { email: myEmail },
                { $pull: { blocked: userToUnblock } }
            ).then(result => {
                if (result.matchedCount != 1) {
                    console.log('failed to unblock user');
                    sock.write('failed to unblock user');
                } else {
                    console.log('user unblocked');

                    /* Add me to the other user's blockedBy array */
                    db.collection('users').updateOne(
                        { email: userToUnblock },
                        { $pull: { blockedBy: myEmail } }
                    ).then(result => {
                        if (result.matchedCount != 1) {
                            console.log('failed to update blockedby');
                            sock.write('failed to unblock user');
                        } else {
                            console.log('user unblocked');
                            sock.write('user unblocked successfully');
                        }
                        return;
                    }).catch(err => {
                        console.log(err);
                        sock.write(err);
                        return;
                    });
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
        sock.write(err);
    }
}

/**
 * Get a list of the users that user with id "id" has blocked
 * @param {object} data 
 * @param {object} sock 
 */
const getBlockedUsers = (data, sock) => {
    id = data.id;   // My id

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            
            /* Get the blocked array for this user */
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                    console.log('user not found');
                    sock.write('user not found');
                    return;
                }

                console.log(result);

                console.log('returning emails of blocked users');
                if (result.blocked.toString() == "") {
                    sock.write("noblockedusers");
                } else {
                    sock.write(result.blocked.toString());
                }
                return;
            }).catch(err => {
                console.log(err);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Get a list of the users that user with id "id" has been blocked by
 * @param {object} data 
 * @param {object} sock 
 */
const getBlockedByUsers = (data, sock) => {
    id = data.id;   // My id

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            
            /* Get the blockedBy array for this user */
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                    console.log('user not found');
                    sock.write('user not found');
                    return;
                }

                console.log(result);
                console.log('returning emails of blockedby users');

                if (result.blockedBy.toString() == "") {
                    sock.write("notblockedbyanyone");
                } else {
                    sock.write(result.blockedBy.toString());
                }
                return;
            }).catch(err => {
                console.log(err);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

exports.getBlockedUsers = getBlockedUsers;
exports.getBlockedByUsers = getBlockedByUsers;
exports.unblockUser = unblockUser;
exports.blockUser = blockUser;