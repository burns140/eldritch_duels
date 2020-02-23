using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace tcpTest {

    public class User {
        public string email;
        public string password;
        public string username;
        public string cmd;

        public User(string cmd, string email, string password, string username) {
            this.email = email;
            this.cmd = cmd;
            this.password = password;
            this.username = username;
        }
    }

    public class UserInfo {
        public string id;
        public string token;
        public string cmd;

        public UserInfo(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    public class AddCardRequest {
        public string id;
        public string token;
        public string cardid;
        public string cmd;

        public AddCardRequest(string id, string token, string cardid, string cmd) {
            this.id = id;
            this.token = token;
            this.cardid = cardid;
            this.cmd = cmd;
        }
    }

    class Class1 {

        const string id = "5e52dc3058e728656c254d01";
        const string token = "token";

        public static void Main() {
            //signup();
            login();
            //getCollectionArray();
        }

        static void getCollectionArray() {
            UserInfo info = new UserInfo(id, token, "getCollection");
            string json = JsonConvert.SerializeObject(info);

            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);
            data = new Byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            Thread.Sleep(1000);
            System.Console.WriteLine(responseData);
            Thread.Sleep(2500);
            client.Close();
        }

        static void signup() {
            User user = new User("signup", "testemail@email.edu", "password", "username");
            string json = JsonConvert.SerializeObject(user);

            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            //Byte[] data = System.Text.Encoding.ASCII.GetBytes("{\"cmd\": \"signup\"}");
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            NetworkStream stream = client.GetStream();

            Console.WriteLine(data);

            stream.Write(data, 0, data.Length);
            Console.WriteLine("Sent");

            data = new Byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);

            Thread.Sleep(2500);

            client.Close();

        }

        static void login() {
            User user = new User("login", "aphantomdolphin@gmail.com", "yU7&Ioj{.F", "username");
            string json = JsonConvert.SerializeObject(user);

            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);
            Console.WriteLine("Sent");

            data = new Byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);

            Thread.Sleep(2500);

            client.Close();
        }

    }
}
