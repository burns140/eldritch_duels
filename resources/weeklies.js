const assert = require('assert');
const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../server/dbconfig.json');


function Weekly(_id, name, description) {
    this._id = _id;
    this.name = name;
    this.description = description;
}

var names = [
    "10 wins",
    "15 wins",
    "200 cards",
    "400 cards",
    "20 games",
    "30 games"
]

var descriptions = [
    "Win 10 games",
    "Win 15 games",
    "Play 200 cards",
    "Play 400 cards",
    "Play 20 games",
    "Play 30 games"
]

function Main() {
    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            var j = 0;
            
            const db = client.db('eldritch_data');
            db.collection('weeklies').deleteMany(
                {}
            ).then(result => {
                for (var i = 0; i < descriptions.length; i++) {
                    var temp = new Weekly(i, names[i], descriptions[i]);
                    db.collection('weeklies').insertOne(
                        temp
                    ).then(result => {
                        j++;
                        console.log('inserted');
                        if (j == descriptions.length) {
                            console.log('closing');
                            client.close();
                        }
                    }).catch(err => {
                        console.log(err);
                        client.close();
                    });
                }
            }).catch(err => {
                console.log(err);
                client.close();
            });
        });
    } catch (err) {
        console.log(err);
    }
}

Main();