namespace OnionEngine.UserInterface
{
	struct Vec2i
	{
		public int x, y;
	}

	struct UIDim1
	{
		int absolutePx;
		float relativePercent;

		public int Calculate(int containerSize)
		{
			return (int)(absolutePx + (relativePercent * containerSize));
		}
	}

	struct UIDim2
	{
		int absolutePxX;
		float relativePercentX;
		int absolutePxY;
		float relativePercentY;

		public Vec2i Calculate(int containerSizeX, int containerSizeY)
		{
			return new Vec2i()
			{
				x = (int)(absolutePxX + (relativePercentX * containerSizeX)),
				y = (int)(absolutePxY + (relativePercentY * containerSizeY))
			};
		}
	}
}