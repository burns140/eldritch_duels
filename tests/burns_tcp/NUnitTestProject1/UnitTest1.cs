using NUnit.Framework;
using tcpTest;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Sockets;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace NUnitTestProject1 {
    [TestFixture]
    public class Tests {

        const string id = "5e52dc3058e728656c254d01";
        const string token = "token";
        /*[SetUp]
        public void Setup() {
        }*/


        [Test]
        public void newSignupTest() {
            Random rnd = new Random();
            int val = rnd.Next(10000);

            User user1 = new User("signup", val.ToString() + "@test.edu", "password", val.ToString());
            string json = JsonConvert.SerializeObject(user1);
            string res = sendNetworkRequest(json);

            Assert.IsTrue(res.Contains("User successfully created"));
            
        }

        [Test]
        public void loginTest() {
            User user = new User("login", "testemail@email.edu", "password", "username");
            string json = JsonConvert.SerializeObject(user);
            string res = sendNetworkRequest(json);
            Regex rx = new Regex(@"[A-Za-z0-9]+\.[A-Za-z0-9]+\.[A-Za-z0-9]+.*");
            Assert.IsTrue(rx.Match(res).Success);
        }

        

        [Test]
        public void getCollectionArrayTest() {
            UserInfo info = new UserInfo(id, token, "getCollection");
            string json = JsonConvert.SerializeObject(info);
            string res = sendNetworkRequest(json);

            Assert.IsFalse(res.Contains("Error"));
        }

        [Test]
        public void addCardToCollectionTest() {
            AddCardRequest cardRequest = new AddCardRequest(id, token, "fake", "addCardToCollection");
            string json = JsonConvert.SerializeObject(cardRequest);
            string res = sendNetworkRequest(json);
            Assert.IsTrue(res.Contains("card added successfully"));
        }

        [Test]
        public void addDeckTest() {
            Random rnd = new Random();
            int val = rnd.Next(10000);

            string[] deckArr = { "card1-1", "card2-1", "card3-4", "card4-2" };

            Deck deck = new Deck("saveDeck", val.ToString(), id, deckArr);
            string json = JsonConvert.SerializeObject(deck);
            string res = sendNetworkRequest(json);

            Assert.IsTrue(res.Contains("Deck added successfully"));
        }

        [Test]
        public void getAllDecksTest() {
            GetAllDecksRequest req = new GetAllDecksRequest(id, token, "getAllDecks");
            string json = JsonConvert.SerializeObject(req);
            string res = sendNetworkRequest(json);

            Assert.IsFalse(res.Contains("Error"));
        }

        [Test]
        public void getOneDeckTest() {
            GetOneDeckRequest req = new GetOneDeckRequest("7822", id, token, "getOneDeck");
            string json = JsonConvert.SerializeObject(req);
            string res = sendNetworkRequest(json);
            Assert.IsFalse(res.Contains("Error"));
        }

        [Test]
        public void deleteDeckTest() {
            DeleteDeckRequest req = new DeleteDeckRequest("5399", id, token, "deleteDeck");
            string json = JsonConvert.SerializeObject(req);
            string res = sendNetworkRequest(json);
            Assert.IsTrue(res.Contains("deck successfully deleted"));
        }


        public string sendNetworkRequest(string obj) {
            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

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

        /*[Test]
         public void getCollectionTest() {
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

            Thread.Sleep(2500);

            var obj = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseData);
            foreach(var key in obj.Keys) {
                string str = $"{key}: {obj[key]}";
                Debug.WriteLine(str);
            }
                        
            client.Close();
        } */
    }

}