using OnionEngine.Core;
using OnionEngine.IoC;
using OnionEngine.Prototypes;

using OpenTK.Graphics.OpenGL4;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Struct describing vertex attributes.
	/// For example position is usually float*3 or float*2.
	/// </summary>
	[PrototypeJSONAutoparse]
	public struct VertexAttributeDescriptor
	{
		[PrototypeJSONAutoparsedField]
		public VertexAttribPointerType type;
		[PrototypeJSONAutoparsedField]
		public int valuesCount;
		[PrototypeJSONAutoparsedField(false)]
		public bool normalized = false;

		public VertexAttributeDescriptor() { }
	}

	public class RenderHooks
	{
		public Action? bindTextures = null;
	}

	/// <summary>
	/// Render group, which has given list of vertex attributes, and uses given shader.
	/// </summary>
	public class RenderGroup : IDisposable
	{
		// OpenGL stuff

		/// <summary>
		/// Handle to VAO.
		/// It stores informations about vertex attributes and their layout.
		/// </summary>
		public int vertexArrayObject;

		/// <summary>
		/// Handle to VBO.
		/// It stores informations about vertices.
		/// </summary>
		public int vertexBufferObject;

		/// <summary>
		/// Handle to EBO.
		/// It stores informations about order of vertices.
		/// </summary>
		public int elementBufferObject;

		/// <summary>
		/// Shader object, used by this render group.
		/// </summary>
		public Shader shader;

		/// <summary>
		/// If OpenGL buffers were disposed
		/// </summary>
		private bool disposed = false;

		[Dependency]
		private Window window = default!;

		public RenderGroup(Shader _shader)
		{
			IoCManager.InjectDependencies(this);

			shader = _shader;

			// Generate OpenGL buffers
			vertexArrayObject = GL.GenVertexArray();
			vertexBufferObject = GL.GenBuffer();
			elementBufferObject = GL.GenBuffer();

			// Bind those buffers
			Bind();

			// Configure VAO
			// Firstly, calculate sizes of attributes
			int totalAttributesSizePerVertex = 0;
			List<int> attributesSizes = new();
			foreach (VertexAttributeDescriptor desc in shader.vertexAttributesDescriptors)
			{
				int attributeSize = 0;
				switch (desc.type)
				{
					case VertexAttribPointerType.Float:
						attributeSize = sizeof(float) * desc.valuesCount;
						break;
					case VertexAttribPointerType.Double:
						attributeSize = sizeof(double) * desc.valuesCount;
						break;
					case VertexAttribPointerType.Byte:
						attributeSize = sizeof(byte) * desc.valuesCount;
						break;
					case VertexAttribPointerType.UnsignedByte:
						attributeSize = sizeof(byte) * desc.valuesCount;
						break;
					case VertexAttribPointerType.Int:
						attributeSize = sizeof(int) * desc.valuesCount;
						break;
					case VertexAttribPointerType.UnsignedInt:
						attributeSize = sizeof(uint) * desc.valuesCount;
						break;
				}
				attributesSizes.Add(attributeSize);
				totalAttributesSizePerVertex += attributeSize;
			}
			// Then write those values to VAO
			int i = 0;
			int attributesSizeUntilNow = 0;
			foreach (VertexAttributeDescriptor desc in shader.vertexAttributesDescriptors)
			{
				GL.VertexAttribPointer(i, desc.valuesCount, desc.type, desc.normalized, totalAttributesSizePerVertex, attributesSizeUntilNow);
				GL.EnableVertexAttribArray(i);
				attributesSizeUntilNow += attributesSizes[i];
				i++;
			}
		}

		~RenderGroup()
		{
			if (disposed == false)
			{
				Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				GL.DeleteVertexArray(vertexArrayObject);
				GL.DeleteBuffer(vertexBufferObject);
				GL.DeleteBuffer(elementBufferObject);

				disposed = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Bind VAO, VBO and EBO, so they will be used in subsequent operations.
		/// </summary>
		public void Bind()
		{
			GL.BindVertexArray(vertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
		}

		/// <summary>
		/// Render image, based on <c>vertives</c>, <c>indices</c> and <c>shader</c>.
		/// </summary>
		public void Render(RenderData renderData, OffscreenRenderTarget? offscreenRenderTarget = null, RenderHooks? renderHooks = null)
		{
			renderHooks ??= new();

			if (offscreenRenderTarget != null)
			{
				offscreenRenderTarget.Bind();
			}
			else
			{
				// Render to default framebuffer - onscreen
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
				GL.Viewport(0, 0, window.width, window.height);
			}

			Bind();

			if (renderHooks.bindTextures != null)
			{
				renderHooks.bindTextures();
			}
			else
			{
				if (renderData.textureName != null)
					window.textureAtlases[renderData.textureName].Use();
			}

			GL.BufferData(BufferTarget.ArrayBuffer, renderData.vertices.Count * sizeof(float), renderData.vertices.ToArray(), BufferUsageHint.StreamDraw);
			GL.BufferData(BufferTarget.ElementArrayBuffer, renderData.indices.Count * sizeof(int), renderData.indices.ToArray(), BufferUsageHint.StreamDraw);
			shader.Use();
			// GL.DrawArrays(PrimitiveType.Triangles, 0, triangle.Length / 6);
			GL.DrawElements(PrimitiveType.Triangles, renderData.indices.Count, DrawElementsType.UnsignedInt, 0);
		}
	}
}