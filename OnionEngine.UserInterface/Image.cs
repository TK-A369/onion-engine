using OnionEngine.Graphics;
using OnionEngine.DataTypes;
using OnionEngine.IoC;

namespace OnionEngine.UserInterface
{
	public class Image : Control
	{
		/// <summary>
		/// It can be texture atlas (in form of "{texture atlas name}.{internal texture name}") or offscreen render target name.)
		/// </summary>
		public string textureName = "";

		[Dependency]
		private Window window = default!;

		public Image(Control parent)
		{
			this.parent = parent;
		}

		public override List<RenderData> RenderThis()
		{
			string[] textureNameSplitted = textureName.Split('.');
			Mat<float> textureTransform = new(3, 3);
			textureTransform.Element(0, 0) = 1.0f;
			textureTransform.Element(0, 1) = 0.0f;
			textureTransform.Element(0, 2) = 0.0f;
			textureTransform.Element(1, 0) = 0.0f;
			textureTransform.Element(1, 1) = 1.0f;
			textureTransform.Element(1, 2) = 0.0f;
			textureTransform.Element(2, 0) = 0.0f;
			textureTransform.Element(2, 1) = 0.0f;
			textureTransform.Element(2, 2) = 1.0f;
			// WIP
			if (textureNameSplitted.Length == 1)
			{
				OffscreenRenderTarget offscreenRenderTarget = window.offscreenRenderTargets[textureNameSplitted[0]];
				textureTransform.Element(0, 0) = 1.0f;
				textureTransform.Element(0, 1) = 0.0f;
				textureTransform.Element(0, 2) = 0.0f;
				textureTransform.Element(1, 0) = 0.0f;
				textureTransform.Element(1, 1) = 1.0f;
				textureTransform.Element(1, 2) = 0.0f;
				textureTransform.Element(2, 0) = 0.0f;
				textureTransform.Element(2, 1) = 0.0f;
				textureTransform.Element(2, 2) = 1.0f;
			}
			else
			{
				textureTransform = window.textureAtlases["texture-atlas-test"].texturesTransformations[textureNameSplitted[0] ?? throw new Exception("Texture name is null")];
			}

			Vec2<float> toDeviceNormalisedCoordinates(Vec2<int> coords)
				=> new((((float)coords.x) / window.width * 2.0f) - 1.0f, (((float)coords.y) / window.height * 2.0f) - 1.0f);

			Vec2<float>[] verticesCoords = new Vec2<float>[4] {
				toDeviceNormalisedCoordinates(positionAbsoluteGlobal),
				toDeviceNormalisedCoordinates(positionAbsoluteGlobal + new Vec2<int>(SizeAbsolute.x, 0)),
				toDeviceNormalisedCoordinates(positionAbsoluteGlobal + SizeAbsolute),
				toDeviceNormalisedCoordinates(positionAbsoluteGlobal + new Vec2<int>(0, SizeAbsolute.y))
			};

			return new List<RenderData>()
			{
				new RenderData() {
					vertices = new() {
						verticesCoords[0].x, verticesCoords[0].y, 0.0f, 0.0f,
						verticesCoords[1].x, verticesCoords[1].y, 1.0f, 0.0f,
						verticesCoords[2].x, verticesCoords[2].y, 1.0f, 1.0f,
						verticesCoords[3].x, verticesCoords[3].y, 0.0f, 1.0f,
					},
					indices = new() {
						0, 1, 2,
						0, 2, 3
					},
					renderGroup = "render-group-ui-textured"
				}
			};
		}
	}
}