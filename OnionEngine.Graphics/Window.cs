using OnionEngine.Core;
using OnionEngine.IoC;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Game window, rendering graphics and executing logic.
	/// </summary>
	public class Window : GameWindow
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

		public int width, height;

		public Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

		/// <summary>
		/// <c>GameManager</c> object used by this window.
		/// </summary>
		[Dependency]
		GameManager gameManager = default!;

		public Window(int _width, int _height, string title)
			: base(
				new GameWindowSettings()
				{
					RenderFrequency = 30,
					UpdateFrequency = 30
				},
				new NativeWindowSettings()
				{
					Size = new OpenTK.Mathematics.Vector2i(_width, _height),
					Title = title,
					Vsync = VSyncMode.On
				})
		{
			width = _width;
			height = _height;

			IoCManager.RegisterInstance(this);
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
