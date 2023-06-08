using OnionEngine.DataTypes;

namespace OnionEngine.UserInterface
{
	struct UIDim1
	{
		int absolutePx;
		float relativePercent;

		public UIDim1(int _absolutePx, float _relativePercent)
		{
			absolutePx = _absolutePx;
			relativePercent = _relativePercent;
		}

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

		public UIDim2(int _absolutePxX, float _relativePercentX, int _absolutePxY, float _relativePercentY)
		{
			absolutePxX = _absolutePxX;
			relativePercentX = _relativePercentX;
			absolutePxY = _absolutePxY;
			relativePercentY = _relativePercentY;
		}

		public Vec2<int> Calculate(int containerSizeX, int containerSizeY)
		{
			return new Vec2<int>()
			{
				x = (int)(absolutePxX + (relativePercentX * containerSizeX)),
				y = (int)(absolutePxY + (relativePercentY * containerSizeY))
			};
		}
	}
}