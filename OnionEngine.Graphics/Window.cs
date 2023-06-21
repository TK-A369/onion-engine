using OnionEngine.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

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
	class RenderGroup : IDisposable
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

	/// <summary>
	/// Game window, rendering graphics and executing logic.
	/// </summary>
	class Window : GameWindow
	{
		// OpenGL stuff

		/// <summary>
		/// If OpenGL stuff has been disposed.
		/// </summary>
		private bool disposed = false;

		/// <summary>
		/// Handle to VAO.
		/// It stores informations about vertex attributes and their layout.
		/// </summary>
		/// <remarks>
		/// To be deleted. Instead, render groups will be used.
		/// </remarks>
		private int vertexArrayObject;

		/// <summary>
		/// Handle to VBO.
		/// It stores informations about vertices
		/// </summary>
		/// <remarks>
		/// To be deleted. Instead, render groups will be used.
		/// </remarks>
		private int vertexBufferObject;

		/// <summary>
		/// Dictionary of shaders by their names.
		/// </summary>
		Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

		/// <summary>
		/// Dictionary of render groups by their names.
		/// </summary>
		Dictionary<string, RenderGroup> renderGroups = new Dictionary<string, RenderGroup>();

		/// <summary>
		/// <c>GameManager</c> object used by this window.
		/// </summary>
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

			shaders["basic-shader"] = new Shader("Resources/Shaders/basic_shader.vert", "Resources/Shaders/basic_shader.frag");

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
