using OnionEngine.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine
{
	public class Game : GameWindow
	{
		private bool disposed;
		private int vertex_array_object;
		private int vertex_buffer_object;
		private int program;

		public Game(int width, int height, string title)
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
				GL.DeleteProgram(program);
				GL.DeleteBuffer(vertex_buffer_object);
				GL.DeleteVertexArray(vertex_array_object);
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

			vertex_array_object = GL.GenVertexArray();
			vertex_buffer_object = GL.GenBuffer();

			GL.BindVertexArray(vertex_array_object);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer_object);
			GL.BufferData(BufferTarget.ArrayBuffer, triangle.Length * sizeof(float), triangle, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);

			string vertex_shader_code = @"#version 330 core
            layout (location = 0) in vec4 a_pos;
            layout (location = 1) in vec4 a_color;
      
            out vec4 v_color;

            void main()
            {
                v_color     = a_color;
                gl_Position = a_pos; 
            }";

			string fragment_shader_code = @"#version 330 core
            out vec4 frag_color;
            in  vec4 v_color;
      
            void main()
            {
                frag_color = v_color; 
            }";

			int vertex_shader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertex_shader, vertex_shader_code);
			GL.CompileShader(vertex_shader);
			string info_log_vertex = GL.GetShaderInfoLog(vertex_shader);
			if (!string.IsNullOrEmpty(info_log_vertex))
				Console.WriteLine(info_log_vertex);

			int fragment_shader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragment_shader, fragment_shader_code);
			GL.CompileShader(fragment_shader);
			string info_log_fragment = GL.GetShaderInfoLog(fragment_shader);
			if (!string.IsNullOrEmpty(info_log_fragment))
				Console.WriteLine(info_log_fragment);

			program = GL.CreateProgram();
			GL.AttachShader(program, vertex_shader);
			GL.AttachShader(program, fragment_shader);
			GL.LinkProgram(program);
			string info_log_program = GL.GetProgramInfoLog(program);
			if (!string.IsNullOrEmpty(info_log_program))
				Console.WriteLine(info_log_program);
			GL.DetachShader(program, vertex_shader);
			GL.DetachShader(program, fragment_shader);
			GL.DeleteShader(vertex_shader);
			GL.DeleteShader(fragment_shader);

			GL.UseProgram(program);
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

			GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

			Context.SwapBuffers();
			base.OnUpdateFrame(e);
		}
	}

	class RenderComponent : Component
	{

	}
	class RigidBodyComponent : Component
	{

	}
	class CollidableComponent : Component
	{

	}

	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");

			GameManager gameManager = new GameManager();
			gameManager.debugMode = true;

			Int64 entity1 = gameManager.AddEntity("entity1");
			Console.Write("New entity has id ");
			Console.WriteLine(entity1);

			Int64 renderComponent = gameManager.AddComponent(new RenderComponent());
			Console.Write("New RenderComponent has id ");
			Console.WriteLine(renderComponent);

			Int64 rigidBodyComponent = gameManager.AddComponent(new RigidBodyComponent());
			Console.Write("New RigidBodyComponent has id ");
			Console.WriteLine(rigidBodyComponent);

			Int64 collidableComponent = gameManager.AddComponent(new CollidableComponent());
			Console.Write("New CollidableComponent has id ");
			Console.WriteLine(collidableComponent);

			gameManager.RemoveComponent(rigidBodyComponent);

			foreach (Int64 entity in gameManager.QueryEntitiesOwningComponents(new HashSet<Type> { typeof(RenderComponent), typeof(CollidableComponent) }))
			{
				Console.Write("Entity ");
				Console.Write(entity);
				Console.WriteLine(" matches query");
			}

			using (Game game = new Game(800, 600, "Onion engine demo"))
			{
				game.Run();
			}
		}
	}
}