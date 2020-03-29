const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;
var commonCardNames = [
    "Mi_Go",
    "Beast Patient",
    "Mi_Go Zombie",
    "Nightmare Apostle",
    "Blood Vial"
]

var rareCardNames =  [
    "Mi_Go Worker",
    "Chime Maiden",
    "Brain of Mensis",
    "Snatcher",
    "Quicksilver Bullets",
    "Pungent Blood Cocktail",
    "Madman's Knowledge"
]

var legendaryCardNames = [
    "Mi_Go Queen",
    "Great One's Wisdom",
    "Blood Starved Beast",
    "Moon Presence",
    "Ludwig Holy Blade",
    "Lady Maria"
]

/**
 * Gets all the cards that a user has on their account
 * @param {object} data 
 * @param {object} sock 
 */
const getCollection = (data, sock) => {
    const id = data.id;             // User's id

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the specified id */
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                var temparr = [];
                var coll = result.collection;

                /* The collection is stored as on object with key:value pairs cardname:numincollection
                   This iterates through that, creating an array with elements in form "cardname-numincollection" 
                   That array is then converted to a string and written back */
                for (var key of Object.keys(coll)) {
                    temparr.push(`${key}-${coll[key]}`);
                }
                console.log(temparr.toString());
                sock.write(temparr.toString());
                // client.close()
                return;
            }).catch(err => {
                console.log(err);
                sock.write(err);
                // client.close()
                return;
            });
        }).catch(e => {
            console.log(e);
            sock.write(e.msg);
            return;
        });
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Adds a card to the user's collection
 * @param {object} data 
 * @param {object} sock 
 */
const addCard = (data, sock) => {
    const userid = data.id;             // user id
    const cardid = data.cardid;         // card name

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            /* Find a user with the supplied id. Because collection is stored as an object, 
               adding a card is as simple as matching the key (cardid) and increasing the corresponding 
               value by 1 */
            db.collection('users').findOne({
                _id: ObjectID(userid)
            }).then(result => {
                let temp = result.collection;
                console.log(temp);
                if(temp[cardid] == NaN){
                    temp[cardid] = 0;
                }
                temp[cardid]++;                         // iterate value in collection

                /* Set updated collection in db */
                db.collection('users').updateOne(
                    {_id: ObjectID(userid)},
                    { $set: { collection: temp } }
                ).then(result => {
                    console.log('card added successfully');
                    sock.write('card added successfully');
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
        })
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

/**
 * Remove a card from a user's collection
 * @param {object} data 
 * @param {object} sock 
 */
const removeCard = (data, sock) => {
    const userid = data.id;             // user's id
    const cardid = data.cardid;         // card's id

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            
            /* Find a user with the supplied id. Because collection is stored as an object, 
               adding a card is as simple as matching the key (cardid) and decreasing the corresponding 
               value by 1 */
            db.collection('users').findOne({
                _id: ObjectID(userid)
            }).then(result => {
                let temp = result.collection;
                console.log(temp);
                if (temp[cardid] > 0) {
                    temp[cardid]--;
                } else {
                    console.log(`tried to remove card you don't have`);
                    sock.write(`card doesn't exist in collection`);
                    return;
                }

                /* Set updated collection in db */
                db.collection('users').updateOne(
                    {_id: ObjectID(userid)},
                    { $set: { collection: temp } }
                ).then(result => {
                    console.log('card removed successfully');
                    sock.write('card removed successfully');
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
        })
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

const openPack = (data, sock) => {
    var pack = [];

    try {
        for (var i = 0; i < 4; i++) {
            pack.push(commonCardNames[Math.floor(Math.random() * commonCardNames.length)]);
        }

        var cardFive = Math.floor(Math.random() * 10) + 1;
        console.log(cardFive);
        if (cardFive >= 1 && cardFive < 4) {
            pack.push(legendaryCardNames[Math.floor(Math.random() * legendaryCardNames.length)]);
        } else if (cardFive >= 4 && cardFive < 9 ) {
            pack.push(rareCardNames[Math.floor(Math.random() * rareCardNames.length)]);
        } else {
            pack.push(commonCardNames[Math.floor(Math.random() * commonCardNames.length)]);
        }

        sock.write(pack.toString());
        console.log(pack.toString());
    } catch (err) {
        sock.write(err.toString());
        console.log(err);
    }
}

const getCredits = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            
            db.collection('users').findOne(
                { _id: ObjectID(id) }
            ).then(result => {
                if (result == null) {
                    throw new Error("no user found");
                }

                sock.write(result.credits.toString());
                console.log("credits returned");
            }).catch(err => {
                sock.write(err.toString());
                console.log(err);
            });
        })
    } catch (err) {
        console.log(err);
        sock.write(err.toString());
    }
}

const updateCredits = (data, sock) => {
    const id = data.id;
    const value = data.value;

    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');

            db.collection('users').updateOne(
                { _id: ObjectID(id) },
                { $inc: { credits: value } }
            ).then(result => {
                if (result.modifiedCount != 1) {
                    throw new Error("no modifications");
                }
                sock.write("credits updated");
                console.log('credits updated');
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


exports.removeCard = removeCard;
exports.getCollection = getCollection;
exports.addCard = addCard;
exports.openPack = openPack;
exports.getCredits = getCredits;
exports.updateCredits = updateCredits;
