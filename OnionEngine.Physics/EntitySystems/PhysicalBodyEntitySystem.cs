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
		private EventSubscriber<object?>? updateFrameSubscriber;

		[EntitySystemDependency]
		private PositionComponent positionComponent = default!;

		[EntitySystemDependency]
		private PhysicalBodyComponent physicalBodyComponent = default!;

		[Dependency]
		private Window window = default!;

		public override void OnCreate()
		{
			updateFrameSubscriber = OnUpdateFrame;
			window.updateFrameEvent.RegisterSubscriber(updateFrameSubscriber);

			base.OnCreate();
		}

		public void OnUpdateFrame(object? _)
		{
			positionComponent.position += physicalBodyComponent.velocity;
		}
	}
}