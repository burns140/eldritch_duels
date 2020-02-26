const express = require("express");
const _ = require("lodash");

let server = express();

server.get('/', (req, res, next) => {
	res.redirect('/files/client.html');
	next();
});

server.use('/files/', express.static(__dirname));
server.use(express.json());

// this is the code to pay attention to
let connections = {};
let data = [];

function addWating(id, res) {
    console.log(`${id} is being matched...`);
    data.push(id);
    connections[id] = res;

    // if the connections is closed too early,
    // remove the connection from the list
    res.addListener("close", () => {
        delete connections[id];
        console.log(id + " disconnected");
    });
}

function getWaiting() {
    while (data.length) {
        let id = data.pop();
        if (connections[id])
            return id;
    }

    return undefined;
}

// Notice that the "next" function is never used.
server.get('/match/:id', (req, res, next) => {
	if (!req.params.id) {
		res.status(400).send("No id");
		return;
	}
		
	let id = req.params.id;
	
	if (connections[id]) {
		res.status(400).send("Already looking for a match");
		return;
    }

    let id2 = getWaiting();
    if (_.isUndefined(id2))
        addWating(id, res);
    else {
        res.send(id2);
        connections[id2].send(id);
        delete connections[id2];
        console.log(`${id} is matched with ${id2}`);
    }
});

const port = 80;
server.listen(port, () => {
	console.log("HTTP on", port);
});