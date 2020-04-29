const MongoClient = require('../mongo_connection');
const ObjectId = require('mongodb').ObjectId;

/**
 * 
 * @param {any} data
 * @param {import('net').Socket} sock
 */
async function incrementStat(data, sock) {
    try {
        const client = await MongoClient.get();
        const collection = client.db('eldritch_data').collection('users');

        let updateOperator = {};
        updateOperator[data.type] = 1;

        const status = await collection.updateOne(
            { _id: ObjectId(data.id) },
            { $inc: updateOperator }
        );

        sock.write(status.result.ok ? "OK" : "ERROR");
    } catch (e) {
        console.log(e);
        sock.write(e.message);
    }
}


function getRatio(data, type) {
    let wins = data[type + 'Wins'];
    let losses = data[type + 'Losses'];

    wins = wins || 0;
    losses = losses || 0;

    return wins + ':' + losses;
}

/**
 * 
 * @param {any} data
 * @param {import('net').Socket} sock
 */
async function getStats(data, sock) {
    try {
        const client = await MongoClient.get();
        const collection = client.db('eldritch_data').collection('users');

        const user = await collection.findOne({ _id: ObjectId(data.id) });

        const response = `Elo=${user.elo}\nAI=${getRatio(user, "ai")}\nCompetetive=${getRatio(user, "competetive")}\nCasual=${getRatio(user, "casual")}`;
        sock.write(response);
    } catch (e) {
        sock.write(e.message);
        console.log(e);
    }
}

module.exports.incrementStat = incrementStat;
module.exports.getStats = getStats;