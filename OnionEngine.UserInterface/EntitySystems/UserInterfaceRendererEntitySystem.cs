using OnionEngine.Core;
using OnionEngine.Graphics;

namespace OnionEngine.UserInterface
{
	[EntitySystem]
	public sealed class UserInterfaceRendererEntitySystem : EntitySystem
	{
		[EntitySystemDependency]
		public RenderComponent renderComponent = default!;

		[EntitySystemDependency]
		public UserInterfaceComponent userInterfaceComponent = default!;
	}
}