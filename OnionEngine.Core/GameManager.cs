using System.Reflection;
using System.Collections.Immutable;

using OnionEngine.Prototypes;

namespace OnionEngine.Core
{
	/// <summary>
	/// Instance of this manages the game.
	/// Is allows creation and removal of entites and components.
	/// It automatically creates and destroys entity systems when appropriate.
	/// </summary>
	public sealed class GameManager
	{
		/// <summary>
		/// The id that will be given for next entity 
		/// </summary>
		Int64 nextEntityId = 0;

		/// <summary>
		/// The id that will be given for next component
		/// </summary>
		Int64 nextComponentId = 0;

		public static bool debugMode = false;

		/// <summary>
		/// Set of entities
		/// </summary>
		public HashSet<Entity> entities = new HashSet<Entity>();

		/// <summary>
		/// Dictionary of components by their id
		/// </summary>
		public Dictionary<Int64, Component> components = new Dictionary<Int64, Component>();

		/// <summary>
		/// Dictionary of parent entities of given components by their id
		/// </summary>
		Dictionary<Int64, Int64> entitiesByComponent = new Dictionary<Int64, Int64>();

		/// <summary>
		/// Dictionary of set of components belonging to given entities by their id
		/// </summary>
		Dictionary<Int64, HashSet<Int64>> componentsByEntity = new Dictionary<Int64, HashSet<Int64>>();

		/// <summary>
		/// Dictionary of set of entities by set of owned components
		/// </summary>
		Dictionary<HashSet<Type>, HashSet<Int64>> entitiesByOwnedComponentsCache = new Dictionary<HashSet<Type>, HashSet<Int64>>();

		/// <summary>
		/// Dictionary of registered entity systems and their dependencies set
		/// </summary>
		Dictionary<Type, Dictionary<Type, string>> registeredEntitySystems = new Dictionary<Type, Dictionary<Type, string>>();

		/// <summary>
		/// Dictionary of entity systems by their type and parent entity
		/// </summary>
		Dictionary<Int64, Dictionary<Type, EntitySystem>> entitySystemsByParent = new Dictionary<Int64, Dictionary<Type, EntitySystem>>();

		Dictionary<string, Type> componentTypesByName = new Dictionary<string, Type>();

		/// <summary>
		/// Prototype manager
		/// </summary>
		public PrototypeManager prototypeManager = new PrototypeManager();

		public GameManager()
		{
		}

		/// <summary>
		/// Create new entity.
		/// </summary>
		/// <param name="name">New entity name</param>
		/// <returns>New entity id</returns>
		public Int64 AddEntity(string name)
		{
			Int64 entityId = nextEntityId++;
			entities.Add(new Entity() { entityId = entityId, name = name });
			componentsByEntity.Add(entityId, new HashSet<Int64>());
			entitySystemsByParent.Add(entityId, new Dictionary<Type, EntitySystem>());

			return entityId;
		}

		/// <summary>
		/// Register component type.
		/// After that, it will be able to be used.
		/// </summary>
		/// <param name="type"><c>Type</c> object representing component type</param>
		/// <exception cref="ArgumentException"></exception>
		public void RegisterComponentType(Type type)
		{
			if (!type.IsAssignableTo(typeof(Component)))
				throw new ArgumentException("Type must be inherit from Component");

			componentTypesByName.Add(type.Name, type);

			if (debugMode)
				Console.WriteLine("Registered component type " + type.Name);
		}

		/// <summary>
		/// Register all types having ComponentAttribute as components
		/// </summary>
		public void AutoRegisterComponentTypes()
		{
			IEnumerable<Type> componentTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
											   from type in assembly.GetTypes()
											   where type.IsDefined(typeof(ComponentAttribute))
											   select type;

			foreach (Type componentType in componentTypes)
				RegisterComponentType(componentType);
		}

		/// <summary>
		/// Create new <c>Component</c> object.
		/// </summary>
		/// <param name="typeName">Name of component type</param>
		/// <param name="args">Arguments passed to constructor</param>
		/// <returns>Newly created <c>Component</c> object</returns>
		/// <exception cref="Exception"></exception>
		public Component CreateComponentByTypeName(string typeName, object[] args)
		{
			Type componentType = componentTypesByName[typeName];
			Component component = Activator.CreateInstance(componentType, args) as Component ?? throw new Exception("Provided type name must inherit from Component");

			return component;
		}

