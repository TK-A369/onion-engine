using OnionEngine.Core;

namespace OnionEngine.UserInterface
{
	/// <summary>
	/// Component that stores information about user interface of the owning entity.
	/// Entities owning this compoennt should also have <see cref="OnionEngine.Graphics.RenderComponent" />
	/// </summary>
	public sealed class UserInterfaceComponent : Component
	{
		/// <summary>
		/// Dictionary of root controls (windows) by their names.
		/// </summary>
		public Dictionary<string, RootControl> controls = new Dictionary<string, RootControl>();
	}
}