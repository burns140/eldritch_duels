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

            Console.WriteLine(json);

            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            data = new byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Thread.Sleep(2500);
            client.Close();

            Debug.WriteLine(responseData);


            Assert.IsTrue(responseData.Contains("User successfully created"));
            
        }

        [Test]
        public void loginTest() {
            User user = new User("login", "testemail@email.edu", "password", "username");
            string json = JsonConvert.SerializeObject(user);

            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            data = new Byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Debug.WriteLine(responseData);

            Regex rx = new Regex(@"[A-Za-z0-9]+\.[A-Za-z0-9]+\.[A-Za-z0-9]+.*");

            Thread.Sleep(2500);
            
            client.Close();
            Assert.IsTrue(rx.Match(responseData).Success);
        }

        [Test]
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
        }

        [Test]
        public static void getCollectionArrayTest() {
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

        [Test]
        public void addCardToCollectionTest() {
            AddCardRequest cardRequest = new AddCardRequest(id, token, "fake", "addCardToCollection");
            string json = JsonConvert.SerializeObject(cardRequest);

            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);
            data = new Byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //Debug.WriteLine("Received: {0}", responseData);
            Debug.WriteLine(responseData);

            Thread.Sleep(2500);

            client.Close();
            Assert.IsTrue(responseData.Contains("card added successfully"));
        }
    }

}