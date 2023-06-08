using OnionEngine.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine.Graphics
{
	public struct RenderData
	{
		public List<float> vertices;
		public List<int> indices;
		public string renderGroup;
	}

	public struct VertexAttributeDescriptor
	{
		public VertexAttribPointerType type;
		public int valuesCount;
		public bool normalized;
	}

	class RenderGroup : IDisposable
	{
		// OpenGL stuff
		public int vertexArrayObject;
		public int vertexBufferObject;
		public int elementBufferObject;

		// Shader object
		public Shader shader;

		// Vertex attributes descriptors
		public List<VertexAttributeDescriptor> vertexAttributesDescriptors = new List<VertexAttributeDescriptor>();

		// If OpenGL buffers were disposed
		private bool disposed = false;

		// Data to be rendered
		public List<float> vertices = new List<float>();
		public List<int> indices = new List<int>();

		public RenderGroup(Shader _shader, List<VertexAttributeDescriptor> _vertexAttributesDescriptors)
		{
			shader = _shader;
			vertexAttributesDescriptors = _vertexAttributesDescriptors;

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

		public void Bind()
		{
			GL.BindVertexArray(vertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
		}

		public void Render()
		{
			Bind();

			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StreamDraw);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StreamDraw);
			shader.Use();
			// GL.DrawArrays(PrimitiveType.Triangles, 0, triangle.Length / 6);
			GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
		}
	}

	class Window : GameWindow
	{
		// OpenGL stuff
		private bool disposed = false;
		private int vertexArrayObject;
		private int vertexBufferObject;

		// Shaders
		Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

		// Render groups
		Dictionary<string, RenderGroup> renderGroups = new Dictionary<string, RenderGroup>();

		GameManager gameManager;

		public Window(int width, int height, string title, GameManager _gameManager)
			: base(
				new GameWindowSettings()
				{
					RenderFrequency = 30,
					UpdateFrequency = 30
				},
				new NativeWindowSettings()
				{
					Size = new OpenTK.Mathematics.Vector2i(width, height),
					Title = title,
					Vsync = VSyncMode.On
				})
		{
			gameManager = _gameManager;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !disposed)
			{
				disposed = true;

				foreach (Shader shader in shaders.Values)
					shader.Dispose();

				foreach (RenderGroup renderGroup in renderGroups.Values)
					renderGroup.Dispose();

				GL.DeleteBuffer(vertexBufferObject);
				GL.DeleteVertexArray(vertexArrayObject);
			}
			base.Dispose(disposing);
		}

		protected override void OnLoad()
		{
			// Equilateral triangle https://en.wikipedia.org/wiki/Equilateral_triangle
			float[] triangle =
			{
            // x        y      z      r     g     b    
              -0.866f, -0.75f, 0.0f,  1.0f, 0.0f, 0.0f,
			   0.866f, -0.75f, 0.0f,  1.0f, 1.0f, 0.0f,
			   0.0f,    0.75f, 0.0f,  0.0f, 0.0f, 1.0f,
			};

			vertexArrayObject = GL.GenVertexArray();
			vertexBufferObject = GL.GenBuffer();

			GL.BindVertexArray(vertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, triangle.Length * sizeof(float), triangle, BufferUsageHint.DynamicDraw);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);

			shaders["basic-shader"] = new Shader("Resources/basic_shader.vert", "Resources/basic_shader.frag");

			renderGroups = new Dictionary<string, RenderGroup>()
			{
				["basic-group"] = new RenderGroup(
					shaders["basic-shader"],
					new List<VertexAttributeDescriptor>()
					{
						new VertexAttributeDescriptor() { type = VertexAttribPointerType.Float, valuesCount = 3, normalized = false },
						new VertexAttributeDescriptor() { type = VertexAttribPointerType.Float, valuesCount = 3, normalized = false }
					})
			};
		}


		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
			GL.Viewport(0, 0, e.Width, e.Height);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			// renderGroups["basic-group"].vertices[18] += (float)e.Time * 0.025f;
			// renderGroups["basic-group"].vertices[19] += (float)e.Time * 0.05f;

			base.OnUpdateFrame(e);
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			// Clear render groups' vertices data
			foreach (RenderGroup renderGroup in renderGroups.Values)
			{
				renderGroup.vertices.Clear();
				renderGroup.indices.Clear();
			}

			// Add vertices to appropriate render groups
			HashSet<Int64> entitiesToRender = gameManager.QueryEntitiesOwningComponents(new HashSet<Type>() { typeof(RenderComponent) });
			foreach (Int64 entity in entitiesToRender)
			{
				Int64 renderComponentId = gameManager.GetComponent(entity, typeof(RenderComponent));
				RenderComponent renderComponent = (gameManager.components[renderComponentId] as RenderComponent) ?? throw new NullReferenceException();
				List<RenderData> dataToRender = renderComponent.GetVertices();
				foreach (RenderData renderData in dataToRender)
				{
					RenderGroup renderGroup = renderGroups[renderData.renderGroup];
					int indexOffset = renderGroup.vertices.Count / 6;
					foreach (float vertex in renderData.vertices)
					{
						renderGroup.vertices.Add(vertex);
					}
					foreach (int index in renderData.indices)
					{
						renderGroup.indices.Add(index + indexOffset);
					}
				}
			}

			// Clear
			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			shaders["basic-shader"].Use();
			GL.BindVertexArray(vertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

			foreach (RenderGroup renderGroup in renderGroups.Values)
			{
				renderGroup.Render();
			}

			Context.SwapBuffers();

			base.OnRenderFrame(args);
		}
	}
}
