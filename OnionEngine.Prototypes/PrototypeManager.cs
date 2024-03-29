using OnionEngine.Core;
using OnionEngine.IoC;

using System.Reflection;
using System.Xml;
using System.Text.Json;
using System.Globalization;

namespace OnionEngine.Prototypes
{
	/// <summary>
	/// Class managing prototypes of entities.
	/// </summary>
	public class PrototypeManager
	{
		/// <summary>
		/// Dictionary of entity group prototypes by their names.
		/// </summary>
		public Dictionary<string, EntityGroupPrototype> entityGroupPrototypes = new();

		public Dictionary<string, Prototype> prototypes = new();

		private Dictionary<Type, Dictionary<string, Prototype>> prototypesByType = new();

		private Dictionary<string, Type> prototypeTypes = new();

		public List<Func<JsonElement, Type, object?>> jsonParsers;

		[Dependency]
		private GameManager gameManager = default!;

		public PrototypeManager()
		{
			IoCManager.RegisterInstance(this);

			jsonParsers = new List<Func<JsonElement, Type, object?>>()
			{
				(JsonElement e, Type t) => {
					if(t == typeof(int))
						return e.GetInt32();
					return null;
				},
				(JsonElement e, Type t) => {
					if(t == typeof(float))
						return e.GetSingle();
					return null;
				},
				(JsonElement e, Type t) => {
					if(t == typeof(double))
						return e.GetDouble();
					return null;
				},
				(JsonElement e, Type t) => {
					if(t == typeof(string))
						return e.GetString();
					return null;
				},
				(JsonElement e, Type t) => {
					if(t == typeof(bool))
						return e.GetBoolean();
					return null;
				},
				(JsonElement e, Type t) => {
					if (t.IsGenericType)
					{
						Type genericTypeDefinition = t.GetGenericTypeDefinition();
						if (genericTypeDefinition == typeof(List<>))
						{
							if (e.ValueKind == JsonValueKind.Array)
							{
								object list = Activator.CreateInstance(t) ?? throw new Exception("Couldn't create instance of desired type");
								foreach (JsonElement child in e.EnumerateArray())
								{
									object? childParsed = ParseJSONParam(child, t.GetGenericArguments()[0]);
									if (childParsed != null)
										(t.GetMethod("Add") ?? throw new Exception("Couldn't add element to list via reflection")).Invoke(list, new object[] { childParsed });
									else
										return null;
								}
								return list;
							}
						}
					}
					return null;
				},
				(JsonElement e, Type t) => {
					if (t.IsGenericType)
					{
						Type genericTypeDefinition = t.GetGenericTypeDefinition();
						if (genericTypeDefinition == typeof(Dictionary<,>))
						{
							if (e.ValueKind == JsonValueKind.Object && t.GetGenericArguments()[0]==typeof(string))
							{
								object list = Activator.CreateInstance(t) ?? throw new Exception("Couldn't create instance of desired type");
								foreach (JsonProperty keyValuePair in e.EnumerateObject())
								{
									object? valueParsed = ParseJSONParam(keyValuePair.Value, t.GetGenericArguments()[1]);
									if (valueParsed != null)
										(t.GetMethod("Add") ?? throw new Exception("Couldn't add element to list via reflection")).Invoke(list, new object[] { keyValuePair.Name, valueParsed });
									else
										return null;
								}
								return list;
							}
						}
					}
					return null;
				},
				(JsonElement e, Type t) => {
					if (t == typeof(ComponentDescriptor))
					{
						string componentTypeName = e.GetProperty("type").GetString() ?? throw new Exception("Component type not provided in JSON");
						Type componentType = gameManager.GetComponentTypeByName(componentTypeName);

						Dictionary<string, ComponentProperty> parameters = new();
						foreach(JsonProperty keyValuePair in e.EnumerateObject()) {
							if(keyValuePair.Name != "type")
							{
								FieldInfo componentFieldInfo = componentType.GetField(keyValuePair.Name) ?? throw new Exception("Couldn't find field " + keyValuePair.Name + " in component type");
								Type fieldType = componentFieldInfo.FieldType;
								parameters.Add(keyValuePair.Name, new ComponentProperty(
											fieldType,
											ParseJSONParam(keyValuePair.Value, fieldType) ?? throw new Exception("Couldn't parse value for field " + keyValuePair.Name + " of type " + fieldType)));
							}
						}
						ComponentDescriptor componentDescriptor = new(componentTypeName, parameters);
						return componentDescriptor;
					}
					return null;
				},
				(JsonElement e, Type t) => {
					if(t.IsDefined(typeof(PrototypeJSONAutoparseAttribute))) {
						object result = IoCManager.CreateInstance(t, new object[]{});
						foreach(FieldInfo fieldInfo in t.GetFields()) {
							if(fieldInfo.IsDefined(typeof(PrototypeJSONAutoparsedFieldAttribute))) {
								PrototypeJSONAutoparsedFieldAttribute fieldAttribute = fieldInfo.GetCustomAttribute<PrototypeJSONAutoparsedFieldAttribute>()!;
								string propertyName = fieldAttribute.nameOverride ?? fieldInfo.Name;
								if(e.TryGetProperty(propertyName, out JsonElement propertyValue)) {
									fieldInfo.SetValue(result, ParseJSONParam(propertyValue, fieldInfo.FieldType));
								}
							}
						}
						return result;
					}
					return null;
				},
				(JsonElement e, Type t) => {
					if(t.IsEnum) {
						return Enum.Parse(t, e.ToString());
					}
					return null;
				}
			};
		}

