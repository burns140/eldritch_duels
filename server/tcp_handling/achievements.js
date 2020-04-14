const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;
const dbconfig = require('../dbconfig.json');

const getAchievements = (data, sock) => {
    const email = data.email;
    const type = data.type;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').findOne(
                { email: email }
            ).then(result => {
                if (result == null) {
                    throw new Error("no user with that email found");
                }

                var achievements = result.achievements;
                if (!type == "objects") {
                    sock.write(achievements.toString());
                    return;
                } else {
                    db.collection('achievements').find(
                        { _id: { $in: achievements } }
                    ).toArray().then(result => {
                        var arrBack = [];
                        for (el of result) {
                            arrBack.push(`name-${el.name};desc-${el.description}`);
                        }
                        sock.write(arrBack.join('_'));
                    }).catch(err => {
                        console.log(err);
                        sock.write(err.toString());
                    });
                }
                

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

exports.getAchievements = getAchievements;