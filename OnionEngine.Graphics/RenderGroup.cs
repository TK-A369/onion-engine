using OnionEngine.Core;
using OnionEngine.IoC;

using OpenTK.Graphics.OpenGL4;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Struct storing some data to render.
	/// It has list of vertices, and the order to draw them.
	/// It belongs to given render group.
	/// </summary>
	public struct RenderData
	{
		public List<float> vertices;
		public List<int> indices;
		public string renderGroup;
	}

	/// <summary>
	/// Struct describing vertex attributes.
	/// For example position is usually float*3 or float*2.
	/// </summary>
	public struct VertexAttributeDescriptor
	{
		public VertexAttribPointerType type;
		public int valuesCount;
		public bool normalized;
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
		/// List of vertex attributes descriptors
		/// </summary>
		public List<VertexAttributeDescriptor> vertexAttributesDescriptors = new List<VertexAttributeDescriptor>();

		public string? textureAtlasName = null;

		/// <summary>
		/// If OpenGL buffers were disposed
		/// </summary>
		private bool disposed = false;

		/// <summary>
		/// Vertices data to be rendered
		/// </summary>
		public List<float> vertices = new List<float>();

		/// <summary>
		/// Order of vertices
		/// </summary>
		public List<int> indices = new List<int>();

		[Dependency]
		Window window = default!;

		public RenderGroup(Shader _shader, List<VertexAttributeDescriptor> _vertexAttributesDescriptors, string? _textureAtlasName = null)
		{
			IoCManager.InjectDependencies(this);

			shader = _shader;
			vertexAttributesDescriptors = _vertexAttributesDescriptors;
			textureAtlasName = _textureAtlasName;

			// Generate OpenGL buffers
			vertexArrayObject = GL.GenVertexArray();
			vertexBufferObject = GL.GenBuffer();
			elementBufferObject = GL.GenBuffer();

			// Bind those buffers
			Bind();

			// Configure VAO
			// Firstly, calculate sizes of attributes
			int totalAttributesSizePerVertex = 0;
			List<int> attributesSizes = new List<int>();
			foreach (VertexAttributeDescriptor desc in vertexAttributesDescriptors)
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
			foreach (VertexAttributeDescriptor desc in vertexAttributesDescriptors)
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
		public void Render(OffscreenRenderTarget? offscreenRenderTarget = null)
		{
			Bind();

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

			if (textureAtlasName != null)
				window.textureAtlases[textureAtlasName].Use();

			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StreamDraw);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StreamDraw);
			shader.Use();
			// GL.DrawArrays(PrimitiveType.Triangles, 0, triangle.Length / 6);
			GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
		}
	}
}