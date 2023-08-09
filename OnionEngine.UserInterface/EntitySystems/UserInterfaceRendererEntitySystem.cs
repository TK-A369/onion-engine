using OnionEngine.Core;
using OnionEngine.Graphics;
using OnionEngine.IoC;

namespace OnionEngine.UserInterface
{
	[EntitySystem]
	public sealed class UserInterfaceRendererEntitySystem : EntitySystem
	{
		private Action<object?>? drawSpriteSubscriber;

		[EntitySystemDependency]
		private RenderComponent renderComponent = default!;

		[EntitySystemDependency]
		private UserInterfaceComponent userInterfaceComponent = default!;

		[Dependency]
		private Window window = default!;

		public override void OnCreate()
		{
			base.OnCreate();

			drawSpriteSubscriber = (_) =>
			{
				renderComponent.renderData.AddRange(userInterfaceComponent.uiRootControl.Render());
			};
			window.drawSpritesEvent.RegisterSubscriber(new EventSubscriber<object?>(drawSpriteSubscriber));
		}
	}
}