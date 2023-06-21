using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Physics
{
	/// <summary>
	/// Component representing physical body, which has velocity and mass.
	/// Entities having this should also have <c>PositionComponent</c>.
	/// </summary>
	[Component]
	sealed class PhysicalBodyComponent : Component
	{
		public Vec2<double> velocity = new Vec2<double>(0, 0);
		public double mass = 1;
	}
}