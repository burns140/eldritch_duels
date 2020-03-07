const assert = require('assert');
const MongoClient = require('mongodb').MongoClient;

const dbconfig = require('./dbconfig.json');

// create one mongo client because creating one every time it is needed is slow
//  see https://stackoverflow.com/questions/10656574/how-do-i-manage-mongodb-connections-in-a-node-js-web-application#answer-14464750
let mongoClient = null;
function get() {
    return new Promise(res => {
        if (mongoClient != null) {
            res(mongoClient);
            return;
        }

        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);
            mongoClient = client;
            res(client);
        });
    });
}

module.exports.get = get;