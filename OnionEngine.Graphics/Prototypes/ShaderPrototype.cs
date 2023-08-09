using OnionEngine.Prototypes;

namespace OnionEngine.Graphics
{
	[Prototype]
	public class ShaderPrototype : Prototype
	{
		[PrototypeParameter(true)]
		public string name = "";
		[PrototypeParameter(true)]
		public string vertexPath = "";
		[PrototypeParameter(true)]
		public string fragmentPath = "";
		[PrototypeParameter(true)]
		public List<VertexAttributeDescriptor> vertexAttributeDescriptors = new();
	}
}