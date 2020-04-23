const assert = require('assert');
const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../server/dbconfig.json');


function Achievement(_id, name, description) {
    this._id = _id;
    this.name = name;
    this.description = description;
}

var names = [
    "5 wins",
    "10 wins",
    "25 wins",
    "50 wins",
    "100 wins",
    "250 wins",
    "5 losses",
    "10 losses",
    "25 losses",
    "50 losses",
    "100 losses",
    "250 losses",
    "50 cards",
    "500 cards",
    "1000 cards",
    "1500 cards",
]

var descriptions = [
    "Win 5 games",
    "Win 10 games",
    "Win 25 games",
    "Win 50 games",
    "Win 100 games",
    "Win 250 games",
    "Lose 5 games",
    "Lose 10 games",
    "Lose 25 games",
    "Lose 50 games",
    "Lose 100 games",
    "Lose 250 games",
    "Play 50 cards",
    "Play 500 cards",
    "Play 1000 cards",
    "Play 1500 cards"
]

function Main() {
    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            var j = 0;
            
            const db = client.db('eldritch_data');
            db.collection('achievements').deleteMany(
                {}
            ).then(result => {
                for (var i = 0; i < descriptions.length; i++) {
                    var temp = new Achievement(i, names[i], descriptions[i]);
                    db.collection('achievements').insertOne(
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