using OnionEngine.Graphics;
using OnionEngine.DataTypes;
using OnionEngine.IoC;

namespace OnionEngine.UserInterface
{
	public class Frame : Control
	{
		public ColorRGBA borderColor = new(0.5f, 0.5f, 0.5f, 0.8f);

		public int borderWidth = 2;

		public ColorRGBA backgroundColor = new(0.2f, 0.2f, 0.2f, 0.5f);

		[Dependency]
		private Window window = default!;

		public Frame(Control parent)
		{
			this.parent = parent;
		}

		public override List<RenderData> RenderThis()
		{
			// throw new NotImplementedException();

			Vec2<float> toDeviceNormalisedCoordinates(Vec2<int> coords)
				=> new((((float)coords.x) / window.width * 2) - 1, (((float)coords.y) / window.height * 2) - 1);

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
						verticesCoords[0].x, verticesCoords[0].y, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a,
						verticesCoords[1].x, verticesCoords[1].y, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a,
						verticesCoords[2].x, verticesCoords[2].y, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a,
						verticesCoords[3].x, verticesCoords[3].y, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a
					},
					indices = new() {
						0, 1, 2,
						0, 2, 3
					},
					renderGroup = "render-group-ui-unicolor"
				}
			};
		}
	}
}