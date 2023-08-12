using OnionEngine.DataTypes;
using OnionEngine.Graphics;
using OnionEngine.IoC;

namespace OnionEngine.UserInterface
{
	public class RootControl : Control
	{
		public override UIDim2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public override UIDim2 Size
		{
			get => new(window.width, 0, window.height, 0);
			set { throw new Exception("Cannot set size of root control!"); }
		}

		public override Vec2<int> SizeAbsolute { get => new(window.width, window.height); }

		[Dependency]
		private Window window = default!;

		public override List<RenderData> RenderThis()
		{
			return new List<RenderData>() { };
		}

		public override void RecalculateDimensions()
		{
			foreach (Control child in children)
				child.RecalculateDimensions();
		}
	}
}