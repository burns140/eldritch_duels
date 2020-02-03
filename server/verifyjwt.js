const jwt = require('jsonwebtoken')
const config = require('./db_config.json')

module.exports = (req,res,next) => {
    //do stuff
    if(!req.header('Authorization')) {
        res.status(401).json({err:'Not logged in'})
        return;
    }
    const token = req.header('Authorization').substr(req.header('Authorization').indexOf(' ') + 1)
    try {
        const decoded = jwt.verify(token, config.jwt_key)
        req.token = decoded.data
        next()
    } catch(err) {
        res.status(401).json({err:'Invalid token'})
    }
}