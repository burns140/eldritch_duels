const express = require('express');
const app = express();
const server = require('http').createServer(app);
const signup = require('./signup.js');
const login = require('./login.js');
const decks = require('./decks.js');
const collection = require('./collection.js');

const port = process.env.PORT || 8000;

app.use('/signup', signup);
app.use('/login', login);
app.use('/decks', decks);
app.use('/collection', collection);

server.listen(port, () => {
    console.log(`Server running on port ${port}`);
});