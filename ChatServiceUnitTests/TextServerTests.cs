using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using ServerCore;
using SocketWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServiceUnitTests
{
    [TestClass]
    public class TextServerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ArgumentNull_Throws()
        {
            var textserver = new TextServer(null,null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartServer_PortMoreThen_Throws()
        {
            var textserver = new TextServer();
            textserver.StartServer(655351);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StartServer_PortLessThen0_Throws()
        {
            var textserver = new TextServer();
            textserver.StartServer(-1);
        }

        [TestMethod]     
        public void StartServer_CantGetHostName_ReturnsErrorMessage()
        {
            var dnsfake = Substitute.For<IDns>();
            var socketmock = Substitute.For<ISocket>();
            var textserver = new TextServer(socketmock, dnsfake);
            dnsfake.GetHostName().Returns(er=> { throw new Exception("SomeFakeExceptionMessage"); });
            var returnmessage = textserver.StartServer(11);
            Assert.AreEqual("SomeFakeExceptionMessage", returnmessage);
        }

        [TestMethod]
        public void StartServer_CantGetHostByName_ReturnsErrorMessage()
        {
            var dnsfake = Substitute.For<IDns>();
            var socketmock = Substitute.For<ISocket>();
            var textserver = new TextServer(socketmock, dnsfake);
            dnsfake.GetHostByName(Arg.Any<string>()).Returns(er => { throw new Exception("SomeFakeExceptionMessage"); });
            var returnmessage = textserver.StartServer(11);
            Assert.AreEqual("SomeFakeExceptionMessage", returnmessage);

        }

        [TestMethod]
        public void OnRecievedData_MoreThanOneMessagePersecond_SendWarnClient()
        {
            HelperStub mystub = new HelperStub();
            TextServer.OnRecievedData(mystub);
            TextServer.firstTime = TextServer.lastTime;
            TextServer.OnRecievedData(mystub);
            mystub.socketmock.ReceivedWithAnyArgs().Send(default, default, default);

        }
    }
    class HelperStub : IAsyncResult
    {

        public ISocket socketmock;
        public TextClientPackage clientPackage;
        public HelperStub()
        {
            socketmock = Substitute.For<ISocket>();
            clientPackage = new TextClientPackage(socketmock);
        }
        public bool IsCompleted => throw new NotImplementedException();

        public WaitHandle AsyncWaitHandle => throw new NotImplementedException();

        public object AsyncState => clientPackage;

        public bool CompletedSynchronously => throw new NotImplementedException();
    }
}
