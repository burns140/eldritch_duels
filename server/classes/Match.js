module.exports = class Match {
    init() {
        this.ids = [];
        this.sockets = {};
        this.closeFuncs = {};
        this.dataFuncs = {};
    }

    constructor() {
        this.init();
    }

    addPlayer(id, socket) {
        this.ids.push(id);
        this.sockets[id] = socket;
        this.closeFuncs[id] = () => {
            this.endMatch(id);
        };

        socket.once('close', this.closeFuncs[id]);

        this.dataFuncs[id] = data => {
            this.forEachPlayer((cid, sock) => {
                if (cid == id)
                    return;

                sock.write(data);
            });
        }

        socket.on('data', this.dataFuncs[id]);
    }

    forEachPlayer(handler) {
        for (let i = 0; i < this.ids.length; i++) {
            let id = this.ids[i];
            let socket = this.sockets[i];
            handler(id, socket);
        }
    }

    endMatch(skipId = undefined) {
        this.forEachPlayer((id, sock) => {
            if (id == skipId)
                return;

            let fn;
            if (fn = this.closeFuncs[id])
                sock.off('close', fn);

            if (fn = this.dataFuncs[id])
                sock.off('data', fn);

            sock.write('match ended');
        });

        this.init();
    }
}