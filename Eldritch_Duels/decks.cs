
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;



namespace deckCommands
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

    public class Deck
    {
        private string id;
        private string cmd;
        private string name;
        private int[] deck;

        public Deck(string cmd, string id, int[] deck, string name)
        {
            this.id = id;
            this.cmd = cmd;
            this.deck = deck;
            this.name = name;
        }
        public Deck(string cmd, string id, string name)
        {
            this.id = id;
            this.cmd = cmd;
            this.name = name;
            this.deck = new int[0];
        }
        public Deck(string cmd, string id)
        {
            this.id = id;
            this.cmd = cmd;
            this.name = "";
            this.deck = new int[0];
        }
    }

    class commands
    {

        static void getDecks(string id)
        {
            Deck deck = new Deck("getDecks", id);
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

        static void saveDeck(string id, int[] deck, string name)
        {
            Deck deck = new Deck("saveDeck", id, deck, name);
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

        static void deleteDeck(string id, string name)
        {
            Deck deck = new Deck("deleteDeck", id, name);
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