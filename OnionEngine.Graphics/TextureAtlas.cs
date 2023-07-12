using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OnionEngine.Graphics
{
	public class TextureAtlas
	{
		public int size;

		public byte[] textureAtlasData;

		int textureHandle;

		private struct Rectangle
		{
			public Int64 startX, startY, width, height;
		}

		public TextureAtlas(int _size, List<string> _texturesPaths)
		{
			size = _size;

			SortedSet<Rectangle> freeRectangles = new SortedSet<Rectangle>(
				Comparer<Rectangle>.Create((a, b) =>
				{
					int comp = Math.Min(a.width, a.height).CompareTo(Math.Min(b.width, b.height));
					if (comp == 0)
					{
						return a.GetHashCode().CompareTo(b.GetHashCode());
					}
					return comp;
				}))
				{
					new Rectangle() { startX = 0, startY = 0, width = size, height = size }
				};

			textureAtlasData = new byte[size * size * 4];

			foreach (string texturePath in _texturesPaths)
			{
				StbImage.stbi_set_flip_vertically_on_load(1);
				ImageResult image = ImageResult.FromStream(File.OpenRead(texturePath), ColorComponents.RedGreenBlueAlpha);

				bool foundFreeRectangle = false;
				foreach (Rectangle rectangle in freeRectangles)
				{
					if (image.Width <= rectangle.width && image.Height <= rectangle.height)
					{
						for (Int64 i = 0; i < image.Height; i++)
						{
							for (Int64 j = 0; j < image.Width; j++)
							{
								for (Int64 k = 0; k < 4; k++)
								{
									textureAtlasData[((rectangle.startY + i) * size * 4) + ((rectangle.startX + j) * 4) + k] =
										image.Data[(i * image.Width * 4) + (j * 4) + k];
								}
							}
						}

						freeRectangles.Remove(rectangle);

						Int64 residueX = rectangle.width - image.Width;
						Int64 residueY = rectangle.height - image.Height;
						Console.WriteLine("Residue: " + residueX + ", " + residueY);

						if (residueX > 0)
						{
							freeRectangles.Add(new Rectangle() { startX = rectangle.startX + image.Width, startY = rectangle.startY, width = residueX, height = image.Height });
						}
						if (residueY > 0)
						{
							freeRectangles.Add(new Rectangle() { startX = rectangle.startX, startY = rectangle.startY + image.Height, width = rectangle.width, height = residueY });
						}

						foundFreeRectangle = true;
						break;
					}
				}

				Console.WriteLine("Free rectangles:");
				foreach (Rectangle r in freeRectangles)
					Console.WriteLine(r.startX + ", " + r.startY + ", " + r.width + ", " + r.height);
				Console.WriteLine();

				if (!foundFreeRectangle)
				{
					throw new Exception("Could not find a free rectangle in the texture atlas - consider enlarging it or creating a new one");
				}
			}

			textureHandle = GL.GenTexture();
			Use();
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, size, size, 0, PixelFormat.Rgba, PixelType.UnsignedByte, textureAtlasData);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		}

		public void Use(TextureUnit unit = TextureUnit.Texture0)
		{
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, textureHandle);
		}
	}
}