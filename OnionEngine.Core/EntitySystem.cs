namespace OnionEngine.Core
{
	/// <summary>
	/// Base class for all entity systems.
	/// When entity has all the dependencies (specified with <see cref="EntitySystemDependencyAttribute" /> attribute)
	/// </summary>
	public abstract class EntitySystem
	{
		public Int64 entityId;

		/// <summary>
		/// Called when the entity system is initialized, after all dependency fields has been assigned.
		/// </summary>
		public virtual void OnCreate() { }

		/// <summary>
		/// Called before the entity system is destroyed.
		/// </summary>
		public virtual void OnDestroy() { }
	}

	/// <summary>
	/// Attribute used to mark classes to be automatically registered as entity system types.
	/// </summary>
	public class EntitySystemAttribute : Attribute { }

	/// <summary>
	/// Attribute used to mark fields in entity systems that are their dependencies. It means that this entity system requires the component of the type of that field.
	/// It might be required or optional.
	/// Entity system must have at least one required dependency field.
	/// When some entity has all components this entity system depends on, the instance of this entity system will be created.
	/// </summary>
	public class EntitySystemDependencyAttribute : Attribute
	{
		public bool required = true;

		public EntitySystemDependencyAttribute() { }

		public EntitySystemDependencyAttribute(bool _required)
		{
			required = _required;
		}
	}
}