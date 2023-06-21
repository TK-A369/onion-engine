namespace OnionEngine.Prototypes
{
	/// <summary>
	/// Prototype describing one entity and its components.
	/// It does not describe entity systems. They are created automatically if all dependencies are satisfied.
	/// Such prototype can also inherit from other entity prototypes.
	/// The name will be overriden. Components from both parent and child prototype will be added to entity.
	/// </summary>
	public class EntityPrototype
	{
		public string name = "";
		public List<ComponentPrototype> components = new List<ComponentPrototype>();
		public List<EntityPrototype> inheritFrom = new List<EntityPrototype>();

		public EntityPrototype(string _name)
		{
			name = _name;
		}
		public EntityPrototype(string _name, List<ComponentPrototype> _components)
		{
			name = _name;
			components = _components;
		}
		public EntityPrototype(string _name, List<ComponentPrototype> _components, List<EntityPrototype> _inheritFrom)
		{
			name = _name;
			components = _components;
			inheritFrom = _inheritFrom;
		}
	}


	/// <summary>
	/// Prototype describing one component.
	/// It has its type and properties.
	/// As for now, component prototypes don't support inheritance.
	/// </summary>
	public class ComponentPrototype
	{
		public string type = "";
		public Dictionary<string, object> properties = new Dictionary<string, object>();

		public ComponentPrototype(string _type)
		{
			type = _type;
		}
		public ComponentPrototype(string _type, Dictionary<string, object> _properties)
		{
			type = _type;
			properties = _properties;
		}
	}

	/// <summary>
	/// Class describing prototype parameter.
	/// It can have following types: number, bool, string or internal reference.
	/// Internal reference means reference to entity or component inside this prototype. Currently WIP.
	/// </summary>
	public class PrototypeParameter
	{
		ParameterType type;
		string value;

		public PrototypeParameter(ParameterType _type, string _value)
		{
			type = _type;
			value = _value;
		}

		public enum ParameterType
		{
			Number, Bool, String, InternalReference
		}
	}

	/// <summary>
	/// Prototype describing many entities.
	/// </summary>
	public class Prototype
	{
		public List<EntityPrototype> entityList = new List<EntityPrototype>();
	}
}