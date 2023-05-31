namespace OnionEngine.Core
{
	sealed class GameManager
	{
		// The id that will be given for next entity
		Int64 nextEntityId = 0;

		// The id that will be given for next component
		Int64 nextComponentId = 0;

		public bool debugMode = false;

		// Set of entities
		public HashSet<Entity> entities = new HashSet<Entity>();

		// Dictionary of components by their id
		public Dictionary<Int64, Component> components = new Dictionary<Int64, Component>();

		// Dictionary of parent entities of given components by their id
		Dictionary<Int64, Int64> entitiesByComponent = new Dictionary<Int64, Int64>();

		// Dictionary of set of components belonging to given entities by their id
		Dictionary<Int64, HashSet<Int64>> componentsByEntity = new Dictionary<Int64, HashSet<Int64>>();

		// Dictionary of set of entities by set of owned components
		Dictionary<HashSet<Type>, HashSet<Int64>> entitiesByOwnedComponentsCache = new Dictionary<HashSet<Type>, HashSet<Int64>>();

		public GameManager()
		{
		}

		public Int64 AddEntity(string name)
		{
			Int64 entityId = nextEntityId++;
			entities.Add(new Entity() { entityId = nextEntityId, name = name });
			componentsByEntity.Add(entityId, new HashSet<Int64>());

			return entityId;
		}

		public Int64 AddComponent(Component component)
		{
			Int64 componentId = nextComponentId++;
			components.Add(componentId, component);

			entitiesByComponent.Add(componentId, component.entityId);
			componentsByEntity[component.entityId].Add(componentId);

			// Consider improving it - recompute only affected queries
			entitiesByOwnedComponentsCache.Clear();

			return componentId;
		}

		public void RemoveComponent(Int64 componentId)
		{
			entitiesByComponent.Remove(componentId);
			componentsByEntity[components[componentId].entityId].Remove(componentId);

			entitiesByOwnedComponentsCache.Clear();

			components.Remove(componentId);
		}

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

				HashSet<Int64> result = QueryEntitiesOwningComponents(firstHalf);
				result.IntersectWith(QueryEntitiesOwningComponents(secondHalf));
				return result;
			}
		}

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
	}
}