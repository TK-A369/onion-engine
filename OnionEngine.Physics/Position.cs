using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Physics
{
	[Component]
	sealed class PositionComponent : Component
	{
		public Vec2<double> position = new Vec2<double>(0, 0);
	}
}