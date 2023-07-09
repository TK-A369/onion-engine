using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OnionEngine.Graphics
{
	public class Texture
	{
		int handle;

		public Texture(string texturePath)
		{
			handle = GL.GenTexture();
			Use();

			StbImage.stbi_set_flip_vertically_on_load(1);
			ImageResult image = ImageResult.FromStream(File.OpenRead(texturePath), ColorComponents.RedGreenBlueAlpha);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

			// GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			// GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		public void Use(TextureUnit unit = TextureUnit.Texture0)
		{
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, handle);
		}
	}
}