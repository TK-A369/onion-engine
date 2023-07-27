namespace OnionEngine.Network
{
	public abstract class NetMessage
	{

	}

	public class WelcomeNetMessage : NetMessage
	{
		Int64 cliendId;

		public WelcomeNetMessage(Int64 cliendId)
		{
			this.cliendId = cliendId;
		}
	}

	public class LoginNetMessage : NetMessage
	{
		public string username = "";

		public LoginNetMessage(string username)
		{
			this.username = username;
		}
	}
}