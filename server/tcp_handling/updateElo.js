const MongoClient = require('../mongo_connection');
const ObjectId = require('mongodb').ObjectId;
const Elo = require('../classes/Elo');

/**
 * 
 * @param {} data
 * @param {import('net').Socket} sock
 */
async function updateElo(data, sock) {
    try {
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

        if (!res.result.ok) {
            console.log("Elo update failure:");
            console.log(data);
        }

        sock.write(`${user.elo} to ${newElo}`);
    } catch (e) {
        console.log(e);
        sock.write(e);
    }
}

module.exports = updateElo;