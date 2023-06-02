using System.Xml;

namespace OnionEngine.Prototypes
{
	class PrototypeManager
	{
		Dictionary<string, Prototype> prototypes = new Dictionary<string, Prototype>();

		public void LoadPrototypes(string xmlPrototype)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlPrototype);
			foreach (XmlNode prototypeNode in doc.SelectNodes("/protos/proto") ?? throw new Exception("Prototype XML error"))
			{
				Prototype prototype = new Prototype();
				Console.WriteLine("Registering prototype: " + prototypeNode.OuterXml);
				foreach (XmlNode entityNode in prototypeNode.SelectNodes("./entities/entity") ?? throw new Exception("Prototype XML error"))
				{
					EntityPrototype entityPrototype = new EntityPrototype();
					Console.WriteLine("  Entity with components:");
					foreach (XmlNode componentNode in entityNode.SelectNodes("./component") ?? throw new Exception("Prototype XML error"))
					{
						ComponentPrototype componentPrototype = new ComponentPrototype();
						string prototypeType = ((componentNode.Attributes ?? throw new Exception("Prototype XML error"))["type"] ?? throw new Exception("Prototype XML error")).Value;
						componentPrototype.name = prototypeType;
						Console.WriteLine("    " + prototypeType);
						entityPrototype.components.Add(componentPrototype);
					}
					prototype.entityList.Add(entityPrototype);
				}
			}
		}
	}
}