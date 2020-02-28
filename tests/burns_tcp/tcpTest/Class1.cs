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
        public string token;

        public User(string cmd, string email, string password, string username) {
            this.email = email;
            this.cmd = cmd;
            this.password = password;
            this.username = username;
        }

        public User(string cmd, string email, string password, string username, string token) {
            this.email = email;
            this.cmd = cmd;
            this.password = password;
            this.username = username;
            this.token = token;
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

    public class GetAllDecksRequest {
        public string id;
        public string token;
        public string cmd;

        public GetAllDecksRequest(string id, string token, string cmd) {
            this.id = id;
            this.token = token;
            this.cmd = cmd;
        }
    }

    public class GetOneDeckRequest : Request {
        public string name;

        public GetOneDeckRequest(string deckname, string id, string token, string cmd) : base(id, token, cmd) {
            this.name = deckname;
        }
    }

    public class DeleteDeckRequest : Request {
        public string name;

        public DeleteDeckRequest(string name, string id, string token, string cmd) : base(id, token, cmd) {
            this.name = name;
        }
    }

    public class PassRequest : Request {
        public string email;

        public PassRequest(string email, string cmd) : base(cmd) {
            this.email = email;
        }
    }

    public class Deck {
        public string cmd;
        public string name;
        public string id;
        public string[] deck;

        public Deck(string cmd, string name, string id, string[] deck) {
            this.cmd = cmd;
            this.name = name;
            this.id = id;
            this.deck = deck;
        }
    }

    class Class1 {

        const string id = "5e58196bc424443f28445126";
        const string token = "token";
        const string validToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkYXRhIjp7ImlkIjoiNWU1MmRjMzA1OGU3Mjg2NTZjMjU0ZDAxIiwiZW1haWwiOiJ0ZXN0ZW1haWxAZW1haWwuZWR1In0sImlhdCI6MTU4MjU3MTQ4N30.H76TaFoOWSePOb4NKChPVpH7xeI-t0SuO2gL7zt8z0s";

        public static void Main() {
            //signup();
            //login();
            //getCollectionArray();
            //newSignupTest();
            //createUsers();
            //getAllDecksTest();
            //newSignupTest();
            addQueueTest();
        }

        public static void addQueueTest() {
            User user1 = new User("enterQueue", "aphantomdolphin@gmail.com", "a", "rand", validToken);
            string json = JsonConvert.SerializeObject(user1);
            string res = sendNetworkRequestClass(json);
            Console.WriteLine(res);
            Thread.Sleep(5000);
        }

        static void getAllDecksTest()
        {
            GetAllDecksRequest req = new GetAllDecksRequest(id, validToken, "getAllDecks");
            string json = JsonConvert.SerializeObject(req);
            string res = sendNetworkRequestClass(json);

        }
        static void createUsers()
        {

            for (int i = 0; i < 6; i++)
            {
                Random rnd = new Random();
                int val = rnd.Next(10000);

                User user1 = new User("signup", val.ToString() + "@test.edu", "password", val.ToString());
                string json = JsonConvert.SerializeObject(user1);

                Console.WriteLine("json: " + json);

                Int32 port = 8000;
                TcpClient client = new TcpClient("localhost", port);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                data = new byte[256];
                string responseData = string.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                Console.WriteLine("response: " + responseData);
                Thread.Sleep(2500);

                client.Close();
            }
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

        static void newSignupTest() {
            Random rnd = new Random();
            int val = rnd.Next(10000);

            User user1 = new User("signup", val.ToString() + "@test.edu", "password", val.ToString());
            string json = JsonConvert.SerializeObject(user1);

            Console.WriteLine("json: " + json);

            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            data = new byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            
            Console.WriteLine("response: " + responseData);
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
            User user = new User("login", "954@test.edu", "password", "username");
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

        public static string sendNetworkRequestClass(string obj)
        {
            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(obj);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);
            data = new Byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine(responseData);

            Thread.Sleep(2500);

            client.Close();

            return responseData;
        }

    }
}
