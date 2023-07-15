using System.Numerics;

namespace OnionEngine.DataTypes
{
	/// <summary>
	/// Vector of two values.
	/// </summary>
	/// <typeparam name="T">Type of values, typically <c>int</c>, <c>float</c> or <c>double</c></typeparam>
	public struct Vec2<T> where T : INumber<T>
	{
		public T x, y;

		public Vec2(T _x, T _y)
		{
			x = _x;
			y = _y;
		}

		public Mat<T> ToMatVertical()
		{
			Mat<T> mat = new Mat<T>(3, 1);
			mat.Element(0, 0) = x;
			mat.Element(1, 0) = y;
			mat.Element(2, 0) = (T)1;
		}
	}
}