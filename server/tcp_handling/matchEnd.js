const MongoClient = require('../mongo_connection');
const dbconfig = require('../dbconfig.json');

const winAch = [5, 10, 25, 50, 100, 250];
const lossAch = [5, 10, 25, 50, 100, 250];

const addWin = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the given id and set the three given values */
            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                {
                    $inc: { wins: 1, totalGames: 1 }
                }
            ).then(result => {
                if (result.matchedCount != 1) {            // No document was modified, so error
                    console.log('modified not 1');
                    sock.write(`Failed to update wins`);
                } else {
                    console.log('successfully updated');    // Success
                    sock.write('failed to update wins');
                }
                return;
            }).catch(err => {
                console.log(err);
                sock.write(`Failed to update wins`);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

const addLoss = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the given id and set the three given values */
            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                {
                    $inc: { losses: 1, totalGames: 1 }
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
        sock.write(err.toString());
    }
}

const resolveAchievements = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the given id and set the three given values */
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                var achievements = result.achievements;
                var changed = false;
                if (result == null) {
                   throw new Error('no user with that id');
                }
                if (winAch.includes(result.wins)) {
                    if (!achievements.includes(winAch.indexOf(result.wins))) {
                        achievements.push(winAch.indexOf(result.wins));
                        changed = true;
                    }
                }
                if (lossAch.includes(result.losses)) {
                    if (!achievements.includes(lossAch.indexOf(result.losses) + winAch.length)) {
                        achievements.push(lossAch.indexOf(result.losses) + winAch.length);
                        changed = true;
                    }
                }
                if (changed) {
                    db.collection('users').updateOne(
                        { _id: ObjectID(id) },
                        { $set: {achievements: achievements } }
                    ).then(result => {
                        if (result.matchedCount != 1) {
                            throw new Error("no user with that id");
                        }
                    }).catch(err => {
                        console.log(err);
                        sock.write(err.toString());
                    });
                }
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

exports.addLoss = addLoss;
exports.addWin = addWin;
exports.resolveAchievements = resolveAchievements;