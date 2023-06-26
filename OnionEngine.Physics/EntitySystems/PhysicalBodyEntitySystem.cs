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
		PositionComponent positionComponent = default!;

		[EntitySystemDependency]
		RigidBodyComponent rigidBodyComponent = default!;
	}
}