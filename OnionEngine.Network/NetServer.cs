using OnionEngine.IoC;

using System.Net;
using System.Net.Sockets;

namespace OnionEngine.Network
{
	enum ClientState
	{
		NoIdSent,
		NotLoggedIn,
		LoggedIn
	}

	class ClientHandler
	{
		Socket socket;

		Int64 clientId;

		ClientState state = ClientState.NoIdSent;

		[Dependency]
		SocketFrameManager socketFrameManager = default!;

		public ClientHandler(Socket socket, Int64 clientId)
		{
			this.socket = socket;
			this.clientId = clientId;
		}

		public async void HandleClient()
		{
			WelcomeNetMessage welcomeMessage = new WelcomeNetMessage(clientId);
			await socketFrameManager.SendMessage(welcomeMessage);
			state = ClientState.NotLoggedIn;

			((Func<Task>)(async () =>
			{

			}))().Start();
		}
	}

	public class NetServer
	{
		Socket? listener;

		Int64 nextClientId = 0;

		Dictionary<int, ClientHandler> clients = new Dictionary<int, ClientHandler>();

		bool running = false;

		NetworkMessagesSerializer? networkMessagesSerializer;

		SocketFrameManager? socketFrameManager;

		async void Start(int port, CancellationToken cancellationToken)
		{
			running = true;

			networkMessagesSerializer = IoCManager.CreateInstance<NetworkMessagesSerializer>(new object[] { });
			socketFrameManager = IoCManager.CreateInstance<SocketFrameManager>(new object[] { });

			IPHostEntry ipHostEntry = await Dns.GetHostEntryAsync("localhost");
			IPAddress ipAddress = ipHostEntry.AddressList[0];
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

			listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind(ipEndPoint);
			listener.Listen(100);

			((Func<Task>)(async () =>
			{
				while (running)
				{
					Socket socket = await listener.AcceptAsync(cancellationToken);

					Int64 clientId = nextClientId;
					nextClientId++;

					ClientHandler clientHandler = IoCManager.CreateInstance<ClientHandler>(new object[] { socket, clientId });
				}
			}))().Start();
		}
	}
}