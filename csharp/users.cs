
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;



namespace userCommands
{
    public class User
    {
        private string email;
        private string password;
        private string username;
        private string cmd;

        public User(string cmd)
        {
            this.email = "testemail@email.edu";
            this.cmd = cmd;
            this.password = "password";
            this.username = "username";
        }

        public User(string cmd, string email, string password, string username)
        {
            this.email = email;
            this.cmd = cmd;
            this.password = password;
            this.username = username;
        }
    }

    public class login
    {
        private string email;
        private string password;
        private string cmd;

        public login(string cmd)
        {
            this.email = "testemail@email.edu";
            this.cmd = cmd;
            this.password = "password";
        }

        public login(string cmd, string email, string password)
        {
            this.email = email;
            this.cmd = cmd;
            this.password = password;
        }
    }



    class commands
    {

        static void signup(string email, string password, string username)
        {
            User user = new User("signup", email, password, username);
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

        static void login(string email, string password)
        {
            login user = new login("login", email, password);
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
