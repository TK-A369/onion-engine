using OnionEngine.Graphics;

namespace OnionEngine.UserInterface
{
	abstract class Control
	{
		public Control? parent;

		public List<Control> children = new List<Control>();

		protected UIDim2 position;
		public virtual UIDim2 Position
		{
			get { return position; }
			set
			{
				position = value;
				// TODO: Check dimensions of parent
				positionAbsolute = position.Calculate(100, 100);
			}
		}
		public Vec2i positionAbsolute;
		public UIDim2 Size;

		public abstract List<RenderData> Render();
	}
}