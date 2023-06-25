using OnionEngine.Graphics;

namespace OnionEngine.UserInterface
{
	public class RootControl : Control
	{
		public override UIDim2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public override List<RenderData> RenderThis()
		{
			return new List<RenderData>() { };
		}
	}
}