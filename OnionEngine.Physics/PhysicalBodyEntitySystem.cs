using OnionEngine.Core;

namespace OnionEngine.Physics
{
	[EntitySystem]
	class PhysicalBodyEntitySystem : EntitySystem
	{
		[EntitySystemDependency]
		public PositionComponent positionComponent = default!;

		[EntitySystemDependency]
		public RigidBodyComponent rigidBodyComponent = default!;
	}
}