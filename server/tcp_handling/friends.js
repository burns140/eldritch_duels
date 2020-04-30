const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;
const tcp = require('../tcp_server.js');

/**
 * Send a friend request to a user. Add emails to users' sentFriendRequests and friendRequests arrays
 * to track it.
 * @param {object} data 
 * @param {object} sock 
 */
const sendFriendRequest = (data, sock) => {
    var myEmail = data.myEmail;
    var theirEmail = data.theirEmail;
    var errString = 'failed to send friend request';

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Check if I already have a friend request from them*/
            db.collection('users').findOne(
                { email: myEmail }
            ).then(result => {
                if (result == null) {
                    throw new Error('user not found');
                }
                if (!result.friendRequests.includes(theirEmail)) {

                    /* Add my email to their friendRequests array */
                    db.collection('users').updateOne(
                        { email: theirEmail },
                        { $addToSet: { friendRequests: myEmail } }
                    ).then(result => {
                        if (result.matchedCount != 1) {
                            throw new Error(errString);
                        } else {
                            
                            /* Add their email to my friendRequestsSent array */
                            db.collection('users').updateOne(
                                { email: myEmail },
                                { $addToSet: { friendRequestsSent: theirEmail } }
                            ).then(result => {
                                if (result.matchedCount != 1) {
                                    throw new Error(errString);
                                } else {
                                    console.log('friend request sent');
                                    sock.write('friend request sent');
                                }
                                return;
                            }).catch(err => {
                                console.log(err);
                                sock.write(err);
                                return;
                            });  
                        }
                    }).catch(err => {
                        console.log(err);
                        sock.write(err);
                        return;
                    });
                } else {
                    sock.write('this user has already sent you a friend request');
                    console.log('this user has already sent you a friend request');
                }
            })

            
        });
    } catch(err) {
        sock.write(err);
        console.log(err);
        return;
    }
}

/**
 * Accept a friend request by altering friendRequestsSent, friendRequests, friends arrays
 * @param {object} data 
 * @param {object} sock 
 */
