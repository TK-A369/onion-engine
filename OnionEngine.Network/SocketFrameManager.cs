using OnionEngine.Core;

using System.Text;
using System.Net.Sockets;

namespace OnionEngine.Network
{
	public class SocketFrameManager
	{
		public NetworkMessagesSerializer networkMessagesSerializer;

		public Event<NetMessage> onMessageReceived;

		Socket socket;

		Queue<byte> rxFifo = new Queue<byte>();

		public SocketFrameManager(Socket _socket)
		{
			socket = _socket;
			networkMessagesSerializer = new NetworkMessagesSerializer();
			onMessageReceived = new Event<NetMessage>();
		}

		public async void Tick()
		{
			// Receive data
			const int bufferSize = 1024;
			byte[] buffer = new byte[bufferSize];
			int bytesReceived = await socket.ReceiveAsync(buffer, SocketFlags.None);

			for (int i = 0; i < bytesReceived; i++)
			{
				rxFifo.Enqueue(buffer[i]);
			}

			// Try to parse frames
			if (rxFifo.Count >= 1)
			{
				int dataSize = rxFifo.Peek();
				if (rxFifo.Count >= dataSize + 1)
				{
					rxFifo.Dequeue();

					byte[] dataBytes = new byte[dataSize];
					for (int i = 0; i < dataSize; i++)
					{
						dataBytes[i] = rxFifo.Dequeue();
					}
					string dataStr = Encoding.ASCII.GetString(dataBytes);

					var (msgType, msg) = networkMessagesSerializer.Deserialize(dataStr);
					onMessageReceived.Fire(msg);
					Console.WriteLine("Received message of type " + msgType + ": " + msg);
				}
			}
		}

		public async Task SendMessage(NetMessage message)
		{
			string messageSerialized = networkMessagesSerializer.Serialize(message);

			await socket.SendAsync(Encoding.ASCII.GetBytes(messageSerialized));
		}
	}
}