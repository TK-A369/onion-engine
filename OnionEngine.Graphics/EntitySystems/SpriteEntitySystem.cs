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
		private SpriteComponent spriteComponent = default!;

		[EntitySystemDependency]
		private PositionComponent positionComponent = default!;

		[EntitySystemDependency]
		private RenderComponent renderComponent = default!;

		[Dependency]
		private Window window = default!;

		[Dependency]
		private GameManager gameManager = default!;

		private Mat<float> textureTransform = new(3, 3);

		private Action<object?>? drawSpriteSubscriber;

		public override void OnCreate()
		{
			textureTransform = window.textureAtlases["texture-atlas-test"].texturesTransformations[spriteComponent.textureName ?? throw new Exception("Texture name is null")];

			drawSpriteSubscriber = (_) =>
			{
				double rotation = 0.0;
				if (gameManager.HasComponent(spriteComponent.entityId, typeof(RotationComponent)))
				{
					Int64 rotationComponentId = gameManager.GetComponent(spriteComponent.entityId, typeof(RotationComponent));
					RotationComponent rotationComponent = (RotationComponent)gameManager.components[rotationComponentId];
					rotation = rotationComponent.rotation;
				}

				Mat<float> positionGlobalMat = positionComponent.position.ToMatTransform().Cast<float>();
				Mat<float> positionRotatedGlobalMat = positionGlobalMat * Mat<float>.RotationMatrix(rotation);
				Mat<float> positionSWGlobalMat = positionRotatedGlobalMat *
					new Vec2<float>((float)(-spriteComponent.size.x / 2), (float)(-spriteComponent.size.y / 2)).ToMatVertical();
				Mat<float> positionNWGlobalMat = positionRotatedGlobalMat *
					new Vec2<float>((float)(-spriteComponent.size.x / 2), (float)(spriteComponent.size.y / 2)).ToMatVertical();
				Mat<float> positionNEGlobalMat = positionRotatedGlobalMat *
					new Vec2<float>((float)(spriteComponent.size.x / 2), (float)(spriteComponent.size.y / 2)).ToMatVertical();
				Mat<float> positionSEGlobalMat = positionRotatedGlobalMat *
					new Vec2<float>((float)(spriteComponent.size.x / 2), (float)(-spriteComponent.size.y / 2)).ToMatVertical();

				Mat<float> texCoordsSWMat = textureTransform * new Vec2<float>(0, 0).ToMatVertical();
				Mat<float> texCoordsNWMat = textureTransform * new Vec2<float>(0, 1).ToMatVertical();
				Mat<float> texCoordsNEMat = textureTransform * new Vec2<float>(1, 1).ToMatVertical();
				Mat<float> texCoordsSEMat = textureTransform * new Vec2<float>(1, 0).ToMatVertical();

				RenderData renderData = new()
				{
					vertices = new List<float>()
					{
						// X                               Y                                  Z  Color_____  Texture coordinates
						positionSWGlobalMat.Element(0, 0), positionSWGlobalMat.Element(1, 0), 0, 1, 0, 0, 1, texCoordsSWMat.Element(0, 0), texCoordsSWMat.Element(0, 1),
						positionNWGlobalMat.Element(0, 0), positionNWGlobalMat.Element(1, 0), 0, 0, 1, 0, 1, texCoordsNWMat.Element(0, 0), texCoordsNWMat.Element(0, 1),
						positionNEGlobalMat.Element(0, 0), positionNEGlobalMat.Element(1, 0), 0, 0, 0, 1, 1, texCoordsNEMat.Element(0, 0), texCoordsNEMat.Element(0, 1),
						positionSEGlobalMat.Element(0, 0), positionSEGlobalMat.Element(1, 0), 0, 1, 1, 0, 1, texCoordsSEMat.Element(0, 0), texCoordsSEMat.Element(0, 1),
					},
					indices = new List<int>() { 0, 1, 2, 0, 2, 3 },
					renderGroup = "render-group-textured"
				};

				renderComponent.renderData.Add(renderData);
			};
			window.drawSpritesEvent.RegisterSubscriber(new EventSubscriber<object?>(drawSpriteSubscriber));

			base.OnCreate();
		}
	}
}