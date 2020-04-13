const MongoClient = require('../mongo_connection');


const fetchLeaderboardData = (data, sock) => {
    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').find(
                {}
            ).toArray().then(result => {
                if (result.length == 0) {
                    throw new Error('no users matched');
                }

                var temp = result;
                temp.sort((i1, i2) => i2.elo - i1.elo);

                var toReturn = [];

                for (var i = 0; i < 100; i++) {
                    if (i == temp.length) {
                        break;
                    }
                    toReturn.push(`${temp.username}_${temp.elo}`);
                }

                console.log('returning elo data');
                sock.write(toReturn.toString());
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

exports.fetchLeaderboardData = fetchLeaderboardData;