﻿using System.Net;
using P2P.Network;

namespace P2P_Tests
{
	internal class SocketTests
	{
		[Test]
		public void ConnectorListenerTest()
		{
			var listener = new Listener(58003);
			var listenerThread = new Thread(() => AcceptConnection(listener));
			var connectThread = new Thread(() => ConnectToListener(5003));
			listenerThread.Start();
			connectThread.Start();
			listenerThread.Join();
			connectThread.Join();
			listener.Close();
		}

		private void AcceptConnection(Listener listener)
		{
			var accepted = listener.Accept();
			var received = accepted.Receive();
			Assert.AreEqual(received, "C2L");
			accepted.Send("L2C");
		}

		private void ConnectToListener(int port)
		{
			var connection = new Connection(IPEndPoint.Parse("127.0.0.1:" + port));
			connection.Send("C2L");
			var received = connection.Receive();
			connection.Close();
			Assert.AreEqual(received, "L2C");
		}
	}
}
