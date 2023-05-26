
using System.Net;
using P2P.Chat;

namespace P2P_Tests
{
	public class ChatTests
	{
		[Test]
		public void ChatCreate()
		{
			var chat = new Chat(58005);
			chat.Dispose();
		}

		[Test]
		public void TwoChatsInteract()
		{
			var messageReceived = 0;

			var chat58000 = new Chat(58000);
			var chat58001 = new Chat(58001);
			chat58001.ConnectTo(IPEndPoint.Parse("127.0.0.1:58000"));

			chat58000.OnMessage += _ => messageReceived++;
			chat58001.OnMessage += _ => messageReceived++;

			Thread.Sleep(100);

			chat58000.Send("A");

			Thread.Sleep(100);

			chat58001.Send("B");

			Thread.Sleep(100);

			Assert.That(4, Is.EqualTo(messageReceived));

			chat58000.Dispose();
			chat58001.Dispose();
		}

		[Test]
		public void ThreeChatsInteract()
		{
			Interact(3, new List<int>());
		}

		[Test]
		public void FourChatsInteract()
		{
			Interact(4, new List<int>());
		}

		[Test]
		public void FiveChatsInteract()
		{
			Interact(5, new List<int>());
		}

		[Test]
		public void ThreeChatsInteractDestruction()
		{
			Interact(3, new List<int> { 1, 2 });
		}

		[Test]
		public void FourChatsInteractDestruction()
		{
			Interact(4, new List<int> { 2, 0 });
		}

		[Test]
		public void FiveChatsInteractDestruction()
		{
			Interact(5, new List<int> { 1, 3, 4 });
		}

		private void Interact(int chatCount, List<int> disposeSeq)
		{
			var messageReceived = 0;

			var chats = new List<Chat>();

			for (var i = 0; i < chatCount; i++)
			{
				chats.Add(new Chat(58003 + i));
				Thread.Sleep(100);
			}

			foreach (var chat in chats)
			{
				chat.OnMessage += _ => messageReceived++;

				if (chat == chats.FirstOrDefault())
					continue;

				chat.ConnectTo(IPEndPoint.Parse("127.0.0.1:58003"));
				Thread.Sleep(100);
			}

			foreach (var elem in disposeSeq)
			{
				chats[elem].Dispose();

				Thread.Sleep(100);
			}

			var index = 0;

			foreach (var chat in chats)
			{
				if (disposeSeq.Contains(index++))
					continue;

				chat.Send(Convert.ToString(index));

				Thread.Sleep(100);
			}

			chatCount -= disposeSeq.Count;

			Assert.That(chatCount * chatCount, Is.EqualTo(messageReceived));

			index = 0;

			foreach (var chat in chats)
			{
				if (disposeSeq.Contains(index++))
					continue;

				chat.Dispose();
			}

			Thread.Sleep(100);
		}
	}
}