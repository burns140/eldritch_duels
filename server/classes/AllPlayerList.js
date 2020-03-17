const BiMap = require('bidirectional-map');

module.exports = class AllPlayerList {

    constructor() {
        this.playerList = new BiMap();
    }

    addPlayer(id, sock) {
        this.playerList.set(id, sock);
    }

    /* Remove a player from the map using id as key */
    removePlayer(id) {
        this.playerList.delete(id);
    }

    /* Remove a player from the map using socket as key */
    removeSocket(socket) {
        this.playerList.deleteValue(socket);
    }

    /* Get the playerlist */
    getList() {
        return this.playerList;
    }

    /* Get a socket using the id as a key */
    getSocketByKey(id) {
        return this.playerList.get(id);
    }

    /* Get the id, using the socket as a key */
    getKeyBySocket(socket) {
        return this.playerList.getKey(socket)
    }

    /* Check if the user is already logged in */
    isLoggedIn(id) {
        if (this.playerList.has(id)) {
            return true;
        }
        return false;
    }
}