const MongoClient = require('../mongo_connection');
const ObjectId = require('mongodb').ObjectId;
const Elo = require('../classes/Elo');

/**
 * 
 * @param {} data
 * @param {import('net').Socket} sock
 */
async function updateElo(data, sock) {
    let func;
    switch (data.status) {
        case "win":
            func = Elo.win;
            break;

        case "lose":
            func = Elo.lose;
            break;

        default:
            sock.write("Bad status: " + sock.status);
            return;
    }

    const client = await MongoClient.get();
    const collection = client.db('eldritch_data').collection('users');

    const id = ObjectId(data.id);
    const user = await collection.findOne({ _id: id });

    const newElo = func(user.elo, data.enemyElo);

    const res = await collection.updateOne(
        { _id: id },
        { $set: { 'elo': newElo } }
    );

    console.log(res);
    sock.write(`${user.elo} to ${newElo}`);
}

module.exports = updateElo;