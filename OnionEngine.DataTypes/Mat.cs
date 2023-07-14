using System.Numerics;
using System.Globalization;

namespace OnionEngine.DataTypes
{
	public struct Mat<T> where T : INumber<T>
	{
		public int m, n;
		T[] values;

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
				throw new Exception("Those matrices cannot be multiplied");

			Mat<T> result = new Mat<T>(a.m, b.n);
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
	}
}