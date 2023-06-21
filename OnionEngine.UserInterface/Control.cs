using OnionEngine.DataTypes;
using OnionEngine.Graphics;

namespace OnionEngine.UserInterface
{
	/// <summary>
	/// Base class of all UI controls.
	/// </summary>
	abstract class Control
	{
		/// <summary>
		/// Parent of this control.
		/// </summary>
		public Control? parent = null;

		/// <summary>
		/// List of children of this control.
		/// </summary>
		public List<Control> children = new List<Control>();

		protected UIDim2 position = new UIDim2(0, 0, 0, 0);
		/// <summary>
		/// Position of this control inside its parent.
		/// </summary>
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

		/// <summary>
		/// Size of this control.
		/// </summary>
		public UIDim2 Size = new UIDim2(0, 0, 0, 0);

		/// <summary>
		/// Generate render data.
		/// </summary>
		/// <returns>List of <see cref="RenderData" /> objects.</returns>
		public abstract List<RenderData> Render();
	}
}