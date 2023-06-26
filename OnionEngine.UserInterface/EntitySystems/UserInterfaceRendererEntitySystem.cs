using OnionEngine.Core;
using OnionEngine.Graphics;

namespace OnionEngine.UserInterface
{
	[EntitySystem]
	public sealed class UserInterfaceRendererEntitySystem : EntitySystem
	{
		[EntitySystemDependency]
		RenderComponent renderComponent = default!;

		[EntitySystemDependency]
		UserInterfaceComponent userInterfaceComponent = default!;
	}
}