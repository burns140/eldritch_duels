const MongoClient = require('../mongo_connection');

/**
 * Reset all users daily stats
 * @param {Object} data 
 * @param {Socket} sock 
 */
const dailyReset = (data, sock) => {
    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').update(
                {},
                { $set: { winsToday: 0, lossesToday: 0, gamesToday: 0, cardsPlayedToday: 0 } }
            ).then(result => {
                if (result.matchedCount == 0) {
                    throw new Error('no users matched');
                }
                console.log('daily reset success');
                sock.write('daily reset completed');
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

/**
 * Reset all users weekly stats
 * @param {Object} data 
 * @param {Socket} sock 
 */
const weeklyReset = (data, sock) => {
    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').update(
                {},
                { $set: { winsThisWeek: 0, lossesThisWeek: 0, gamesThisWeek: 0, cardsPlayedThisWeek: 0 } }
            ).then(result => {
                if (result.matchedCount == 0) {
                    throw new Error('no users matched');
                }
                console.log('daily reset success');
                sock.write('daily reset completed');
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

exports.dailyReset = dailyReset;
exports.weeklyReset = weeklyReset;