using OnionEngine.Prototypes;

namespace OnionEngine.DataTypes
{
	[PrototypeJSONAutoparse]
	public struct ColorRGB
	{
		[PrototypeJSONAutoparsedField(true)]
		public float r;

		[PrototypeJSONAutoparsedField(true)]
		public float g;

		[PrototypeJSONAutoparsedField(true)]
		public float b;

		public ColorRGB(float _r, float _g, float _b)
		{
			r = _r;
			g = _g;
			b = _b;
		}
	}

	public struct ColorRGBA
	{
		public float r, g, b, a;

		public ColorRGBA(float _r, float _g, float _b, float _a)
		{
			r = _r;
			g = _g;
			b = _b;
			a = _a;
		}
	}
}