using OnionEngine.Core;
using OnionEngine.IoC;
using OnionEngine.Graphics;

namespace OnionEngine.Physics
{
	/// <summary>
	/// Entity system that moves physical bodies.
	/// </summary>
	[EntitySystem]
	public class PhysicalBodyEntitySystem : EntitySystem
	{
		[EntitySystemDependency]
		PositionComponent positionComponent = default!;

		[EntitySystemDependency]
		PhysicalBodyComponent physicalBodyComponent = default!;

		[Dependency]
		Window window = default!;

		public override void OnCreate()
		{
			window.updateFrameEvent.RegisterSubscriber(OnUpdateFrame);

			base.OnCreate();
		}

		public void OnUpdateFrame(object? _)
		{
			positionComponent.position += physicalBodyComponent.velocity;
		}
	}
}