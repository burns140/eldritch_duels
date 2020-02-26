const MongoClient = require('mongodb').MongoClient;
const ObjectID = require('mongodb').ObjectID;
const assert = require('assert');
const dbconfig = require('../dbconfig.json');
const verify = require('../verifyjwt');

const getCollection = (data, sock) => {
    const id = data.id;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            
            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                var temparr = [];
                var coll = result.collection;
                for (var key of Object.keys(coll)) {
                    temparr.push(`${key}-${coll[key]}`);
                }
                console.log(temparr.toString());
                sock.write(temparr.toString());
                //sock.write(JSON.stringify(result.collection));
                client.close();
                return;
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
                return;
            });
        });
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

/*const getCollection = (data, sock) => {
    const id = data.id;
    console.log('called');

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                _id: ObjectID(id)
            }).then(result => {
                sock.write(result.collection.toString());
                client.close();
                return;
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
                return;
            })
        });
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
} */

const addCard = (data, sock) => {
    const userid = data.id;
    const cardid = data.cardid;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                _id: ObjectID(userid)
            }).then(result => {
                let temp = result.collection;
                console.log(temp);
                temp[cardid]++;
                db.collection('users').updateOne(
                    {_id: ObjectID(userid)},
                    { $set: { collection: temp } }
                ).then(result => {
                    console.log('card added successfully');
                    sock.write('card added successfully');
                    client.close();
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    client.close();
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
                return;
            });
        })
    } catch(err) {
        console.log(err);
        sock.write(err);
    }
}

const removeCard = (data, sock) => {
    const userid = data.id;
    const cardid = data.cardid;

    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
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
                    client.close();
                    return;
                }
                db.collection('users').updateOne(
                    {_id: ObjectID(userid)},
                    { $set: { collection: temp } }
                ).then(result => {
                    console.log('card removed successfully');
                    sock.write('card removed successfully');
                    client.close();
                    return;
                }).catch(err => {
                    console.log(err);
                    sock.write(err);
                    client.close();
                    return;
                });
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
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