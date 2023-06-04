namespace OnionEngine.Core
{
	public abstract class EntitySystem
	{
		public Int64 entityId;

		public virtual void OnCreate() { }
		public virtual void OnDestroy() { }
	}

	public class EntitySystemAttribute : Attribute { }

	public class EntitySystemDependencyAttribute : Attribute { }
}