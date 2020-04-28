// If you change this, disable the "Unequal tests" test in ~/test/server/classes/Elo.js
const K = 32;

const WIN = 1;
const LOSE = 0;
const DRAW = (WIN + LOSE) / 2;

/**
 * Your expected win rate vs the opponent
 * @param {number} current - your current Elo score
 * @param {number} opp - opponent's current Elo score
 * @returns {number} Probability that you will win
 */
function expected(current, opp) {
    return 1 / (1 + Math.pow(10, (opp - current) / 400));
}

/**
 * Gets your score based on the results of the battle
 * @param {number} score - your current score
 * @param {number} opp - your opponent's score
 * @param {number} result - the result of the battle
 * @returns {number} your new score
 */
function getNewScore(score, opp, result) {
    return Math.round(score + K * (result - expected(score, opp)));
}

/**
 * Calculates your score if you win
 * @param {number} yourScore
 * @param {number} oppScore
 * @returns {number} your new score
 */
function win(yourScore, oppScore) {
    return getNewScore(yourScore, oppScore, WIN);
}

/**
 * Calculates your score if you lose
 * @param {number} yourScore
 * @param {number} oppScore
 * @returns {number} your new score
 */
function lose(yourScore, oppScore) {
    return getNewScore(yourScore, oppScore, LOSE);
}

/**
 * Calculates your score if you draw
 * @param {number} yourScore
 * @param {number} oppScore
 * @returns {number} your new score
 */
function draw(yourScore, oppScore) {
    return getNewScore(yourScore, oppScore, DRAW);
}

module.exports.win = win;
module.exports.lose = lose;
module.exports.draw = draw;
module.exports.K = K;