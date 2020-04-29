// TESTS
const should = require('should');
const Elo = require('../../../server/classes/Elo');

describe('Elo', function () {
    it('Equal draw', function () {
        Elo.draw(1000, 1000).should.equal(1000);

        for (let i = 0; i < 1000; i++) {
            let score = Math.round(Math.random() * 2500);
            Elo.draw(score, score).should.equal(score);
        }
    });

    it('Equal win', function () {
        Elo.win(1000, 1000).should.equal(1000 + Elo.K / 2);

        for (let i = 0; i < 1000; i++) {
            let score = Math.round(Math.random() * 2500);
            Elo.win(score, score).should.equal(score + Elo.K / 2);
        }
    });

    it('Equal lose', function () {
        Elo.lose(1000, 1000).should.equal(1000 - Elo.K / 2);

        for (let i = 0; i < 1000; i++) {
            let score = Math.round(Math.random() * 2500);
            Elo.lose(score, score).should.equal(score - Elo.K / 2);
        }
    });

    it('Unequal tests', function () {
        function manualTest(s1, s2, winD, drawD, loseD) {
            Elo.win (s1, s2).should.equal(s1 + winD, `manual test: ${s1} vs ${s2} win`);
            Elo.draw(s1, s2).should.equal(s1 + drawD, `manual test: ${s1} vs ${s2} draw`);
            Elo.lose(s1, s2).should.equal(s1 + loseD, `manual test: ${s1} vs ${s2} lose`);
            Elo.win(s2, s1).should.equal(s2 - loseD);
            Elo.draw(s2, s1).should.equal(s2 - drawD);
            Elo.lose(s2, s1).should.equal(s2 - winD);
        }
        Elo.K.should.equal(32, "This test only works if K = 32");

        // these were calculate by hand
        manualTest(1000, 1010, 16, 0, -16);
        manualTest(1000, 1400, 29, 13, -3);
        manualTest(800, 700, 12, -4, -20);
    })
});