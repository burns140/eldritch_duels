const MongoClient = require('../mongo_connection');
const dbconfig = require('../dbconfig.json');
const nodemailer = require('nodemailer');
const myEmail = 'eldritch.duels@gmail.com';             // Account I created specifically for this project
const generator = require('generate-password');
const bcrypt = require('bcrypt');
const ObjectID = require('mongodb').ObjectID;


var transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: myEmail,
        pass: dbconfig.emailpass
    }
});


/**
 * Send a temporary password to the user that they can use to login
 * @param {object} data 
 * @param {object} sock 
 */
const resetPassword = (data, sock) => {
    const toEmail = data.email;     // Email to send password reset request to
    console.log('in temp pass');
    try {
        MongoClient.get().then(client => {
            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                email: toEmail
            }).then(result => {
                /* User with  that email doesn't have account, so don't send email */
                if (!result) {
                    console.log('account with that email doesn\'t exist')
                    sock.write('Account with that email doesn\'t exist');
                    return;
                } else {
                    var tempPass = generator.generate({         // Generate temp password
                        length: 10,
                        numbers: true,
                        symbols: true
                    });

                    /* Create html for email */
                    var emailText = `<h1>Password reset request</h1>` +
                                    `<p>We have received a request for a temporary password for your account. ` +
                                    `If you requested this, please login with the following password ` +
                                    `on your next login attempt. After you login with this password, your old ` +
                                    `password will be invalid and you can change your password in your account settings. ` +
                                    `If you login with your old password, this password will become invalid.</p>` +
                                    `<p>Password: ${tempPass}</p>`
                    const hash = bcrypt.hashSync(tempPass, 10);  // Hash password

                    var passes = [];                    // New array with original password and temp password
                    passes.push(result.password[0]);
                    passes.push(hash);

                    db.collection('users').updateOne(
                        { email: toEmail },
                        { $set: { password: passes } }
                    ).then(result => {
                        if (result.modifiedCount != 0) {
                            var mailOptions = {             // Set mail options
                                from: myEmail,
                                to: toEmail,
                                subject: 'Password reset request',
                                html: emailText
                            };
                        
                            transporter.sendMail(mailOptions, (error, info) => {        // Send the email
                                if (error) {
                                    console.log(err);
                                } else {
                                    console.log('Email send: ' + info.response);
                                }
                            });
                            return;
                        } else {
                            console.log('Unable to update database');
                            sock.write('Failed to update database');
                            return;
                        }
                    }).catch(err => {
                        console.log(err);
                        sock.write(err);
                        return;
                    });
                }
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        })
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

const resendVerification = (data, sock) => {
    const toEmail = data.email;
    const id = data.id;
    const host = "localhost:7999";


    try {
        MongoClient.get().then(client => {
            assert.equal(null, err);
            const db = client.db('eldritch_data');
            
            db.collection('users').find({
                _id: ObjectID(id)
            }).limit(1).toArray().then(result => {
                if (result.length != 1) {
                    console.log("failed to find that id");
                    sock.write("failed to find that id");
                    return;
                } else {
                    var emailText = `<h1>Email Verification</h1>` +
                                    `<p>You have requested a link to verify your account. Before you can ` +
                                    `access matchmaking, you must verify your account by clicking on the link below.</p>` +
                                    `<a href="http://${host}/verify/${result[0].verifyStr}">Verify Account</a>`;

                    var mailOptions = {
                        from: myEmail,
                        to: toEmail,
                        subject: 'Verify Account',
                        html: emailText
                    };

                    transporter.sendMail(mailOptions, (error, info) => {
                        if (error) {
                            console.log(err);
                        } else {
                            console.log('Email send: ' + info.response);
                        }
                    });
                    
                    sock.write('Verification email resent');
                    console.log('Verification email resent');
                    return;
                }
            }).catch(err => {
                console.log(err);
                sock.write(err);
                return;
            });
        });
    } catch (err) {
        console.log(err);
        sock.write("failed to send verification email");
        return;
    }
}

exports.resetPassword = resetPassword;
exports.resendVerification = resendVerification;