		public void RegisterPrototypeType(Type prototypeType)
		{
			if (prototypeType.IsAssignableTo(typeof(Prototype)))
			{
				prototypeTypes.Add(prototypeType.Name, prototypeType);
			}
			else
			{
				throw new Exception("Type " + prototypeType.FullName + " is not a prototype type.");
			}
		}

		public void AutoregisterPrototypeTypes()
		{
			IEnumerable<Type> prototypeTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
											   from type in assembly.GetTypes()
											   where type.IsDefined(typeof(PrototypeAttribute))
											   select type;

			foreach (Type prototypeType in prototypeTypes)
				RegisterPrototypeType(prototypeType);
		}

		public object? ParseJSONParam(JsonElement json, Type desiredType)
		{
			foreach (Func<JsonElement, Type, object?> parser in jsonParsers)
			{
				object? parsedValue = parser(json, desiredType);
				if (parsedValue != null)
					return parsedValue;
			}
			return null;
		}

		public void LoadPrototypes(string jsonPrototypes)
		{
			JsonDocument document = JsonDocument.Parse(jsonPrototypes);
			foreach (JsonProperty prototypeDescriptorPair in document.RootElement.EnumerateObject())
			{
				JsonElement prototypeDescriptor = prototypeDescriptorPair.Value;
				string prototypeName = prototypeDescriptorPair.Name;

				// Determine prototype type
				string prototypeTypeName = prototypeDescriptor.GetProperty("type").GetString() ?? throw new Exception("Prototype JSON error");
				Type prototypeType = (prototypeTypes.ContainsKey(prototypeTypeName) ? prototypeTypes[prototypeTypeName] : prototypeTypes[prototypeTypeName + "Prototype"]) ?? throw new Exception("Prototype type not found");

				// Create instance
				Prototype prototypeInstance = (Prototype)(Activator.CreateInstance(prototypeType) ?? throw new Exception("Error ocurred when creating prototype instance"));

				// Set fields
				foreach (FieldInfo fieldInfo in prototypeType.GetFields())
				{
					PrototypeParameterAttribute? paramAttribute = fieldInfo.GetCustomAttribute<PrototypeParameterAttribute>(true);
					if (paramAttribute != null)
					{
						string propertyName = paramAttribute.nameOverride ?? fieldInfo.Name;
						if (prototypeDescriptor.TryGetProperty(propertyName, out JsonElement propertyValue))
						{
							fieldInfo.SetValue(prototypeInstance, ParseJSONParam(propertyValue, fieldInfo.FieldType) ?? throw new Exception("Couldn't parse value of property " + propertyName + " of type " + fieldInfo.FieldType.Name));
						}
						else
						{
							if (paramAttribute.required)
								throw new Exception("Required property " + propertyName + " not found in prototype of type " + prototypeTypeName);
						}
					}
				}

				prototypes.Add(prototypeName, prototypeInstance);
				if (!prototypesByType.ContainsKey(prototypeType))
				{
					prototypesByType.Add(prototypeType, new Dictionary<string, Prototype>());
				}
				prototypesByType[prototypeType].Add(prototypeName, prototypeInstance);

				if (GameManager.debugMode)
					Console.Write("Loaded prototype:\n" + prototypeInstance.ToString() + "\n");
			}
		}

