using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Source of light.
	/// </summary>
	[Component]
	public sealed class ShadowCasterComponent : Component
	{
		public float transparency = 0.0f;

		public ColorRGB color = new(1, 1, 1);

		public List<Vec2<double>> shadowsEndpoints = new();
	}
}