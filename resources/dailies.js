const assert = require('assert');
const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../server/dbconfig.json');


function Daily(_id, name, description) {
    this._id = _id;
    this.name = name;
    this.description = description;
}

var names = [
    "2 wins",
    "3 wins",
    "20 cards",
    "40 cards",
    "5 games"
]

var descriptions = [
    "Win 2 games",
    "Win 3 games",
    "Play 20 cards",
    "Play 40 cards",
    "Play 5 games"
]

function Main() {
    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            var j = 0;
            
            const db = client.db('eldritch_data');
            db.collection('dailies').deleteMany(
                {}
            ).then(result => {
                for (var i = 0; i < descriptions.length; i++) {
                    var temp = new Daily(i, names[i], descriptions[i]);
                    db.collection('dailies').insertOne(
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