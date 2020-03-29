module.exports = class Match {
    init() {
        /** @type {ID[]} */
        this.ids = [];

        /** @type {import("net").Socket[]} */
        this.sockets = [];

        /** @type {Object.<ID, Function} */
        this.closeFuncs = {};

        /** @type {Object.<ID, Function} */
        this.dataFuncs = {};
    }

    constructor(dataHandler) {
        this.dataHandler = dataHandler;

        this.init();
    }

    /**
     * Adds a player to the match
     * @param {string|number} id
     * @param {Socket} socket
     */
    addPlayer(id, socket) {
        console.log('adding player to match');
        console.log(this.ids);
        this.ids.push(id);
        this.sockets.push(socket);

        /* THIS DOES NOT WORK DO FUCKING REMOVE THIS */
        /*if (this.sockets.length == 1) {
            this.sockets[0].write("my turn");
        }*/

        //ask on discord before changing anything in this file plz

        this.closeFuncs[id] = () => {
            this.endMatch(id);
        };

        socket.once('close', this.closeFuncs[id]);

        this.dataFuncs[id] = data => {
            if (data == "YOU LOSE\n") {
                this.endMatch(data, id);
                return;
            }
            
            this.forEachPlayer((cid, sock) => {
                if (cid == id) {
                    return;
                }
                
                sock.write(data);
            });
        }

        socket.on('data', this.dataFuncs[id]);
    }
    
    /** 
     * @param {(string|number, import("net").Socket)} handler
     */
    forEachPlayer(handler) {
        for (let i = 0; i < this.ids.length; i++) {
            let id = this.ids[i];
            let socket = this.sockets[i];
            handler(id, socket);
        }
    }
    
    endMatch(data, skipID) {
        this.forEachPlayer((id, sock) => {

            try {
                if (skipID == id)
                    return;

                let fn;
                if (fn = this.closeFuncs[id])
                    sock.off('close', fn);

                if (fn = this.dataFuncs[id])
                    sock.off('data', fn);

                sock.write(data);

                sock.on("data", this.dataHandler);
            } catch (err) {
                console.log(err);
                sock.write(err.toString());
            }
            
        });

        this.init();
    }
}