namespace OnionEngine.Prototypes
{
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

	public class Prototype
	{
		public List<EntityPrototype> entityList = new List<EntityPrototype>();
	}
}