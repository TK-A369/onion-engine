using System.Globalization;

namespace OnionEngine.Prototypes
{
	public abstract class Prototype
	{

	}

	public class PrototypeAttribute : Attribute { }

	public class PrototypeParameterAttribute : Attribute
	{
		public bool required;
		public string? nameOverride;

		public PrototypeParameterAttribute(bool _required = false, string? _nameOverride = null)
		{
			required = _required;
			nameOverride = _nameOverride;
		}
	}

	[Prototype]
	public class TestPrototype : Prototype
	{
		[PrototypeParameter(true)]
		public int field1 = 0;
		[PrototypeParameter(true)]
		public double field2 = 0.0;
		[PrototypeParameter(true)]
		public string field3 = "";
		[PrototypeParameter(true)]
		public List<int> field4 = new List<int>();

		public override string ToString()
		{
			return "TestPrototype {" + field1 + ", " + field2 + ", \"" + field3 + "\", {" + String.Join(", ", field4) + "}}";
		}
	}

	/// <summary>
	/// Prototype describing one entity and its components.
	/// It does not describe entity systems. They are created automatically if all dependencies are satisfied.
	/// Such prototype can also inherit from other entity prototypes.
	/// The name will be overriden. Components from both parent and child prototype will be added to entity.
	/// </summary>
	[Prototype]
	public class EntityPrototype : Prototype
	{
		[PrototypeParameter(true)]
		public string name = "";
		[PrototypeParameter(true)]
		public List<ComponentDescriptor> components = new List<ComponentDescriptor>();
		[PrototypeParameter(false)]
		public List<string> inheritFrom = new List<string>();

		public EntityPrototype() { }
		public EntityPrototype(string _name)
		{
			name = _name;
		}
		public EntityPrototype(string _name, List<ComponentDescriptor> _components)
		{
			name = _name;
			components = _components;
		}
		public EntityPrototype(string _name, List<ComponentDescriptor> _components, List<string> _inheritFrom)
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
	public class ComponentDescriptor
	{
		public string type = "";
		public Dictionary<string, ComponentProperty> properties = new Dictionary<string, ComponentProperty>();

		public ComponentDescriptor(string _type)
		{
			type = _type;
		}
		public ComponentDescriptor(string _type, Dictionary<string, ComponentProperty> _properties)
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
	public class ComponentProperty
	{
		public Type type;
		public object value;

		public ComponentProperty(Type _type, object _value)
		{
			type = _type;
			value = _value;
		}
	}

	/// <summary>
	/// Prototype describing many entities.
	/// </summary>
	[Prototype]
	public class EntityGroupPrototype : Prototype
	{
		[PrototypeParameter(true)]
		public List<EntityPrototype> entityList = new List<EntityPrototype>();
	}
}