		/// <summary>
		/// Load all prototype files (JSON) in directory with specified relative path, including subdirectories.
		/// </summary>
		/// <param name="relativeDirectoryPath">Relative path to directory containing prototype files.</param>
		public void LoadPrototypesFromDirectory(string relativeDirectoryPath)
		{
			string execPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Couldn't determine directory in which executable is");
			string fullPath = System.IO.Path.Combine(execPath, relativeDirectoryPath);

			foreach (string file in Directory.EnumerateFiles(fullPath, "*.json", SearchOption.AllDirectories))
			{
				if (GameManager.debugMode)
					Console.WriteLine("Loading prototypes file " + file);

				LoadPrototypes(File.ReadAllText(file));
			}
		}

		/// <summary>
		/// Spawn entity prototype.
		/// </summary>
		/// <param name="prototypeName">Name of prototype to spawn</param>
		/// <returns>Entity id</returns>
		/// <exception cref="Exception"></exception>
		/// <remarks>Should be moved into more appropriate place</remarks>
		public Int64 SpawnEntityPrototype(string prototypeName)
		{
			EntityPrototype prototype = (EntityPrototype)prototypesByType[typeof(EntityPrototype)][prototypeName];

			// Create entity
			Int64 entityId = gameManager.AddEntity(prototype.name);

			// Add components to it
			void AddComponents(EntityPrototype entityPrototype, Int64 entityId)
			{
				foreach (ComponentDescriptor componentPrototype in entityPrototype.components)
				{
					Component component = gameManager.CreateComponentByTypeName(componentPrototype.type, new object[] { });
					Type componentType = component.GetType();

					// Assign values to component's fields
					foreach (KeyValuePair<string, ComponentProperty> property in componentPrototype.properties)
					{
						FieldInfo fieldInfo = componentType.GetField(property.Key) ?? throw new Exception("Field " + property.Key + " not found in component type " + componentType.Name);
						fieldInfo.SetValue(component, property.Value.value);
					}

					// Add component to entity
					if (!gameManager.HasComponent(entityId, componentType))
					{
						component.entityId = entityId;
						gameManager.AddComponent(component);
					}
				}

				// Recursively call for parent prototypes
				foreach (string parent in entityPrototype.inheritFrom)
				{
					AddComponents((EntityPrototype)prototypesByType[typeof(EntityPrototype)][parent], entityId);
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
		/// <remarks>Doesn't fully work now. Should be moved into more appropriate place.</remarks>
		public List<Int64> SpawnEntityGroupPrototype(string prototypeName)
		{
			EntityGroupPrototype prototype = entityGroupPrototypes[prototypeName];

			List<Int64> entitiesIds = new();

			// TODO: Remake, so it uses SpawnEntityPrototype

			foreach (EntityPrototype entityPrototype in prototype.entityList)
			{
				Int64 entityId = gameManager.AddEntity(entityPrototype.name);
				entitiesIds.Add(entityId);

				foreach (ComponentDescriptor componentPrototype in entityPrototype.components)
				{
					Component component = gameManager.CreateComponentByTypeName(componentPrototype.type, new object[] { });
					component.entityId = entityId;

					// TODO: Set properties

					gameManager.AddComponent(component);
				}
			}

			return entitiesIds;
		}

		public Dictionary<string, Prototype> GetPrototypesOfType(Type type)
			=> prototypesByType.ContainsKey(type) ? prototypesByType[type] : new Dictionary<string, Prototype>();

		public Dictionary<string, T> GetPrototypesOfType<T>() where T : Prototype
		{
			Dictionary<string, Prototype> prototypesOfThatType = GetPrototypesOfType(typeof(T));
			Dictionary<string, T> result = new();
			foreach (var (key, value) in prototypesOfThatType)
			{
				result.Add(key, (T)value);
			}
			return result;
		}
	}
}