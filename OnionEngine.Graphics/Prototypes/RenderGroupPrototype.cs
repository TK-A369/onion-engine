using OnionEngine.Prototypes;

namespace OnionEngine.Graphics
{
	[Prototype]
	public class RenderGroupPrototype : Prototype
	{
		[PrototypeParameter(true)]
		public string name = "";
		[PrototypeParameter(true)]
		public string shaderName = "";
		[PrototypeParameter(false)]
		public string? textureAtlasName = null;
	}
}