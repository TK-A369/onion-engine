using OnionEngine.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine
{
	public class Game : GameWindow
	{
		private bool disposed;
		private int vertexArrayObject;
		private int vertexBufferObject;

		private Shader? basicShader;

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

				if (basicShader != null)
					basicShader.Dispose();

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

			basicShader = new Shader("Resources/basic_shader.vert", "Resources/basic_shader.frag");
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

			(basicShader ?? throw new NullReferenceException()).Use();
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