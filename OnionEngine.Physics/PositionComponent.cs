using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Physics
{
	/// <summary>
	/// Component storing position of object.
	/// </summary>
	[Component]
	sealed class PositionComponent : Component
	{
		public Vec2<double> position = new Vec2<double>(0, 0);
	}
}