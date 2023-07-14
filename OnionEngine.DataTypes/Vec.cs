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
	}
}