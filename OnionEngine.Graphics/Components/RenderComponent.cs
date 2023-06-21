using OnionEngine.Core;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Component containing data about image to render.
	/// </summary>
	[Component]
	public sealed class RenderComponent : Component
	{
		int counter = 0;
		public List<RenderData> GetVertices()
		{
			List<float> vertices = new List<float>()
			{
				// x        y      z      r     g     b
                  -0.9f,   -0.9f,  0.0f,  1.0f, 0.0f, 0.0f,
				  -0.7f,   -0.9f,  0.0f,  1.0f, 1.0f, 0.0f,
				  -0.8f,   -0.8f,  0.0f,  0.0f, 0.0f, 1.0f,
				  -0.7f,   -0.8f + counter * 0.001f,  0.0f,  1.0f, 1.0f, 1.0f,
			};
			List<int> indices = new List<int>()
			{
				0, 1, 2,
				1, 2, 3
			};
			counter++;

			return new List<RenderData>() { new RenderData { vertices = vertices, indices = indices, renderGroup = "basic-group" } };
		}
	}
}