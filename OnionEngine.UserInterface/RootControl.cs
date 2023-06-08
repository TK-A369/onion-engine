using OnionEngine.Graphics;

namespace OnionEngine.UserInterface
{
	class RootControl : Control
	{
		public override UIDim2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public override List<RenderData> Render()
		{
			throw new NotImplementedException();
		}
	}
}