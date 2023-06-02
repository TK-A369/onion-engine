namespace OnionEngine.Prototypes
{
	public class EntityPrototype
	{
		public List<ComponentPrototype> components = new List<ComponentPrototype>();
	}
	public class ComponentPrototype
	{
		public string name = "";
		public Dictionary<string, object> properties = new Dictionary<string, object>();
	}

	public class Prototype
	{
		public List<EntityPrototype> entityList = new List<EntityPrototype>();
	}
}