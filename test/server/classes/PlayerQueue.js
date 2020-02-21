const Queue = require('../../../server/classes/PlayerQueue');

const should = require('should');

describe('Player Queue', function () {
    describe('Integrity', function () {
        it('Can add player only one time', function () {
            let queue = new Queue();
            queue.addPlayer(0, 10).should.be.true();
            queue.addPlayer(0, 10).should.be.false();
            queue.addPlayer(0, 11).should.be.false();
            queue.addPlayer(1, 10).should.be.true();
        });

        it('Removing players', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 10).should.be.true();
            queue.removePlayer(0).should.be.true();
            queue.removePlayer(0).should.be.false();

            queue.addPlayer(0, 11).should.be.true();
            queue.removePlayer(0).should.be.true();

            queue.addPlayer(1, 10).should.be.true();
            queue.addPlayer(2, 10).should.be.true();

            let res = await queue.matchPlayers();
            res.length.should.equal(2);
            res.should.containEql(1);
            res.should.containEql(2);
        });
    });

    describe('Matchmaking', function () {
        this.slow(5);

        it('Empty queue', async function () {
            this.slow(3);
            let queue = new Queue();
            let res = await queue.matchPlayers();
            res.length.should.equal(0);
        });

        it('1 player in queue', async function () {
            this.slow(3);
            let queue = new Queue();
            queue.addPlayer(1, 20);
            let res = await queue.matchPlayers();
            res.length.should.equal(0);
        });

        it('2 people in queue', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 1000).should.be.true();
            queue.addPlayer(1, 1000).should.be.true();

            let res = await queue.matchPlayers();
            res.length.should.equal(2);
            res.should.containEql(0);
            res.should.containEql(1);

            res = await queue.matchPlayers();
            res.length.should.equal(0);
        });

        it('3 people in queue, match best elos', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 20);
            queue.addPlayer(1, 1000);
            queue.addPlayer(2, 25);

            let res = await queue.matchPlayers();
            res.length.should.equal(2);
            res.should.containEql(0);
            res.should.containEql(2);
        });

        it('matches people with 0 (falsey) elo', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 0).should.be.true();
            queue.addPlayer(1, 0).should.be.true();

            let res = await queue.matchPlayers();
            res.length.should.equal(2);
            res.should.containEql(0);
            res.should.containEql(1);
        });

        it('eventually matches people with sufficiently different elos', async function () {
            let queue = new Queue(10);
            queue.addPlayer(0, 10).should.be.true();
            queue.addPlayer(1, 21).should.be.true();

            let res = await queue.matchPlayers();
            res.length.should.equal(0);

            res = await queue.matchPlayers();
            res.length.should.equal(2);
            res.should.containEql(0);
            res.should.containEql(1);
        });

        it('large number of users', async function () {
            this.slow(75);
            let queue = new Queue();
            const sz = 1000;
            for (let i = 0; i < sz; i++)
                queue.addPlayer(i, i).should.be.true();

            let matchesFound = 0;
            while (matchesFound != Math.floor(sz / 2)) {
                let res = await queue.matchPlayers();
                if (res.length == 2)
                    matchesFound++;
            }
        });

        // more representative of real-world data, and it runs faster
        it('large number of users and random elos', async function () {
            this.slow(40);
            let queue = new Queue();
            const sz = 1000;

            for (let i = 0; i < sz; i++)
                queue.addPlayer(i, Math.floor(Math.random() * 2000)).should.be.true();

            let matchesFound = 0;
            while (matchesFound != Math.floor(sz / 2)) {
                let res = await queue.matchPlayers();
                if (res.length == 2)
                    matchesFound++;
            }
        });
    });
});