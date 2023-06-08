using OnionEngine.DataTypes;
using OnionEngine.Graphics;

namespace OnionEngine.UserInterface
{
	abstract class Control
	{
		public Control? parent = null;

		public List<Control> children = new List<Control>();

		protected UIDim2 position = new UIDim2(0, 0, 0, 0);
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
		public Vec2<int> positionAbsolute;
		public UIDim2 Size = new UIDim2(0, 0, 0, 0);

		public abstract List<RenderData> Render();
	}
}