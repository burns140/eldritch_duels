const MongoClient = require('mongodb').MongoClient;
const dbconfig = require('../dbconfig.json');
const assert = require('assert');
const bcrypt = require('bcrypt');
const net = require('net');
const signup = require('./tcp_handling/signup.js');

const host = 'localhost';
const port = 8000;

var server = net.createServer(onClientConnected);

server.listen(port, host, () => {
    console.log(`server listening on ${server.address().address}, port ${server.address().port}`);
});

function onClientConnected(sock) {
    let remoteAddress = `${sock.remoteAddress}:${sock.remotePort}`;
    console.log(`new client connectioned: ${remoteAddress}`);

    sock.on('data', (data) => {
        console.log(data.toString('utf-8'));
    });

    sock.on('error', (err) => {
        console.log(err);
    });

    sock.on('close', () => {
        console.log('connection closed');
    })
}
