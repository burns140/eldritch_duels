const LinkedList = require("./LinkedList");
const Tree = require('binary-search-tree').BinarySearchTree
const _ = require('lodash');


/**
 * Class that players join when waiting for a game.
 * matches 2 players with similar elo
 * will prioritize first added
 * @typedef {number | string} ID type of ID
 * @typedef {{ elo: number, list: LinkedList<number> }} TreeData
**/
class PlayerQueue {
    /**
     * @param {number} eloDiff - The initial max difference between 2 players' elo in order to be matched
     */
    constructor(eloDiff = 10) {
        if (!_.isNumber(eloDiff))
            throw Error("Must provide number");

        eloDiff = Math.abs(eloDiff);
        if (eloDiff == 0)
            eloDiff = 1; // Must allow some wiggle room

        this.size = 0;
        this.eloDifference = eloDiff;

        // doubles every time a player goes unmatched in a matchPlayers.
        // resets to eloDifference when a player gets matched.
        this.currentEloRange = eloDiff;

        // list of IDs
        this.queue = /** @type {LinkedList<ID>} */ (new LinkedList());

        // faster way to find matches
        this.tree = new Tree({
            unique: true
        });
        
        /**
         * id => Data
         * @typedef {number} Data
        */
        this.queueData = /** @type {Object.<ID, Data> */ ({});
    }
    
    /**
     * adds a player with a given elo (competetive socre)
     * @param {ID} id
     * @param {number} elo
     */
    addPlayer(id, elo) {
        if (this.queueData[id])
            return false;

        this.queue.enqueue(id);
        this.queueData[id] = elo;

        let treeData = this.tree.search(elo);
        if (treeData == null || treeData.length == 0) {
            this.tree.insert(elo, { list: new LinkedList(id), elo: elo });
        } else
            treeData[0].list.push(id);

        this.size++;
        return true;
    }

    /**
     * Removes a player from the queue
     * @param {ID} id
     */
    removePlayer(id) {
        if (this.queueData[id]) {
            delete this.queueData[id];
            this.size--;

            if (this.queue.peekFirst() == id) {
                this.queue.dequeue();
                this.currentEloRange = this.eloDifference;
            }

            return true;
        }

        return false;
        // The rest of the operations take more than O(1) time, so they can be handled during O(n) and O(log n) operations
    }
    
    /**
     * Use for stuff like account deletion or if you know what you're doing
     * If the player is still enqueued, elo is optional
     * @param {ID} id
     * @param {number} [elo]
     */
    deepRemovePlayer(id, elo) {
        // remove id from data
        if (this.queueData[id]) {
            elo = this.queueData[id];
            delete this.queueData[id];
            this.size--;
        }

        // remove id from queue
        for (let it = this.queue.it(); it.hasValue(); it.next()) {
            do {
                if (it.value() == id) {
                    it.delete();
                    continue;
                }

                break;
            } while (true);
        }

        // remove id from elo tree
        // doesn't do full delete
        if (elo == undefined)
            return;

        let data = this.tree.search(elo);
        for (let it = data.list.it(); it.hasValue(); it.next()) {
            do {
                if (it.value() == id) {
                    it.delete();
                    contineu;
                }

                break;
            } while (true);
        }
    }

    /**
     * @param {(id: ID)} func - a function that will receive every queued ID, one at a time
     */
    forEachId(func) {
        let it = this.queue.it();
        while (it.hasValue()) {
            let id = it.value();
            if (!this.queueData[id]) {
                it.delete();
                continue;
            }

            func(id);
        }
    }
	
    /**
     * Call this to match players with another
     * @returns {Promise<ID[]>} Promises matched players as an array of couples (array of len 2). If no one is matched, resolves an empty array
    */
	matchPlayers() {
		return new Promise(res => {
			if (this.size <= 1) {
				res([]);
				return;
            }

            // get topmost queued player
            let it = this.queue.it();

            /** @type {ID} */
            let id

            /** @type {number} */
            let elo;

            while (it.hasValue()) {
                id = it.value();
                elo = this.queueData[id];
                if (elo == undefined) {
                    it.delete();
                    continue;
                }

                let matchingPlayers = /** @type {TreeData[]} */ (this.tree.betweenBounds({ $lte: elo + this.currentEloRange, $gte: elo - this.currentEloRange }));
                if (matchingPlayers.length != 0) {
                    // favor matching higher elo players, because the growth is unbounded
                    /** @type {number[]} */
                    let emptyEloBins = [];
                    for (let i = matchingPlayers.length - 1; i >= 0; i--) {
                        let data2 = matchingPlayers[i];
                        let it2 = data2.list.it();

                        while (it2.hasValue()) {
                            let id2 = it2.value();
                            let elo2 = /** @type {number} */ (this.queueData[id2]);
                            if (elo2 == undefined || elo2 != data2.elo) {
                                it2.delete();
                                continue;
                            }

                            if (id2 == id) {
                                it2.next();
                                continue;
                            }

                            // ids found
                            // cleanup
                            it.delete();
                            it2.delete();

                            if (data2.list.isEmpty())
                                this.tree.delete(elo2);

                            let firstPlayerTreeData = this.tree.search(elo);
                            if (firstPlayerTreeData.length != 0) {
                                firstPlayerTreeData = firstPlayerTreeData[0];
                                if (firstPlayerTreeData.list.peekFirst() == id) {
                                    firstPlayerTreeData.list.dequeue();
                                    if (firstPlayerTreeData.list.isEmpty())
                                        this.tree.delete(elo);
                                }
                            }

                            delete this.queueData[id];
                            delete this.queueData[id2];

                            for (let bin of emptyEloBins)
                                this.tree.delete(bin);

                            this.currentEloRange = this.eloDifference;

                            this.size -= 2;

                            // id2 and id will enter a match
                            res([id, id2]);
                            return;
                        }

                        if (!it2.hasValue())
                            emptyEloBins.push(data2.elo);
                    }

                    for (let bin of emptyEloBins)
                        this.tree.delete(bin);
                }

                // no one matched
                this.currentEloRange *= 2;
                res([]);
                return;
            }

            res([]);
            return;
		});
    }
}

module.exports = PlayerQueue;