using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Physics
{
	[Component]
	sealed class PhysicalBodyComponent : Component
	{
		public Vec2<double> velocity = new Vec2<double>(0, 0);
		public double mass = 1;
	}
}