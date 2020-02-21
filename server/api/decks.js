const express = require('express');
const MongoClient = require('mongodb').MongoClient;
const router = express.Router();
const assert = require('assert');
const dbconfig = require('../db_config.json');
const verify = require('../verifyjwt');

router.use(express.urlencoded({extended: false}));
router.use(verify);
router.get('/', (req, res) => {
    
});