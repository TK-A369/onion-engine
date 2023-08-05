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
		[EntitySystemDependency]
		RotationComponent rotationComponent = default!;

		[EntitySystemDependency]
		RigidBodyComponent rigidBodyComponent = default!;

		[Dependency]
		Window window = default!;

		public override void OnCreate()
		{
			window.updateFrameEvent.RegisterSubscriber(OnUpdateFrame);

			base.OnCreate();
		}

		public void OnUpdateFrame(object? _)
		{
			rotationComponent.rotation += rigidBodyComponent.angularVelocity;
		}
	}
}