		/// <summary>
		/// Add existing <c>Component</c> object to entity with id <c>component.entityId</c>.
		/// If after adding component to that entity, it has all the components required by some entity system, instance of this system will be created for that entity.
		/// </summary>
		/// <param name="component">Component to add</param>
		/// <returns>Id of this component</returns>
		/// <exception cref="Exception"></exception>
		/// <exception cref="NullReferenceException"></exception>
		public Int64 AddComponent(Component component)
		{
			Int64 componentId = nextComponentId++;
			components.Add(componentId, component);

			entitiesByComponent.Add(componentId, component.entityId);
			componentsByEntity[component.entityId].Add(componentId);

			// Consider improving it - recompute only affected queries
			entitiesByOwnedComponentsCache.Clear();

			// List all components (by type) currently owned by this entity
			Dictionary<Type, Int64> ownedComponents = new Dictionary<Type, Int64>();
			if (debugMode)
				Console.WriteLine("Components owned by entity " + component.entityId + ":");
			foreach (Int64 componentId2 in componentsByEntity[component.entityId])
			{
				ownedComponents.Add(components[componentId2].GetType(), componentId2);
				if (debugMode)
					Console.WriteLine(componentId2 + " " + components[componentId2].GetType().Name);
			}

			// Check if this entity should get new entity system
			foreach (KeyValuePair<Type, Dictionary<Type, string>> entitySystemPair in registeredEntitySystems)
			{
				// If all components needed by system are owned by this entity, and such system hasn't been created yet
				if (!entitySystemsByParent[component.entityId].ContainsKey(entitySystemPair.Key))
				{
					if (entitySystemPair.Value.Keys.ToImmutableHashSet().IsSubsetOf(ownedComponents.Keys))
					{
						if (debugMode)
							Console.WriteLine("Creating new instance of entity system " + entitySystemPair.Key.Name);
						// Create instance of entity system
						EntitySystem entitySystem = Activator.CreateInstance(entitySystemPair.Key) as EntitySystem ?? throw new Exception("Bad EntitySystem registered: " + entitySystemPair.Key);
						foreach (KeyValuePair<Type, string> dependencyPair in entitySystemPair.Value)
						{
							(entitySystemPair.Key.GetField(dependencyPair.Value) ?? throw new NullReferenceException()).SetValue(entitySystem, components[ownedComponents[dependencyPair.Key]]);
						}
						entitySystemsByParent[component.entityId].Add(entitySystemPair.Key, entitySystem);
						entitySystem.OnCreate();
					}
				}
			}

			return componentId;
		}

		/// <summary>
		/// Remove component.
		/// If this component was required by some entity system, this instance of system will be destroyed.
		/// </summary>
		/// <param name="componentId">Id of component to remove</param>
		public void RemoveComponent(Int64 componentId)
		{
			if (debugMode)
				Console.WriteLine("Removing component " + componentId);

			// Remove entity systems which cannot live longer due to removed component
			foreach (KeyValuePair<Type, EntitySystem> entitySystemPair in entitySystemsByParent[components[componentId].entityId])
			{
				Type entitySystemType = entitySystemPair.Key;
				// If this entity system depends on this component
				if (registeredEntitySystems[entitySystemType].Keys.Contains(components[componentId].GetType()))
				{
					if (debugMode)
						Console.WriteLine("Destroying entity system of type " + entitySystemType.Name + " on entity " + components[componentId].entityId);
					entitySystemPair.Value.OnDestroy();
					entitySystemsByParent[components[componentId].entityId].Remove(entitySystemType);
				}
			}

			// Remove component
			entitiesByComponent.Remove(componentId);
			componentsByEntity[components[componentId].entityId].Remove(componentId);

			entitiesByOwnedComponentsCache.Clear();

			components.Remove(componentId);
		}

		/// <summary>
		/// Get <c>Component</c> of given type owned by given entity.
		/// </summary>
		/// <param name="entityId">Id of entity owning component you're looking for</param>
		/// <param name="componentType"><c>Type</c> object representing type of component you're looking for</param>
		/// <returns>Id of component found</returns>
		/// <exception cref="Exception"></exception>
		public Int64 GetComponent(Int64 entityId, Type componentType)
		{
			foreach (Int64 componentId in componentsByEntity[entityId])
			{
				if (components[componentId].GetType().IsAssignableTo(componentType))
				{
					return componentId;
				}
			}
			throw new Exception("Component not found");
		}

