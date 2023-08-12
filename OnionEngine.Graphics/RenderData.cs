using OnionEngine.Core;
using OnionEngine.IoC;
using OnionEngine.Prototypes;

using OpenTK.Graphics.OpenGL4;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Struct storing some data to render.
	/// It has list of vertices, and the order to draw them.
	/// It belongs to given render group.
	/// </summary>
	public struct RenderData
	{
		public List<float> vertices;
		public List<int> indices;
		public string renderGroup;
		public string? textureAtlasName;
	}
}