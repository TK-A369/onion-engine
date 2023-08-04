namespace OnionEngine.Prototypes
{
	public class PrototypeJSONAutoparseAttribute : Attribute { }

	public class PrototypeJSONAutoparsedFieldAttribute : Attribute
	{
		public bool required = true;
		public string? nameOverride = null;

		public PrototypeJSONAutoparsedFieldAttribute() { }
		public PrototypeJSONAutoparsedFieldAttribute(bool required)
		{
			this.required = required;
		}
		public PrototypeJSONAutoparsedFieldAttribute(bool required, string? nameOverride)
		{
			this.required = required;
			this.nameOverride = nameOverride;
		}
	}
}