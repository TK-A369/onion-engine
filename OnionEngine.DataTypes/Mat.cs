using System.Numerics;
using System.Globalization;

namespace OnionEngine.DataTypes
{
	public struct Mat<T> where T : INumber<T>
	{
		public int m, n;
		public T[] values;

		public Mat(int _m, int _n)
		{
			m = _m;
			n = _n;
			values = new T[m * n];
		}

		public ref T Element(int i, int j)
		{
			return ref values[i * n + j];
		}

		public static Mat<T> operator *(Mat<T> a, Mat<T> b)
		{
			if (a.n != b.m)
				throw new Exception("Those matrices (" + a.m + "x" + a.n + ") and (" + b.m + "x" + b.n + ") cannot be multiplied");

			Mat<T> result = new(a.m, b.n);
			for (int i = 0; i < a.m; i++)
			{
				for (int j = 0; j < b.n; j++)
				{
					for (int k = 0; k < a.n; k++)
					{
						result.Element(i, j) += a.Element(i, k) * b.Element(k, j);
					}
				}
			}
			return result;
		}

		public static Mat<T> operator *(Mat<T> a, T b)
		{
			Mat<T> result = new(a.m, a.n);
			for (int i = 0; i < a.m; i++)
			{
				for (int j = 0; j < a.n; j++)
				{
					result.Element(i, j) = a.Element(i, j) * b;
				}
			}

			return result;
		}

		public override string ToString()
		{
			string result = "";

			for (int i = 0; i < m; i++)
			{
				result += "| ";
				for (int j = 0; j < n; j++)
				{
					result += Element(i, j).ToString("N", CultureInfo.InvariantCulture) + "\t";
				}
				result += "|\n";
			}

			return result;
		}

		public Mat<T2> Cast<T2>() where T2 : INumber<T2>
		{
			Mat<T2> result = new(m, n);
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					result.Element(i, j) = (T2)(dynamic)Element(i, j);
				}
			}

			return result;
		}

		public static Mat<T> RotationMatrix(double angle)
		{
			Mat<T> result = new(3, 3);
			result.Element(0, 0) = (T)(dynamic)Math.Cos(angle);
			result.Element(0, 1) = (T)(dynamic)Math.Sin(angle);
			result.Element(0, 2) = (T)(dynamic)0.0;
			result.Element(1, 0) = (T)(dynamic)(-Math.Sin(angle));
			result.Element(1, 1) = (T)(dynamic)Math.Cos(angle);
			result.Element(1, 2) = (T)(dynamic)0.0;
			result.Element(2, 0) = (T)(dynamic)0.0;
			result.Element(2, 1) = (T)(dynamic)0.0;
			result.Element(2, 2) = (T)(dynamic)1.0;
			return result;
		}
	}
}