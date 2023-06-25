using OnionEngine.DataTypes;

namespace OnionEngine.UserInterface
{
	public struct UIDim1
	{
		int absolutePx;
		double relativePercent;

		public UIDim1(int _absolutePx, double _relativePercent)
		{
			absolutePx = _absolutePx;
			relativePercent = _relativePercent;
		}

		public int Calculate(int containerSize)
		{
			return (int)(absolutePx + (relativePercent * containerSize));
		}
	}

	public struct UIDim2
	{
		int absolutePxX;
		double relativePercentX;
		int absolutePxY;
		double relativePercentY;

		public UIDim2(int _absolutePxX, double _relativePercentX, int _absolutePxY, double _relativePercentY)
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