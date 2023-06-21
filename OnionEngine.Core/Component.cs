namespace OnionEngine.Core
{
	/// <summary>
	/// Base class for all components.
	/// </summary>
	public abstract class Component
	{
		public Int64 entityId;
	}

	public class ComponentAttribute : Attribute { }
}