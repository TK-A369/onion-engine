using OnionEngine.Core;

namespace OnionEngine.UserInterface
{
	/// <summary>
	/// Component that stores information about user interface of the owning entity.
	/// Entities owning this compoennt should also have <see cref="OnionEngine.Graphics.RenderComponent" />
	/// </summary>
	public sealed class UserInterfaceComponent : Component
	{
		public RootControl? uiRootControl;
	}
}