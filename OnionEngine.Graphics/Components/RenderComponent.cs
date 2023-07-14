using OnionEngine.Core;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Component containing data about image to render.
	/// </summary>
	[Component]
	public sealed class RenderComponent : Component
	{
		public List<RenderData> renderData = new List<RenderData>();

		int counter = 0;
		public List<RenderData> GetVertices()
		{
			return renderData;
		}
	}
}