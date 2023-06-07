using System.Xml;

using OnionEngine.Core;

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
				string prototypeName = ((prototypeNode.Attributes ?? throw new Exception("Prototype XML error"))["name"] ?? throw new Exception("Prototype XML error")).Value;
				Console.WriteLine("Registering prototype: " + prototypeName);
				foreach (XmlNode entityNode in prototypeNode.SelectNodes("./entities/entity") ?? throw new Exception("Prototype XML error"))
				{
					List<ComponentPrototype> componentPrototypes = new List<ComponentPrototype>();
					string entityName = ((entityNode.Attributes ?? throw new Exception("Prototype XML error"))["name"] ?? throw new Exception("Prototype XML error")).Value;
					Console.WriteLine("  Entity with components:");
					foreach (XmlNode componentNode in entityNode.SelectNodes("./component") ?? throw new Exception("Prototype XML error"))
					{
						string prototypeType = ((componentNode.Attributes ?? throw new Exception("Prototype XML error"))["type"] ?? throw new Exception("Prototype XML error")).Value;
						Console.WriteLine("    " + prototypeType);
						ComponentPrototype componentPrototype = new ComponentPrototype(prototypeType);
						componentPrototypes.Add(componentPrototype);
					}
					prototype.entityList.Add(new EntityPrototype(entityName, componentPrototypes));
				}
				prototypes.Add(prototypeName, prototype);
			}
		}

		public List<Int64> SpawnPrototype(GameManager gameManager, string prototypeName)
		{
			Prototype prototype = prototypes[prototypeName];

			List<Int64> entitiesIds = new List<Int64>();

			foreach (EntityPrototype entityPrototype in prototype.entityList)
			{
				Int64 entityId = gameManager.AddEntity(entityPrototype.name);
				entitiesIds.Add(entityId);

				foreach (ComponentPrototype componentPrototype in entityPrototype.components)
				{
					Component component = gameManager.CreateComponentByTypeName(componentPrototype.type, new object[] { });
					component.entityId = entityId;

					// TODO: Set properties

					gameManager.AddComponent(component);
				}
			}

			return entitiesIds;
		}
	}
}