		/// <summary>
		/// Get basic informations about currently existing entities and their components
		/// </summary>
		/// <returns><c>string</c> containing those informations</returns>
		public string DumpEntitiesAndComponents()
		{
			string result = "";

			foreach (Entity entity in entities)
			{
				result += "Entity " + entity.entityId + " \"" + entity.name + "\":\n";
				foreach (Int64 componentId in componentsByEntity[entity.entityId])
				{
					result += "  Component " + componentId + " " + components[componentId].GetType().Name + "\n";
				}
			}

			return result;
		}

		/// <summary>
		/// Get all entities that own all given components.
		/// </summary>
		/// <param name="componentTypes">Set of types of components</param>
		/// <returns>Set of entities matching query</returns>
		public HashSet<Int64> QueryEntitiesOwningComponents(HashSet<Type> componentTypes)
		{
			if (entitiesByOwnedComponentsCache.ContainsKey(componentTypes))
			{
				return entitiesByOwnedComponentsCache[componentTypes];
			}

			if (componentTypes.Count == 0)
			{
				HashSet<Int64> result = new HashSet<Int64>();
				foreach (Entity entity in entities)
				{
					result.Add(entity.entityId);
				}
				return result;
			}
			else if (componentTypes.Count == 1)
			{
				HashSet<Int64> result = new HashSet<Int64>();
				foreach (KeyValuePair<Int64, Int64> keyValuePair in entitiesByComponent)
				{
					if (components[keyValuePair.Key].GetType().IsAssignableTo(componentTypes.First()))
						result.Add(keyValuePair.Value);
				}
				return result;
			}
			else
			{
				Int64 half = componentTypes.Count / 2;
				Int64 i = 0;
				HashSet<Type> firstHalf = new HashSet<Type>();
				HashSet<Type> secondHalf = new HashSet<Type>();
				foreach (Type type in componentTypes)
				{
					if (i < half)
						firstHalf.Add(type);
					else
						secondHalf.Add(type);
					i++;
				}

				HashSet<Int64> firstHalfResult = QueryEntitiesOwningComponents(firstHalf);
				entitiesByOwnedComponentsCache.Add(firstHalf, firstHalfResult);
				HashSet<Int64> secondHalfResult = QueryEntitiesOwningComponents(secondHalf);
				entitiesByOwnedComponentsCache.Add(secondHalf, secondHalfResult);
				firstHalfResult.IntersectWith(secondHalfResult);
				return firstHalfResult;
			}
		}

		/// <summary>
		/// Register entity system type.
		/// After doing so, it will automatically be created for entities having all required components.
		/// </summary>
		/// <param name="entitySystemType"><c>Type</c> object representing the entity system type</param>
		/// <exception cref="ArgumentException"></exception>
		public void RegisterEntitySystemType(Type entitySystemType)
		{
			if (!entitySystemType.IsAssignableTo(typeof(EntitySystem)))
				throw new ArgumentException("You must provide Type object representing subclass of EntitySystem");

			Dictionary<Type, string> dependencies = new Dictionary<Type, string>();
			foreach (FieldInfo fieldInfo in entitySystemType.GetFields())
			{
				if (fieldInfo.IsDefined(typeof(EntitySystemDependencyAttribute)))
				{
					dependencies.Add(fieldInfo.FieldType, fieldInfo.Name);
				}
			}

			registeredEntitySystems.Add(entitySystemType, dependencies);

			if (debugMode)
				Console.WriteLine("Registered entity system type " + entitySystemType.Name);
		}

		/// <summary>
		/// Register all types having EntitySystemAttribute as entity system types.
		/// </summary>
		public void AutoRegisterEntitySystemTypes()
		{
			IEnumerable<Type> entitySystemTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
												  from type in assembly.GetTypes()
												  where type.IsDefined(typeof(EntitySystemAttribute))
												  select type;

			foreach (Type entitySystemType in entitySystemTypes)
				RegisterEntitySystemType(entitySystemType);
		}
	}
}