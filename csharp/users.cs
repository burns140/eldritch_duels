
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO.Path;



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

        static string login(string email, string password)
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
            if (String.Equals(responseData,"Incorrect password"))
            {
                client.Close();
                return String.Empty;
            }
            string tempFile = "LoginTemp";
            try
            {
                tempFile = Path.GetTempFileName();
                FileInfo fileInfo = new FileInfo(tempFile);
                fileInfo.Attributes = FileAttributes.Temporary;
                Console.WriteLine("TEMP file created at: " + tempFile);
                try
                {
                    string[] loginstuff = responseData.Split(':');
                    // Write to the temp file.
                    StreamWriter streamWriter = File.AppendText(tempFile);
                    streamWriter.WriteLine(loginstuff[0]); // token
                    streamWriter.WriteLine(loginstuff[1]); // ID
                    streamWriter.Flush();
                    streamWriter.Close();

                    Console.WriteLine("Temporary login file updated.");
                    client.Close();
                    return tempFile;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error writing to login file: " + e.Message);
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to create login file or set its attributes: " + e.Message);
                client.Close();
            }
        }
    }
}
