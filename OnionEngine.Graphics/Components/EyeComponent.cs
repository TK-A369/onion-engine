using OnionEngine.Core;
using OnionEngine.DataTypes;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Component containing data about image to render.
	/// </summary>
	[Component]
	public sealed class EyeComponent : Component
	{
		public Vec2<double> position;

		public double rotation;

		public Mat<float> GetEyeMatrix() =>
			position.ToMatTransform().Cast<float>() * Mat<float>.RotationMatrix(rotation);
	}
}