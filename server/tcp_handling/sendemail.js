const MongoClient = require('mongodb').MongoClient;
const assert = require('assert');
const dbconfig = require('../dbconfig.json');
const verify = require('../verifyjwt');
const nodemailer = require('nodemailer');
const myEmail = 'eldritch.duels@gmail.com';
const generator = require('generate-password');
const bcrypt = require('bcrypt');


var transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: myEmail,
        pass: dbconfig.emailpass
    }
});


/* There is a security flaw here where they can send infinite password reset requests.
   Because we are sending temporary passwords, they could hypothetically have infinite passwords
   linked to their account until the attacker guesses correctly, which would then remove all other passwords.
   Could be fixed by removing one temporary password and replacing it with another, but right now that
   is at the bottom of the priority list. */
const resetPassword = (data, sock) => {
    const toEmail = data.email;
    //const toEmail = 'aphantomdolphin@gmail.com';
    try {
        MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
            assert.equal(null, err);

            const db = client.db('eldritch_data');
            db.collection('users').findOne({
                email: toEmail
            }).then(result => {
                if (!result) {
                    sock.write('Account with that email doesn\'t exist');
                    client.close();
                    return;
                } else {
                    var tempPass = generator.generate({
                        length: 10,
                        numbers: true,
                        symbols: true
                    });

                    var emailText = `<h1>Password reset request</h1>` +
                                    `<p>We have received a request for a temporary password for your account. ` +
                                    `If you requested this, please login with the following password ` +
                                    `on your next login attempt. After you login with this password, your old ` +
                                    `password will be invalid and you can change your password in your account settings. ` +
                                    `If you login with your old password, this password will become invalid.</p>` +
                                    `<p>Password: ${tempPass}</p>`
                    const hash = bcrypt.hashSync(tempPass, 10);
                    
                    db.collection('users').updateOne(
                        { email: toEmail },
                        { $push: { password: hash } }
                    ).then(result => {
                        if (result.modifiedCount != 0) {
                            console.log('email sent');
                            sock.write(`Email sent with temporary password`);
                            var mailOptions = {
                                from: myEmail,
                                to: toEmail,
                                subject: 'Password reset request',
                                html: emailText
                            };
                        
                            transporter.sendMail(mailOptions, (error, info) => {
                                if (error) {
                                    console.log(err);
                                } else {
                                    console.log('Email send: ' + info.response);
                                }
                            });
                            client.close();
                            return;
                        } else {
                            console.log('Unable to update database');
                            sock.write('Failed to update database');
                            client.close();
                            return;
                        }
                    }).catch(err => {
                        console.log(err);
                        sock.write(err);
                        client.close();
                        return;
                    });
                }
            }).catch(err => {
                console.log(err);
                sock.write(err);
                client.close();
                return;
            });
        })
    } catch (err) {
        console.log(err);
        sock.write(err);
    }
}

/*const toEmail = 'aphantomdolphin@gmail.com';
try {
    MongoClient.connect(dbconfig.url, { useNewUrlParser: true, useUnifiedTopology: true }, (err, client) => {
        assert.equal(null, err);

        const db = client.db('eldritch_data');
        db.collection('users').findOne({
            email: toEmail
        }).then(result => {
            if (!result) {
                client.close();
                return;
            } else {
                var tempPass = generator.generate({
                    length: 10,
                    numbers: true,
                    symbols: true
                });

                var emailText = `<h1>Password reset request</h1>` +
                                `<p>We have received a request for a temporary password for your account. ` +
                                `If you requested this, please login with the following password ` +
                                `on your next login attempt. After you login with this password, your old ` +
                                `password will be invalid and you can change your password in your account settings. ` +
                                `If you login with your old password, this password will become invalid.</p>` +
                                `<p>Password: ${tempPass}</p>`
                const hash = bcrypt.hashSync(tempPass, 10);
                
                db.collection('users').updateOne(
                    { email: toEmail },
                    { $push: { password: hash } }
                ).then(result => {
                    if (result.modifiedCount != 0) {
                        console.log('email sent');
                        var mailOptions = {
                            from: myEmail,
                            to: toEmail,
                            subject: 'Password reset request',
                            html: emailText
                        };
                    
                        transporter.sendMail(mailOptions, (error, info) => {
                            if (error) {
                                console.log(err);
                            } else {
                                console.log('Email send: ' + info.response);
                            }
                        });
                        client.close();
                        return;
                    } else {
                        console.log('Unable to update database');
                        client.close();
                        return;
                    }
                }).catch(err => {
                    console.log(err);
                    client.close();
                    return;
                });
            }
        }).catch(err => {
            console.log(err);
            client.close();
            return;
        });
    })
} catch (err) {
    console.log(err);
}*/

exports.resetPassword = resetPassword;