using OnionEngine.Core;
using OnionEngine.IoC;
using OnionEngine.Graphics;

namespace OnionEngine.Physics
{
	/// <summary>
	/// Entity system that moves physical bodies.
	/// </summary>
	[EntitySystem]
	public class RigidBodyEntitySystem : EntitySystem
	{
		private EventSubscriber<object?>? updateFrameSubscriber;

		[EntitySystemDependency]
		private RotationComponent rotationComponent = default!;

		[EntitySystemDependency]
		private RigidBodyComponent rigidBodyComponent = default!;

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
			rotationComponent.rotation += rigidBodyComponent.angularVelocity;
			if (rotationComponent.rotation > 2 * Math.PI)
				rotationComponent.rotation -= 2 * Math.PI;
			if (rotationComponent.rotation < 0)
				rotationComponent.rotation += 2 * Math.PI;
		}
	}
}