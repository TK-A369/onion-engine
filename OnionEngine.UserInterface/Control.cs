using OnionEngine.DataTypes;
using OnionEngine.Graphics;
using OnionEngine.IoC;

namespace OnionEngine.UserInterface
{
	/// <summary>
	/// Base class of all UI controls.
	/// </summary>
	public abstract class Control
	{
		/// <summary>
		/// Parent of this control.
		/// </summary>
		public Control? parent = null;

		/// <summary>
		/// List of children of this control.
		/// </summary>
		public List<Control> children = new();

		protected UIDim2 position = new(0, 0, 0, 0);
		/// <summary>
		/// Position of this control inside its parent.
		/// </summary>
		public virtual UIDim2 Position
		{
			get => position;
			set
			{
				position = value;
				positionAbsoluteLocal = position.Calculate((parent ?? throw new Exception("Couldn't calculate position of parentless control")).SizeAbsolute.x, parent.SizeAbsolute.y);
				positionAbsoluteGlobal = positionAbsoluteLocal + parent.positionAbsoluteGlobal;
			}
		}

		public Vec2<int> positionAbsoluteLocal = new(0, 0);
		public Vec2<int> positionAbsoluteGlobal = new(0, 0);

		/// <summary>
		/// Size of this control.
		/// </summary>
		protected UIDim2 size = new(0, 0, 0, 0);
		public virtual UIDim2 Size
		{
			get => size;
			set
			{
				size = value;
				sizeAbsolute = size.Calculate((parent ?? throw new Exception("Couldn't calculate size of parentless control")).SizeAbsolute.x, parent.SizeAbsolute.y);
				positionAbsoluteLocal = position.Calculate(parent.SizeAbsolute.x, parent.SizeAbsolute.y);
				positionAbsoluteGlobal = positionAbsoluteLocal + parent.positionAbsoluteGlobal;
			}
		}

		protected Vec2<int> sizeAbsolute;
		public virtual Vec2<int> SizeAbsolute { get => sizeAbsolute; }

		/// <summary>
		/// Generate render data.
		/// </summary>
		/// <returns>List of <see cref="RenderData" /> objects.</returns>
		public abstract List<RenderData> RenderThis();

		public List<RenderData> Render()
		{
			List<RenderData> result = new();

			// Render itself
			result.AddRange(RenderThis());

			// Render children
			foreach (Control child in children)
			{
				result.AddRange(child.Render());
			}

			// Console.WriteLine("UI render data:");
			// Console.WriteLine("(" + string.Join("), (", from elem in result select (string.Join(", ", elem.vertices))) + ")");

			return result;
		}

		public void AddChild(Control child)
		{
			children.Add(child);
			child.parent = this;
		}

		public virtual void RecalculateDimensions()
		{
			Size = Size;
			Position = Position;

			foreach (Control child in children)
				child.RecalculateDimensions();
		}
	}
}