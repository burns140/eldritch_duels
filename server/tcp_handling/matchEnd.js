const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;


const winAch = [5, 10, 25, 50, 100, 250];
const lossAch = [5, 10, 25, 50, 100, 250];
const cardAch = [50, 500, 1000, 1500];
const winDailies = [2, 3];
const cardDailies = [20, 40];
const playDailies = [5];
const winWeeklies = [10, 15];
const cardWeeklies = [200, 400];
const playWeeklies = [20, 30];

var dailyChallenge = 0;
var weeklyChallenge = 0;

const addWin = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the given id and set the three given values */
            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                {
                    $inc: { wins: 1, winsToday: 1, winsThisWeek: 1, totalGames: 1, gamesToday: 1, gamesThisWeek: 1 }
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
                    $inc: { losses: 1, lossesToday: 1, lossesThisWeek: 1, totalGames: 1, gamesToday: 1, gamesThisWeek: 1 }
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

const addXPUnit = (data, sock) => {
    const id = data.id;
    const amount = data.amount;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the given id and set the three given values */
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {            // No document was modified, so error
                    console.log('modified not 1');
                    sock.write(`Failed to update profile correctly`);
                    return;
                }
                
                var xp = result.xp + amount;
                var levelUp = 0;
                if (xp >= 500) {
                    levelUp = 1;
                    xp = xp % 500;
                }

                db.collection('users').updateOne(
                    { _id: ObjectID(id) },
                    { 
                        $set: { xp: xp },
                        $inc: { level: levelUp } 
                    }
                ).then(result => {
                    if (result.modifiedCount != 1) {
                        throw new Error("xp not added");
                    }
                    console.log('xp updated');
                    sock.write('xp updated');
                }).catch(err => {
                    console.log(err);
                    sock.write(err.toString());
                })
            }).catch(err => {
                console.log(err);
                sock.write(err.toString());
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
    const cardsPlayed = data.cardsPlayed;
    const xpGain = data.xpGain;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the given id and set the three given values */
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                   throw new Error('no user with that id');
                }
                var checkChallengeObj = result;
                var achievements = result.achievements;

                if (winAch.includes(result.wins)) {
                    if (!achievements.includes(winAch.indexOf(result.wins))) {
                        achievements.push(winAch.indexOf(result.wins));
                    }
                }

                if (lossAch.includes(result.losses)) {
                    if (!achievements.includes(lossAch.indexOf(result.losses) + winAch.length)) {
                        achievements.push(lossAch.indexOf(result.losses) + winAch.length);
                    }
                }

                checkChallengeObj.cardsPlayedTotal = result.cardsPlayedTotal + cardsPlayed;
                checkChallengeObj.cardsPlayedToday = result.cardsPlayedToday + cardsPlayed;
                checkChallengeObj.cardsPlayedThisWeek = result.cardsPlayedThisWeek + cardsPlayed;

                for (var j = 0; j < cardAch.length; j++) {
                    if (cardsTotal > cardAch[j] && !achievements.includes(j + winAch.length + lossAch.len)) {
                        achievements.push(j + winAch.length + lossAch.length);
                        break;
                    }
                }

                var dailyCompleted = result.dailyChallenge;
                if (dailyCompleted == 0) {
                    dailyCompleted = checkDailyChallenge(checkChallengeObj);
                }

                var weeklyCompleted = result.weeklyChallenge;
                if (weeklyCompleted == 0) {
                    weeklyCompleted = checkWeeklyChallenge(checkChallengeObj);
                }

                var xp = result.xp + xpGain;
                var levelUp = 0;
                if (xp >= 500) {
                    levelUp = 1;
                    xp = xp % 500;
                }

                db.collection('users').updateOne(
                    { _id: ObjectID(id) },
                    { 
                        $set: { achievements: achievements, xp: xp, dailyChallenge: dailyCompleted, weeklyChallenge: weeklyCompleted, cardsPlayedTotal: cardsTotal, cardsPlayedToday: cardsToday, cardsPlayedThisWeek: cardsWeek },
                        $inc: { level: levelUp }
                    }
                ).then(result => {
                    if (result.matchedCount != 1) {
                        throw new Error("no user with that id");
                    }
                }).catch(err => {
                    console.log(err);
                    sock.write(err.toString());
                });
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

function checkDailyChallenge(result) {
    switch (dailyChallenge) {
        case 0:
            if (result.winsToday == winDailies[0]) {
                return 1;
            }
            break;
        case 1:
            if (result.winsToday == winDailies[1]) {
                return 1;
            }
            break;
        case 2:
            if (result.cardsPlayedToday >= cardDailies[0]) {
                return 1;
            }
            break;
        case 3:
            if (result.cardsPlayedToday >= cardDailies[1]) {
                return 1;
            }
            break;
        case 4:
            if (result.gamesToday >= playDailies[0]) {
                return 1;
            }
            break;
        default:
            return 0;
    }

    return 0;
}

function checkWeeklyChallenge(result) {
    switch (weeklyChallenge) {
        case 0:
            if (result.winsThisWeek >= winWeeklies[0]) {
                return 1;
            }
            break;
        case 1:
            if (result.winsThisWeek >= winWeeklies[1]) {
                return 1;
            }
            break;
        case 2:
            if (result.cardsPlayedThisWeek >= cardWeeklies[0]) {
                return 1;
            }
            break;
        case 3:
            if (result.cardsPlayedThisWeek >= cardWeeklies[1]) {
                return 1;
            }
            break;
        case 4:
            if (result.gamesThisWeek >= playWeeklies[0]) {
                return 1;
            }
            break;
        case 5:
            if (result.gamesThisWeek >= playWeeklies[1]) {
                return 1;
            }
            break;
        default:
            return 0;
    }

    return 0;
}

exports.addLoss = addLoss;
exports.addWin = addWin;
exports.resolveAchievements = resolveAchievements;
exports.addXPUnit = addXPUnit;