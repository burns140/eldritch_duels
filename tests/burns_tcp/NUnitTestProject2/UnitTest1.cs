using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using tcpTest;

namespace NUnitTestProject2 {

    public class Tests {

        const string id = "5e52cf7918d7573148ba4752";
        const string token = "token";

        [SetUp]
        public void Setup() {
        }

        [Test]
        public void getCollectionTestArray() {
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
            Debug.WriteLine(responseData);
        }
    }
}