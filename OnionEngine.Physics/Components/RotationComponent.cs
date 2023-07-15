using OnionEngine.Core;

namespace OnionEngine.Physics
{
	/// <summary>
	/// Component storing position of object.
	/// </summary>
	[Component]
	public sealed class RotationComponent : Component
	{
		public double rotation = 0.0;
	}
}