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

		public readonly Mat<T> ToMatVertical()
		{
			Mat<T> mat = new(3, 1);
			mat.Element(0, 0) = x;
			mat.Element(1, 0) = y;
			mat.Element(2, 0) = (T)(dynamic)1.0;
			return mat;
		}

		public readonly Mat<T> ToMatTransform()
		{
			Mat<T> mat = new(3, 3);
			mat.Element(0, 0) = (T)(dynamic)1.0;
			mat.Element(0, 1) = (T)(dynamic)0.0;
			mat.Element(0, 2) = x;
			mat.Element(1, 0) = (T)(dynamic)0.0;
			mat.Element(1, 1) = (T)(dynamic)1.0;
			mat.Element(1, 2) = y;
			mat.Element(2, 0) = (T)(dynamic)0.0;
			mat.Element(2, 1) = (T)(dynamic)0.0;
			mat.Element(2, 2) = (T)(dynamic)1.0;
			return mat;
		}

		public static Vec2<T> operator +(Vec2<T> a, Vec2<T> b) => new(a.x + b.x, a.y + b.y);
	}
}