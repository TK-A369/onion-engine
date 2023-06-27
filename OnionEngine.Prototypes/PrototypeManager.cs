using System.Xml;

using OnionEngine.Core;
using OnionEngine.IoC;

namespace OnionEngine.Prototypes
{
	/// <summary>
	/// Class managing prototypes of entities.
	/// </summary>
	public class PrototypeManager
	{
		public PrototypeManager()
		{
			IoCManager.RegisterInstance(this);
		}

		[Dependency]
		private GameManager gameManager = default!;

		/// <summary>
		/// Dictionary of entity group prototypes by their names.
		/// </summary>
		public Dictionary<string, EntityGroupPrototype> entityGroupPrototypes = new Dictionary<string, EntityGroupPrototype>();

		/// <summary>
		/// Dictionary of entity prototypes by their names.
		/// </summary>
		public Dictionary<string, EntityPrototype> entityPrototypes = new Dictionary<string, EntityPrototype>();

		/// <summary>
		/// Load prototype from XML.
		/// </summary>
		/// <param name="xmlPrototype"><c>string</c> containing XML describing the prototype.</param>
		/// <exception cref="Exception"></exception>
		public void LoadPrototypes(string xmlPrototype)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlPrototype);

			// Entity prototypes
			foreach (XmlNode prototypeNode in doc.SelectNodes("/protos/entityproto") ?? throw new Exception("Prototype XML error"))
			{
				//TODO
			}

			// Entity group prototypes
			foreach (XmlNode prototypeNode in doc.SelectNodes("/protos/entitygroupproto") ?? throw new Exception("Prototype XML error"))
			{
				EntityGroupPrototype prototype = new EntityGroupPrototype();
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
				entityGroupPrototypes.Add(prototypeName, prototype);
			}
		}

		public Int64 SpawnEntityPrototype(string prototypeName)
		{
			EntityPrototype prototype = entityPrototypes[prototypeName];

			// Create entity
			Int64 entityId = gameManager.AddEntity(prototype.name);

			// Add components to it
			void AddComponents(EntityPrototype entityPrototype, Int64 entityId)
			{
				foreach (ComponentPrototype componentPrototype in entityPrototype.components)
				{
					Component component = gameManager.CreateComponentByTypeName(componentPrototype.type, new object[] { });
					if (!gameManager.HasComponent(entityId, component.GetType()))
					{
						component.entityId = entityId;
						gameManager.AddComponent(component);
					}

					// TODO: Assign values to component's fields
				}

				// Recursively call for parent prototypes
				foreach (string parent in entityPrototype.inheritFrom)
				{
					AddComponents(entityPrototypes[parent], entityId);
				}
			}
			AddComponents(prototype, entityId);

			return entityId;
		}

		/// <summary>
		/// Spawn prototype.
		/// </summary>
		/// <param name="gameManager"><c>GameManager</c> to spawn the prototype in</param>
		/// <param name="prototypeName">Name of prototype to spawn</param>
		/// <returns></returns>
		public List<Int64> SpawnEntityGroupPrototype(string prototypeName)
		{
			EntityGroupPrototype prototype = entityGroupPrototypes[prototypeName];

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