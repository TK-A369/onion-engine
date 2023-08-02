using OnionEngine.Prototypes;

namespace OnionEngine.Graphics
{
	[Prototype]
	public class TextureAtlasPrototype : Prototype
	{
		[PrototypeParameter(true)]
		public string name = "";
		[PrototypeParameter(true)]
		public int size;
		[PrototypeParameter(true)]
		public Dictionary<string, string> textures = new Dictionary<string, string>();
	}
}