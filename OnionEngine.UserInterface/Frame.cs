using OnionEngine.Graphics;
using OnionEngine.DataTypes;

namespace OnionEngine.UserInterface
{
	public class Frame : Control
	{
		public ColorRGBA borderColor = new ColorRGBA(0.5f, 0.5f, 0.5f, 0.8f);

		public int borderWidth = 2;

		public ColorRGBA backgroundColor = new ColorRGBA(0.2f, 0.2f, 0.2f, 0.5f);

		public override List<RenderData> RenderThis()
		{
			throw new NotImplementedException();

			// return new List<RenderData>()
			// {
			// 	//Background
			// 	new RenderGroup()
			// };
		}
	}
}