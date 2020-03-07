const jwt = require('jsonwebtoken')
const dbconfig = require('./dbconfig.json')

const verify = (token, sock) => {
    try {
        const decoded = jwt.verify(token, dbconfig.jwt_key);        // Get data from the token
        token = decoded.data
        console.log(token);
        return true;
    } catch(err) {                  // Returns false if the token is invalid because invalid causes error
        console.log(err);
        sock.write('Invalid token');
        return false;
    } 
}

exports.verify = verify;