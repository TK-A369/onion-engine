using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Source of light.
	/// </summary>
	[Component]
	public sealed class LightSourceComponent : Component
	{
		public float intensity = 1.0f;

		public string lightmapTextureName = "";

		public ColorRGB lightColor = new(1, 1, 1);

		public float size = 1.0f;

		public RenderData renderData = new();
	}
}