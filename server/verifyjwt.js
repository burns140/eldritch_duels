const jwt = require('jsonwebtoken')
const dbconfig = require('./dbconfig.json')

const verify = (token, sock) => {
    try {
        const decoded = jwt.verify(token, dbconfig.jwt_key);
        token = decoded.data
        console.log(token);
        return true;
    } catch(err) {
        console.log(err);
        sock.write('Invalid token');
        return false;
    } 
}

exports.verify = verify;