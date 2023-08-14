using OnionEngine.Core;
using OnionEngine.IoC;
using OnionEngine.Prototypes;
using OnionEngine.UserInterface;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine.Graphics
{
	public delegate void RenderDelegate();

	/// <summary>
	/// Game window, rendering graphics and executing logic.
	/// </summary>
	public class Window : GameWindow
	{
		/// <summary>
		/// Dimensions of the window.
		/// </summary>
		public int width, height;

		/// <summary>
		/// This event will be fired after window has been loaded.
		/// OpenGL, texture atlases and shaders will be ready before this event is fired.
		/// </summary>
		public Event<object?> afterLoadEvent = new();

		/// <summary>
		/// This event is fired every render frame.
		/// Appropriate entity systems should update <c>renderData</c> field of <c>RenderComponent</c> when this event is fired.
		/// </summary>
		public Event<object?> drawSpritesEvent = new();

		/// <summary>
		/// This event is fired every update frame.
		/// </summary>
		public Event<object?> updateFrameEvent = new();

		public Event<object?> windowResizeEvent = new();

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
		public int vertexArrayObject;

		/// <summary>
		/// Handle to VBO.
		/// It stores informations about vertices
		/// </summary>
		/// <remarks>
		/// To be deleted. Instead, render groups will be used.
		/// </remarks>
		public int vertexBufferObject;

		/// <summary>
		/// Dictionary of shaders by their names.
		/// </summary>
		public Dictionary<string, Shader> shaders = new();

		/// <summary>
		/// Dictionary of render groups by their names.
		/// </summary>
		public Dictionary<string, RenderGroup> renderGroups = new();

		/// <summary>
		/// Dictionary of texture atlases by their names.
		/// </summary>
		public Dictionary<string, TextureAtlas> textureAtlases = new();

		/// <summary>
		/// Dictionary of offscreen render targets by their names.
		/// </summary>
		public Dictionary<string, OffscreenRenderTarget> offscreenRenderTargets = new();

		/// <summary>
		/// This callback in called during render frame.
		/// </summary>
		public Action? renderCallback = null;

		[Dependency]
		private PrototypeManager prototypeManager = default!;

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
			textureAtlases.Add("texture-atlas-test", new TextureAtlas(128, new Dictionary<string, string>() {
				{"human-1","Resources/Textures/human-1.png"},
				{"floor-tile-1","Resources/Textures/floor-tile-1.png"},
				{"smiling-ball-1","Resources/Textures/smiling-ball-1.png"},
				{"smiling-ball-2","Resources/Textures/smiling-ball-1.png"},
				{"floor-tile-2","Resources/Textures/floor-tile-1.png"},
				{"human-2","Resources/Textures/human-1.png"},
				{"human-3","Resources/Textures/human-1.png"},
				{"human-4","Resources/Textures/human-1.png"},
				{"floor-tile-3","Resources/Textures/floor-tile-1.png"},
				{"floor-tile-4","Resources/Textures/floor-tile-1.png"}
			}));

			// Create texture atlases based on prototypes
			foreach (var (_, prototype) in prototypeManager.GetPrototypesOfType<TextureAtlasPrototype>())
			{
				textureAtlases.Add(prototype.name, new TextureAtlas(prototype.size, prototype.textures));
			}

			// Create offscreen render targets based on prototypes
			foreach (var (_, prototype) in prototypeManager.GetPrototypesOfType<OffscreenRenderTargetPrototype>())
			{
				offscreenRenderTargets.Add(
					prototype.name, new OffscreenRenderTarget(prototype.width, prototype.height));
			}

			// Create shaders based on prototypes
			foreach (var (_, prototype) in prototypeManager.GetPrototypesOfType<ShaderPrototype>())
			{
				shaders.Add(
					prototype.name, new Shader(prototype.vertexPath, prototype.fragmentPath, prototype.vertexAttributeDescriptors));
			}

			// Create render groups based on prototypes
			foreach (var (_, prototype) in prototypeManager.GetPrototypesOfType<RenderGroupPrototype>())
			{
				renderGroups.Add(prototype.name, IoCManager.CreateInstance<RenderGroup>(new object?[] {
					shaders[prototype.shaderName] }));
			}

			float[] vertexData =
			{
            // x        y      z      r     g     b      texX,  texY
              -0.75f,  -0.75f, 0.0f,  0.0f, 0.0f, 0.0f,  0.0f,  0.0f,
			   0.75f,  -0.75f, 0.0f,  0.0f, 0.0f, 0.0f,  1.0f,  0.0f,
			   0.75f,   0.75f, 0.0f,  0.0f, 0.0f, 0.0f,  1.0f,  1.0f,
			  -0.75f,  -0.75f, 0.0f,  0.0f, 0.0f, 0.0f,  0.0f,  0.0f,
			  -0.75f,   0.75f, 0.0f,  0.0f, 0.0f, 0.0f,  0.0f,  1.0f,
			   0.75f,   0.75f, 0.0f,  0.0f, 0.0f, 0.0f,  1.0f,  1.0f,
			};

			vertexArrayObject = GL.GenVertexArray();
			vertexBufferObject = GL.GenBuffer();

			GL.BindVertexArray(vertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.DynamicDraw);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
			GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);

			afterLoadEvent.Fire(null);
		}


		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			width = e.Width;
			height = e.Height;

			GL.Viewport(0, 0, e.Width, e.Height);

			windowResizeEvent.Fire(null);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			// renderGroups["render-group-basic"].vertices[18] += (float)e.Time * 0.025f;
			// renderGroups["render-group-basic"].vertices[19] += (float)e.Time * 0.05f;

			updateFrameEvent.Fire(null);

			base.OnUpdateFrame(e);
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			renderCallback?.Invoke();

			base.OnRenderFrame(args);
		}

		public List<RenderData> OptimizeRenderDataList(List<RenderData> renderDataList)
		{
			Dictionary<(string, string?), RenderData> renderDataDictionary = new();

			foreach (RenderData renderData in renderDataList)
			{
				(string, string?) groupAndAtlasName = (renderData.renderGroup, renderData.textureAtlasName);
				if (!renderDataDictionary.ContainsKey(groupAndAtlasName))
				{
					renderDataDictionary[groupAndAtlasName] = new()
					{
						vertices = new(),
						indices = new(),
						renderGroup = renderData.renderGroup,
						textureAtlasName = renderData.textureAtlasName
					};
				}

				int indexOffset = renderDataDictionary[groupAndAtlasName].vertices.Count
					/ renderGroups[renderData.renderGroup].shader.vertexDescriptorSize;
				renderDataDictionary[groupAndAtlasName].vertices.AddRange(renderData.vertices);
				foreach (int index in renderData.indices)
				{
					renderDataDictionary[groupAndAtlasName].indices.Add(index + indexOffset);
				}
			}

			return renderDataDictionary.Values.ToList();
		}
	}
}
