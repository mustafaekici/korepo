using System;
using ClientConnection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SocketWrapper;

namespace ChatServiceUnitTests
{
    [TestClass]
    public class TestClientTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ArgumentNull_Throws()
        {
            var textclient = new TextClient(null);       
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StartClient_ServerIpNullOrEmpty_Throws()
        {
            var textclient = new TextClient();
            textclient.StartClient(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StartClient_ServerIpWrongFormat_Throws()
        {
            var textclient = new TextClient();
            textclient.StartClient("wrongformat", 1);
        }
 
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartClient_PortSigned_Throws()
        {
            var textclient = new TextClient();
            textclient.StartClient("10.10.0.0", -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartClient_PortMoreThen_Throws()
        {
            var textclient = new TextClient();
            textclient.StartClient("10.10.0.0", 655351);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartClient_PortLessThen0_Throws()
        {
            var textclient = new TextClient();
            textclient.StartClient("10.10.0.0", -23);
        }

        [TestMethod]
        public void SendMessage_EmptyMessage_WontSend()
        {
            var socketmock = Substitute.For<ISocket>();
            var client = new TextClient(socketmock);
            client.Sendmessage(string.Empty);
            socketmock.DidNotReceiveWithAnyArgs().Send(default,default,default);
        }
    }
}
