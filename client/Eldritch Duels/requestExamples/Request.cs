using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTestProject1 {

    /* Superclass to be used for all requests */
    public class Request {
        public string id;
        public string token;
        public string cmd;

        public Request(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }

        public Request(string cmd) {
            this.id = "";
            this.token = "";
            this.cmd = cmd;
        }
    }
}
