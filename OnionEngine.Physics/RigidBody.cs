using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Physics
{
	[Component]
	sealed class RigidBodyComponent : Component
	{
		public double angularVelocity = 0;
		public double momentOfInertia = 1;
	}
}