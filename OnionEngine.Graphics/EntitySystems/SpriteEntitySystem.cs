using OnionEngine.Core;
using OnionEngine.Physics;

namespace OnionEngine.Graphics
{
	[EntitySystem]
	public class SpriteEntitySystem : EntitySystem
	{
		[EntitySystemDependency]
		SpriteComponent spriteComponent = default!;

		[EntitySystemDependency]
		PositionComponent positionComponent = default!;

		[EntitySystemDependency]
		RenderComponent renderComponent = default!;
	}
}