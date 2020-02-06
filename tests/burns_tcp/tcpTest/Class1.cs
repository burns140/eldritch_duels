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
        private string email;
        private string password;
        private string username;
        private string cmd;

        public User(string cmd) {
            this.email = "testemail@email.edu";
            this.cmd = cmd;
            this.password = "password";
            this.username = "username";
        }
    }

    class Class1 {

        public static void Main() {
            signupTest();
            loginTest();
        }

        static void signupTest() {
            User user = new User("signup");
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

        static void loginTest() {
            User user = new User("login");
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
