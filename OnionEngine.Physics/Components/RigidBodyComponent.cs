using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Physics
{
	/// <summary>
	/// Component representing rigid body, which has angular velocity and moment of intertia.
	/// Entities having this should also have <c>PhysicalBodyComponent</c>, <c>PositionComponent</c> and <c>RotationComponent</c>.
	/// </summary>
	[Component]
	public sealed class RigidBodyComponent : Component
	{
		public double angularVelocity = 0;
		public double momentOfInertia = 1;
	}
}