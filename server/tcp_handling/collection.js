const MongoClient = require('../mongo_connection');
const ObjectID = require('mongodb').ObjectID;

/**
 * Gets all the cards that a user has on their account
 * @param {object} data 
 * @param {object} sock 
 */
const getCollection = (data, sock) => {
    const id = data.id;             // User's id

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
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
                return;
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        }).catch(e => {
            console.log(e);
            sock.write(e.msg);
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

exports.removeCard = removeCard;
exports.getCollection = getCollection;
exports.addCard = addCard;
