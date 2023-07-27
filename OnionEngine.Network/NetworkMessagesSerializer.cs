using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OnionEngine.Network
{
	public struct NetMessageWrapper
	{
		public string type;
		public XmlNode messageSerialized;
	}

	public class NetworkMessagesSerializer
	{
		Dictionary<string, Type> messageTypes = new Dictionary<string, Type>();

		XmlSerializer netMessageWrapperSerializer = new XmlSerializer(typeof(NetMessageWrapper));

		public NetworkMessagesSerializer()
		{
		}

		public void RegisterMessageType(Type type)
		{
			messageTypes.Add(type.Name, type);
		}

		public string Serialize(NetMessage msg)
		{
			XmlSerializer serializer = new XmlSerializer(msg.GetType());
			XmlDocument doc = new XmlDocument();
			using (XmlWriter xmlWriter = doc.CreateNavigator()!.AppendChild())
			{
				serializer.Serialize(xmlWriter, msg);
			}

			NetMessageWrapper wrappedMsg = new NetMessageWrapper() { messageSerialized = doc, type = msg.GetType().Name };
			StringWriter stringWriter = new StringWriter();
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter,
				new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = false, OmitXmlDeclaration = true }))
			{
				netMessageWrapperSerializer.Serialize(xmlWriter, wrappedMsg);
			}

			return stringWriter.ToString();
		}

		public Tuple<Type, NetMessage> Deserialize(string msg)
		{
			NetMessageWrapper wrappedMsg =
				(NetMessageWrapper)(netMessageWrapperSerializer.Deserialize(new StringReader(msg))
					?? throw new Exception("Couldn't deserialize network message"));

			Type msgType = messageTypes[wrappedMsg.type];
			XmlSerializer serializer = new XmlSerializer(msgType);
			using (XmlReader xmlReader = new XmlNodeReader(wrappedMsg.messageSerialized))
			{
				return new Tuple<Type, NetMessage>(msgType, (NetMessage)(serializer.Deserialize(xmlReader)
					?? throw new Exception("Couldn't deserialize network message")));
			}
		}
	}
}