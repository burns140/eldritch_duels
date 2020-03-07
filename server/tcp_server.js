const assert = require('assert');
const bcrypt = require('bcrypt');
const net = require('net');
const MongoClient = require('./mongo_connection');
const onClientConnected = require('./tcp_handling/data_handler');

/* Create server */
const host = 'localhost';
const port = process.env.port || 8000;
var server = net.createServer(onClientConnected);

/* Start server */
console.log("establishing mongo client");
MongoClient.get().then((client) => {
    console.log("mongo client established");
    
    server.listen(port, host, () => {
        console.log(`server listening on ${server.address().address}, port ${server.address().port}`);
    });
}).catch(e => {
    console.log("Mongo db error");
    console.log(e);
});
