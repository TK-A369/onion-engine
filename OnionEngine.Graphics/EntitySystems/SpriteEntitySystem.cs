using OnionEngine.Core;
using OnionEngine.Physics;
using OnionEngine.DataTypes;
using OnionEngine.IoC;

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

		[Dependency]
		Window window = default!;

		Mat<float> textureTransform = new Mat<float>(3, 3);

		int textureAtlasId = 0;

		public override void OnCreate()
		{
			textureTransform = window.textureAtlases[0].texturesTransformations[spriteComponent.textureName ?? throw new Exception("Texture name is null")];

			window.drawSprites.RegisterSubscriber((_) =>
			{
				renderComponent.renderData.Add(new RenderData()
				{
					vertices = new List<float>()
					{

					},
					indices = new List<int>() { 0, 1, 2, 1, 2, 3 },
					renderGroup = "textured-group"
				});
			});

			base.OnCreate();
		}
	}
}