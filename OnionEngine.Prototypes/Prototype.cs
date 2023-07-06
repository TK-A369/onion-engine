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
	public class EntityPrototype
	{
		public string name = "";
		public List<ComponentPrototype> components = new List<ComponentPrototype>();
		public List<string> inheritFrom = new List<string>();

		public EntityPrototype(string _name)
		{
			name = _name;
		}
		public EntityPrototype(string _name, List<ComponentPrototype> _components)
		{
			name = _name;
			components = _components;
		}
		public EntityPrototype(string _name, List<ComponentPrototype> _components, List<string> _inheritFrom)
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
		public Dictionary<string, PrototypeParameter> properties = new Dictionary<string, PrototypeParameter>();

		public ComponentPrototype(string _type)
		{
			type = _type;
		}
		public ComponentPrototype(string _type, Dictionary<string, PrototypeParameter> _properties)
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

		public object GetValue()
		{
			switch (type)
			{
				case ParameterType.Number:
					if (double.TryParse(value, CultureInfo.InvariantCulture, out double number))
						return number;
					else
						return (double)0.0;
				case ParameterType.Bool:
					if (bool.TryParse(value, out bool boolValue))
						return boolValue;
					else
						return false;
				case ParameterType.String:
					return value;
				default:
					throw new NotImplementedException();
			}
		}

		public enum ParameterType
		{
			Number, Bool, String, InternalReference
		}
	}

	/// <summary>
	/// Prototype describing many entities.
	/// </summary>
	public class EntityGroupPrototype
	{
		public List<EntityPrototype> entityList = new List<EntityPrototype>();
	}
}