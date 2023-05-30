using OnionEngine.Core;

namespace OnionEngine.Graphics
{
	sealed class RenderComponent : Component
	{
		public VerticesIndices GetVertices(int start_index)
		{
			List<float> vertices = new List<float>()
			{
				0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
				0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
				-0.5f, -0.5f, 0.0f, 1.0f, 1.0f, 0.0f,
				-0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f
			};
			List<int> indices = new List<int>()
			{
				start_index,
				start_index + 1,
				start_index + 2,
				start_index + 3
			};

			return new VerticesIndices { vertices = vertices, indices = indices };
		}
	}
}