using OnionEngine.Core;
using OnionEngine.Physics;
using OnionEngine.DataTypes;
using OnionEngine.IoC;

namespace OnionEngine.Graphics
{
	[EntitySystem]
	public class ShadowCasterEntitySystem : EntitySystem
	{
		[EntitySystemDependency]
		private ShadowCasterComponent shadowCasterComponent = default!;

		[EntitySystemDependency]
		private PositionComponent positionComponent = default!;

		[Dependency]
		private Window window = default!;

		[Dependency]
		private GameManager gameManager = default!;

		public override void OnCreate()
		{
			base.OnCreate();
		}

		public RenderData GetShadowsRenderData(Vec2<float> sourcePosition)
		{
			double rotation = 0.0;
			if (gameManager.HasComponent(shadowCasterComponent.entityId, typeof(RotationComponent)))
			{
				Int64 rotationComponentId = gameManager.GetComponent(shadowCasterComponent.entityId, typeof(RotationComponent));
				RotationComponent rotationComponent = (RotationComponent)gameManager.components[rotationComponentId];
				rotation = rotationComponent.rotation;
			}

			Mat<float> positionGlobalMat = positionComponent.position.ToMatTransform().Cast<float>();
			Mat<float> positionRotatedGlobalMat = positionGlobalMat * Mat<float>.RotationMatrix(rotation);

			List<float> vertices = new();
			List<int> indices = new();
			int indexOffset = 0;

			for (int i = 0; i < shadowCasterComponent.shadowsEndpoints.Count; i += 2)
			{
				Mat<float> positionEndpoint1GlobalMat = positionRotatedGlobalMat *
					new Vec2<float>(
						(float)shadowCasterComponent.shadowsEndpoints[i].x,
						(float)shadowCasterComponent.shadowsEndpoints[i].y).ToMatVertical();
				Mat<float> positionEndpoint2GlobalMat = positionRotatedGlobalMat *
					new Vec2<float>(
						(float)shadowCasterComponent.shadowsEndpoints[i + 1].x,
						(float)shadowCasterComponent.shadowsEndpoints[i + 1].y).ToMatVertical();

				Vec2<float> positionEndpoint1 = new(positionEndpoint1GlobalMat.Element(0, 0), positionEndpoint1GlobalMat.Element(1, 0));
				Vec2<float> positionEndpoint2 = new(positionEndpoint2GlobalMat.Element(0, 0), positionEndpoint2GlobalMat.Element(1, 0));

				Vec2<float> delta1 = (positionEndpoint1 - (sourcePosition * -1)).Normalize();
				Vec2<float> delta2 = (positionEndpoint2 - (sourcePosition * -1)).Normalize();

				float farMultiplier = 10.0f;

				Vec2<float> positionEndpointFar1 = positionEndpoint1 + (delta1 * farMultiplier);
				Vec2<float> positionEndpointFar2 = positionEndpoint2 + (delta2 * farMultiplier);

				vertices.AddRange(new List<float>()
				{
					positionEndpoint1.x, positionEndpoint1.y,
					positionEndpoint2.x, positionEndpoint2.y,
					positionEndpointFar1.x, positionEndpointFar1.y,
					positionEndpointFar2.x, positionEndpointFar2.y
				});
				indices.AddRange(new List<int>() {
					indexOffset + 0, indexOffset + 1, indexOffset + 2,
					indexOffset + 1, indexOffset + 2, indexOffset + 3
				});
				indexOffset += 4;
			}

			return new RenderData()
			{
				vertices = vertices,
				indices = indices,
				renderGroup = "render-group-shadows"
			};
		}
	}
}