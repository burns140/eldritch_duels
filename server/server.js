const express = require('express');
const app = express();
const http = require('http').createServer(app);
const signup = require('./api/signup.js');
const login = require('./api/login.js')

const port = process.env.PORT || 8000;

app.use('/signup', signup);
app.use('/login', login);

http.listen(port, () => {
    console.log(`Server running on port ${port}`);
});