const acceptFriendRequest = (data, sock) => {
    const theirEmail = data.theirEmail;
    const myEmail = data.myEmail;
    const errString = 'failed to accept friend request';

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
    
            /* Remove my email from their friendRequestsSend and add my email to their friends list */
            db.collection('users').updateOne(
                { email: theirEmail },
                { 
                    $pull: { friendRequestsSent: myEmail }, 
                    $addToSet: { friends: myEmail }
                }
            ).then(result => {
                if (result.matchedCount != 1) {
                    throw new Error(errString);
                }
    
                /* Remove their email from my friendRequests and add their email to my friends list */
                db.collection('users').updateOne(
                    { email: myEmail },
                    { 
                        $pull: { friendRequests: theirEmail },
                        $addToSet: { friends: theirEmail } 
                    }
                ).then(result => {
                    if (result.matchedCount != 1) {
                        throw new Error(errString);
                    }
                    console.log('friend request accepted');
                    sock.write('friend request accepted');
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    return;
                });
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
 * Reject friend request by removing emails from friendRequests and friendRequestsSent arrays
 * @param {object} data 
 * @param {object} sock 
 */
const rejectFriendRequest = (data, sock) => {
    const myEmail = data.myEmail;
    const theirEmail = data.theirEmail;
    const errString = 'failed to reject friend request';

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Remove their email from my friendRequests array */
            db.collection('users').updateOne(
                { email: myEmail },
                { $pull: { friendRequests: theirEmail } }
            ).then(result => {
                if (result.matchedCount != 1) {
                    throw new Error(errString);
                }

                /* Remove my email from their friendRequestsSent array */
                db.collection('users').updateOne(
                    { email: theirEmail },
                    { $pull: { friendRequestsSent: myEmail } }
                ).then(result => {
                    if (result.matchedCount != 1) {
                        throw new Error(errString);
                    }
                    console.log('friend request rejected');
                    sock.write('friend request rejected');
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        })
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Remove friends from friend lists by removing emails from friends array
 * @param {object} data 
 * @param {object} sock 
 */
const removeFriend = (data, sock) => {
    const myEmail = data.myEmail;
    const theirEmail = data.theirEmail;
    const errString = 'failed to remove friend';

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
    
            /* Remove their email from my friends array */
            db.collection('users').updateOne(
                { email: myEmail },
                { $pull: { friends: theirEmail } }
            ).then(result => {
                if (result.matchedCount != 1) {
                    throw new Error(errString);
                }

                /* Remove my email from their friends array */
                db.collection('users').updateOne(
                    { email: theirEmail },
                    { $pull: { friends: myEmail } }
                ).then(result => {
                    if (result.matchedCount != 1) {
                        throw new Error(errString);
                    }
                    console.log('friend removed');
                    sock.write('friend removed')
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        })
    } catch (err) {
        console.log(err);
        sock.write(err);
        return;
    }
    
}

/**
 * Get a user's friends list
 * @param {object} data 
 * @param {object} sock 
 */
const getAllFriends = (data, sock) => {
    const id = data.id;
    const errString = 'could not get friends list'

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Get the user with my id */
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                    throw new Error(errString);
                }

                if (result.friends.toString() == "") {
                    sock.write("nofriends");
                } else {
                    sock.write(result.friends.toString());
                }
                console.log('friends list returned');
            }).catch(err => {
                console.log(err);
                sock.write(err);
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
    
}

/* Return a string comma separated that has all usernames and emails that exist */
const getAllUsernames = (data, sock) => {
    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            
            /* Return all users by passing in empty document */
            db.collection('users').find(
                {}
            ).toArray().then(result => {
                if (result.length == 0) {
                    throw new Error("returned no users");
                }
                var usernames = [];

                for (user of result) {
                    usernames.push(`${user.username}-${user.email}`);
                }

                
                sock.write(usernames.toString());
                console.log('list of users returned');
            }).catch(err => {
                console.log(err);
                sock.write(err.toString());
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err.toString());
    }
}

const getFriendRequests = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
    
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                    throw new Error("user not found");
                }
    
                if (result.friendRequests.toString() == "") {
                    sock.write("norequests");
                } else {
                    sock.write(result.friendRequests.toString());
                }
                console.log('getting friend requests');
            }).catch(err => {
                console.log(err);
                sock.write(err.toString());
            });
        })
    } catch (err) {
        console.log(err);
        sock.write(err.toString());
    }
    
}

const getFriendRequestsSent = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
    
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                    throw new Error("user not found");
                }
    
                if (result.friendRequestsSent.toString() == "") {
                    sock.write("nofriendrequestssent");
                } else {
                    sock.write(result.friendRequestsSent.toString());
                }
                console.log('get sent friend requests');
            }).catch(err => {
                console.log(err);
                sock.write(err.toString());
            });
        })
    } catch (err) {
        console.log(err);
        sock.write(err.toString());
    }
}

const sendMessage = (data, sock) => {
    const myEmail = data.email;
    const theirEmail = data.friend;
    const message = data.message;
    const errString = 'failed to send message';
    const id = data.id;

    try {
        var players = tcp.getPlayList();

        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            db.collection('users').findOne(
                { email: theirEmail }
            ).then(result => {
                if (result == null) {
                    throw new Error('user not found');
                }
                console.log(tcp.getPlayList());
                if (players.isLoggedIn(result._id.toString()))
                {
                    console.log("sending message to id " + result._id.toString() + " from id " + _id.toString());
                    players.getSocketByKey(result._id.toString()).write('message:' + myEmail + "|" + message);
                    sock.write("message sent");
                }
                else
                {
                    console.log("player not logged in");
                    sock.write("notloggedin");
                }
            })

            
        });

    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

exports.getAllUsernames = getAllUsernames;
exports.sendFriendRequest = sendFriendRequest;
exports.acceptFriendRequest = acceptFriendRequest;
exports.rejectFriendRequest = rejectFriendRequest;
exports.removeFriend = removeFriend;
exports.getAllFriends = getAllFriends;
exports.getFriendRequests = getFriendRequests;
exports.getFriendRequestsSent = getFriendRequestsSent;
exports.sendMessage = sendMessage;