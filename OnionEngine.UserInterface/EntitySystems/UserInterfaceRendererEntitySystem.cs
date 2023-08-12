using OnionEngine.Core;
using OnionEngine.Graphics;
using OnionEngine.IoC;

namespace OnionEngine.UserInterface
{
	[EntitySystem]
	public sealed class UserInterfaceRendererEntitySystem : EntitySystem
	{
		private EventSubscriber<object?>? drawSpriteSubscriber;

		private EventSubscriber<object?>? windowResizeSubscriber;

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
				if (userInterfaceComponent.uiRootControl != null)
				{
					renderComponent.renderData.AddRange(userInterfaceComponent.uiRootControl.Render());

					// Console.WriteLine("Rendering UI...");
					// Console.WriteLine(string.Join("), ", from elem in renderComponent.renderData select (elem.renderGroup + ": (" + string.Join(", ", elem.vertices))) + ")");
				}
			};
			window.drawSpritesEvent.RegisterSubscriber(drawSpriteSubscriber);

			windowResizeSubscriber = (_) =>
			{
				userInterfaceComponent.uiRootControl?.RecalculateDimensions();
			};
			window.windowResizeEvent.RegisterSubscriber(windowResizeSubscriber);
		}
	}
}