using OpenTK.Graphics.OpenGL4;

namespace OnionEngine.Graphics
{
	public class OffscreenRenderTarget : IDisposable
	{
		int framebuffer;
		int texture;
		int depthRenderbuffer;

		public int width, height;

		/// <summary>
		/// If OpenGL buffers were disposed
		/// </summary>
		private bool disposed = false;

		public OffscreenRenderTarget(int width, int height)
		{
			this.width = width;
			this.height = height;

			framebuffer = GL.GenFramebuffer();
			FramebufferErrorCode error = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

			texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (nint)null);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

			depthRenderbuffer = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRenderbuffer);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthRenderbuffer);
			GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture, 0);
			GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

			if (error != FramebufferErrorCode.FramebufferComplete)
				throw new Exception("Error occurred during framebuffer creation: " + error);
		}

		~OffscreenRenderTarget()
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
				GL.DeleteFramebuffer(framebuffer);

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
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
			GL.Viewport(0, 0, width, height);
		}

		public void Clear(float r = 0.0f, float g = 0.0f, float b = 0.0f)
		{
			Bind();
			GL.ClearColor(r, g, b, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		public int GetTextureId() => texture;

		public void UseTexture(TextureUnit unit = TextureUnit.Texture0)
		{
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, GetTextureId());
		}
	}
}