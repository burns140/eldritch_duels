
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;



namespace collectionCommands
{
    public class User
    {
        private string id;
        private string cmd;

        public User(string cmd, string id)
        {
            this.id = id;
            this.cmd = cmd;
        }
    }

    public class Card
    {
        private string id;
        private string cmd;
        private string cardid;

        public Card(string cmd, string id, string cardid)
        {
            this.id = id;
            this.cmd = cmd;
            this.cardid = cardid;
        }
    }

    class commands
    {

        static void getCollection(string id)
        {
            User user = new User("getCollection", id);
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

        static void addtoCollection(string id, string cardid)
        {
            Card card = new Card("addCardToCollection", id, cardid);
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

        static void removeFromCollection(string id, string name)
        {
            Card card = new Card("removeCardFromCollection", id, cardid);
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
