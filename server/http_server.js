const express = require('express');
const app = express();
const cors = require('cors');
const http = require('http').createServer(app);
const verifyLogin = require('./api/verifyemail.js');

const port = 7999;
app.use(cors());
app.use('/verify', verifyLogin);

http.listen(port, () => {
    console.log(`http server running on port ${port}`);
});