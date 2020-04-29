const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;


const getAchievements = (data, sock) => {
    const id = data.id;
    const type = data.type;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                    throw new Error("no user with that email found");
                }

                var achievements = result.achievements;
                if (achievements.length == 0) {
                    sock.write("no achievements earned");
                    return;
                }
                achievements.sort((a, b) => {
                    return a - b;
                });
                console.log(achievements);
                if (type != "objects") {
                    if (achievements.length == 0) {
                        sock.write('no achievements');
                    } else {
                        sock.write(achievements.toString());
                    }
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

const getOneAchievement = (data, sock) => {
    const id = parseInt(data.id);

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('achievements').findOne(
                { _id: id }
            ).then(result => {
                if (result == null) {
                    throw new Error('no achievement with that id exists');
                }

                sock.write(`name-${result.name};desc-${result.description}`);
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

const getAllAchievements = (data, sock) => {
    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('achievements').find(
                {}
            ).toArray().then(result => {
                if (result.length == 0) {
                    throw new Error('no achievements found');
                }

                var arrBack = [];
                for (el of result) {
                    arrBack.push(`name-${el.name};desc-${el.description}`);
                }
                sock.write(arrBack.join('_'));
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

exports.getOneAchievement = getOneAchievement;
exports.getAllAchievements = getAllAchievements;
exports.getAchievements = getAchievements;