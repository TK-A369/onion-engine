using OnionEngine.Core;
using OnionEngine.Physics;
using OnionEngine.DataTypes;
using OnionEngine.IoC;

namespace OnionEngine.Graphics
{
	[EntitySystem]
	public class LightSourceEntitySystem : EntitySystem
	{
		private Mat<float> textureTransform = new(3, 3);

		[EntitySystemDependency]
		private LightSourceComponent lightSourceComponent = default!;

		[EntitySystemDependency]
		private PositionComponent positionComponent = default!;

		[Dependency]
		private Window window = default!;

		[Dependency]
		private GameManager gameManager = default!;

		public override void OnCreate()
		{
			base.OnCreate();

			textureTransform = window.textureAtlases["texture-atlas-lightmaps"].texturesTransformations[lightSourceComponent.lightmapTextureName];
		}

		public RenderData GetLightRenderData()
		{
			Mat<float> positionGlobalMat = positionComponent.position.ToMatTransform().Cast<float>();
			Mat<float> positionSWGlobalMat = positionGlobalMat *
				new Vec2<float>((float)(-lightSourceComponent.size / 2), (float)(-lightSourceComponent.size / 2)).ToMatVertical();
			Mat<float> positionNWGlobalMat = positionGlobalMat *
				new Vec2<float>((float)(-lightSourceComponent.size / 2), (float)(lightSourceComponent.size / 2)).ToMatVertical();
			Mat<float> positionNEGlobalMat = positionGlobalMat *
				new Vec2<float>((float)(lightSourceComponent.size / 2), (float)(lightSourceComponent.size / 2)).ToMatVertical();
			Mat<float> positionSEGlobalMat = positionGlobalMat *
				new Vec2<float>((float)(lightSourceComponent.size / 2), (float)(-lightSourceComponent.size / 2)).ToMatVertical();

			Mat<float> texCoordsSWMat = textureTransform * new Vec2<float>(0, 0).ToMatVertical();
			Mat<float> texCoordsNWMat = textureTransform * new Vec2<float>(0, 1).ToMatVertical();
			Mat<float> texCoordsNEMat = textureTransform * new Vec2<float>(1, 1).ToMatVertical();
			Mat<float> texCoordsSEMat = textureTransform * new Vec2<float>(1, 0).ToMatVertical();

			return new RenderData()
			{
				vertices = new List<float>()
					{
						// X                               Y                                  Z  Texture coordinates
						positionSWGlobalMat.Element(0, 0), positionSWGlobalMat.Element(1, 0), 0, texCoordsSWMat.Element(0, 0), texCoordsSWMat.Element(0, 1),
						positionNWGlobalMat.Element(0, 0), positionNWGlobalMat.Element(1, 0), 0, texCoordsNWMat.Element(0, 0), texCoordsNWMat.Element(0, 1),
						positionNEGlobalMat.Element(0, 0), positionNEGlobalMat.Element(1, 0), 0, texCoordsNEMat.Element(0, 0), texCoordsNEMat.Element(0, 1),
						positionSEGlobalMat.Element(0, 0), positionSEGlobalMat.Element(1, 0), 0, texCoordsSEMat.Element(0, 0), texCoordsSEMat.Element(0, 1),
					},
				indices = new List<int>() { 0, 1, 2, 0, 2, 3 },
				textureAtlasName = "texture-atlas-lightmaps",
				renderGroup = "render-group-lights"
			};
		}
	}
}