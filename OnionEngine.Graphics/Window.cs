using OnionEngine.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine.Graphics
{
	struct RenderData
	{
		public List<float> vertices;
		public List<int> indices;
		public string renderGroup;
	}

	class RenderGroup : IDisposable
	{
		// OpenGL stuff
		public int vertexArrayObject;
		public int vertexBufferObject;
		public int elementBufferObject;

		public Shader shader;

		// If OpenGL buffers were disposed
		private bool disposed = false;

		// Data to be rendered
		public List<float> vertices = new List<float>();
		public List<int> indices = new List<int>();

		public RenderGroup(Shader _shader)
		{
			shader = _shader;

			// Generate OpenGL buffers
			vertexArrayObject = GL.GenVertexArray();
			vertexBufferObject = GL.GenBuffer();
			elementBufferObject = GL.GenBuffer();

			// Bind those buffers
			Bind();

			// Configure VAO
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
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
				new GameWindowSettings(),
				new NativeWindowSettings()
				{
					Size = new OpenTK.Mathematics.Vector2i(width, height),
					Title = title
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

			// string vertexShaderCode = @"#version 330 core
			// layout (location = 0) in vec4 a_pos;
			// layout (location = 1) in vec4 a_color;

			// out vec4 v_color;

			// void main()
			// {
			//     v_color     = a_color;
			//     gl_Position = a_pos; 
			// }";

			// string fragmentShaderCode = @"#version 330 core
			// out vec4 frag_color;
			// in  vec4 v_color;

			// void main()
			// {
			//     frag_color = v_color; 
			// }";

			// int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			// GL.ShaderSource(vertexShader, vertexShaderCode);
			// GL.CompileShader(vertexShader);
			// string info_log_vertex = GL.GetShaderInfoLog(vertexShader);
			// if (!string.IsNullOrEmpty(info_log_vertex))
			// 	Console.WriteLine(info_log_vertex);

			// int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			// GL.ShaderSource(fragmentShader, fragmentShaderCode);
			// GL.CompileShader(fragmentShader);
			// string infoLogFragment = GL.GetShaderInfoLog(fragmentShader);
			// if (!string.IsNullOrEmpty(infoLogFragment))
			// 	Console.WriteLine(infoLogFragment);

			// program = GL.CreateProgram();
			// GL.AttachShader(program, vertexShader);
			// GL.AttachShader(program, fragmentShader);
			// GL.LinkProgram(program);
			// string infoLogProgram = GL.GetProgramInfoLog(program);
			// if (!string.IsNullOrEmpty(infoLogProgram))
			// 	Console.WriteLine(infoLogProgram);
			// GL.DetachShader(program, vertexShader);
			// GL.DetachShader(program, fragmentShader);
			// GL.DeleteShader(vertexShader);
			// GL.DeleteShader(fragmentShader);

			// GL.UseProgram(program);

			shaders["basic-shader"] = new Shader("Resources/basic_shader.vert", "Resources/basic_shader.frag");

			renderGroups = new Dictionary<string, RenderGroup>()
			{
				["basic-group"] = new RenderGroup(shaders["basic-shader"])
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
