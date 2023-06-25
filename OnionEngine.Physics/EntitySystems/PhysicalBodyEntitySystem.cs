using OnionEngine.Core;

namespace OnionEngine.Physics
{
	/// <summary>
	/// Entity system that moves physical bodies.
	/// </summary>
	[EntitySystem]
	public class PhysicalBodyEntitySystem : EntitySystem
	{
		[EntitySystemDependency]
		public PositionComponent positionComponent = default!;

		[EntitySystemDependency]
		public RigidBodyComponent rigidBodyComponent = default!;
	}
}