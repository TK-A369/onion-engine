using OnionEngine.Core;
using OnionEngine.Graphics;
using OnionEngine.IoC;

namespace OnionEngine.UserInterface
{
	[EntitySystem]
	public sealed class UserInterfaceRendererEntitySystem : EntitySystem
	{
		private EventSubscriber<object?>? drawSpriteSubscriber;

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

				Console.WriteLine("Rendering UI...");
				Console.WriteLine(string.Join("), ", from elem in renderComponent.renderData select (elem.renderGroup + ": (" + string.Join(", ", elem.vertices))) + ")");
			};
			window.drawSpritesEvent.RegisterSubscriber(drawSpriteSubscriber);
		}
	}
}