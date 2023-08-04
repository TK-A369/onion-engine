using OnionEngine.Prototypes;

namespace OnionEngine.Graphics
{
	[Prototype]
	public class OffscreenRenderTargetPrototype : Prototype
	{
		[PrototypeParameter(true)]
		public string name = "";
		[PrototypeParameter(true)]
		public int width;
		[PrototypeParameter(true)]
		public int height;
	}
}