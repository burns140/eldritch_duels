const jwt = require('jsonwebtoken')
const config = require('./dbconfig.json')

const verify = (token, sock) => {
    try {
        const decoded = jwt.verify(token, config.jwt_key);
        token = decoded.data
        next();
    } catch(err) {
        sock.write('Invalid token');
    } 
}

exports.verify = verify;