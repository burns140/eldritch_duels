const Queue = require('../../../server/classes/PlayerQueue');

const should = require('should');

should.Assertion.add('Set', function () {
    this.params.operator = 'to be a set';

    let s = this.obj.slice(0);
    s.should.be.Array();

    s.sort();
    for (let i = 0; i < s.length - 1; i++)
        s[i].should.not.equal(s[i + 1]);
});

should.Assertion.add('arrayEqualUnordered', function (x) {
    let a = this.obj.slice(0);
    let b = x.slice(0);

    this.params.operator = "to have the same unordered contents as";
    this.params.expected = x;


    a.should.be.Set();
    b.should.be.Set();
    a.length.should.equal(b.length);

    a.sort();
    b.sort();

    for (let i in a)
        a[i].should.equal(b[i]);
});

describe('Set', function () {
    it('[a, a]', function () {
        return ['a', 'a'].should.not.be.Set();
    });
    it('[a]', function () {
        return ['a'].should.be.Set();
    })
});

describe('Array Equals Unsorted', function () {
    describe('equal', function () {
        it('[a, aa], [aa, a]', function () {
            return ['a', 'aa'].should.arrayEqualUnordered(['aa', 'a']);
            return ['a', 'aa'].should.arrayEqualUnordered(['a', 'aa']);
        })
    });

    describe('not equal', function () {
        it('[a], [b]', function () {
            return ['a'].should.not.arrayEqualUnordered(['b']);
        });

        it('[a, aa], [a]', function () {
            return ['a', 'aa'].should.not.arrayEqualUnordered(['a']);
        })
    })
});

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
            res.should.arrayEqualUnordered([1, 2]);
        });

        it('Matchmaking after removing players', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 10);

            let res = await queue.matchPlayers();
            res.length.should.equal(0);

            queue.addPlayer(1, 10);
            queue.removePlayer(1);
            queue.addPlayer(2, 20);
            queue.addPlayer(3, 40);
            queue.addPlayer(4, 40);

            res = await queue.matchPlayers();
            res.should.arrayEqualUnordered([0, 2]);

            res = await queue.matchPlayers();
            res.should.arrayEqualUnordered([3, 4]);
        })
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
            queue.addPlayer(0, 20);
            queue.removePlayer(0);

            let res = await queue.matchPlayers();
            res.length.should.equal(0);
        });

        it('2 people in queue', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 1000).should.be.true();
            queue.addPlayer(1, 1000).should.be.true();

            let res = await queue.matchPlayers();
            res.should.arrayEqualUnordered([0, 1]);

            res = await queue.matchPlayers();
            res.length.should.equal(0);
        });

        it('3 people in queue, match best elos', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 20);
            queue.addPlayer(1, 1000);
            queue.addPlayer(2, 25);

            let res = await queue.matchPlayers();
            res.should.arrayEqualUnordered([0, 2]);
            res.length.should.equal(2);
            res.should.containEql(0);
            res.should.containEql(2);
        });

        it('4 people in queue, interleaved elos', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 10);
            queue.addPlayer(1, 1000);
            queue.addPlayer(2, 15);
            queue.addPlayer(3, 1007);

            let res = await queue.matchPlayers();
            res.should.arrayEqualUnordered([0, 2]);

            res = await queue.matchPlayers();
            res.should.arrayEqualUnordered([1, 3]);
        });

        it('matches people with 0 (falsey) elo', async function () {
            let queue = new Queue();
            queue.addPlayer(0, 0).should.be.true();
            queue.addPlayer(1, 0).should.be.true();

            let res = await queue.matchPlayers();
            res.should.arrayEqualUnordered([0, 1]);
        });

        it('eventually matches people with sufficiently different elos', async function () {
            let queue = new Queue(10);
            queue.addPlayer(0, 10).should.be.true();
            queue.addPlayer(1, 21).should.be.true();

            let res = await queue.matchPlayers();
            res.should.arrayEqualUnordered([0, 1]);
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
                res.length.should.equal(2);
                matchesFound++;
            }

            res = await queue.matchPlayers();
            res.length.should.equal(0);
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
                res.length.should.equal(2);
                matchesFound++;
            }

            res = await queue.matchPlayers();
            res.length.should.equal(0);
        });
    });
});