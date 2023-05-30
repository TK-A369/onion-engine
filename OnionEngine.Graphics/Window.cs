using OnionEngine.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine.Graphics
{
	class Window : GameWindow
	{
		private bool disposed;
		private int vertexArrayObject;
		private int vertexBufferObject;

		Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

		public Window(int width, int height, string title)
			: base(
				new GameWindowSettings(),
				new NativeWindowSettings()
				{
					Size = new OpenTK.Mathematics.Vector2i(width, height),
					Title = title
				})
		{ }

		protected override void Dispose(bool disposing)
		{
			if (disposing && !disposed)
			{
				disposed = true;

				foreach (Shader shader in shaders.Values)
					shader.Dispose();

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
		}


		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
			GL.Viewport(0, 0, e.Width, e.Height);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			shaders["basic-shader"].Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

			Context.SwapBuffers();
			base.OnUpdateFrame(e);
		}
	}
}
