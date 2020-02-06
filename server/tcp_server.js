const net = require('net');

const host = 'localhost';
const port = 8000;

var server = net.createServer(onClientConnected);

server.listen(port, host, () => {
    console.log(`server listening on ${server.address().address}, port ${server.address().port}`);
});

function onClientConnected(sock) {
    let remoteAddress = `${sock.remoteAddress}:${sock.remotePort}`;
    console.log(`new client connectioned: ${remoteAddress}`);
    sock.write('fuck you\r\n');

    sock.on('data', (data) => {
        console.log(data.toString('utf-8'));
        sock.write(`\r\nchar received\r\n`);
    });
}
