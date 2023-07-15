using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Graphics
{
	[Component]
	public class SpriteComponent : Component
	{
		public string? textureName = null;

		public Vec2<double> size = new Vec2<double>(1, 1);

		public double rotation = 0;
	}
}