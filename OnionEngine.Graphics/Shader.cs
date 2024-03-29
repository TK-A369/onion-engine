using OnionEngine.DataTypes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine.Graphics
{
	/// <summary>
	/// Class managing pair of vertex shader and fragment shader.
	/// </summary>
	public class Shader : IDisposable
	{
		/// <summary>
		/// List of vertex attributes descriptors.
		/// </summary>
		public List<VertexAttributeDescriptor> vertexAttributesDescriptors = new();

		public readonly int vertexDescriptorSize = 0;

		private int handle;

		private bool disposedValue = false;

		public Shader(string vertexPath, string fragmentPath, List<VertexAttributeDescriptor> _vertexAttributesDescriptors)
		{
			vertexAttributesDescriptors = _vertexAttributesDescriptors;

			// Read shaders codes from files
			string vertexShaderSource = File.ReadAllText(System.IO.Path.Join(
				System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
				vertexPath));
			string fragmentShaderSource = File.ReadAllText(System.IO.Path.Join(
				System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
				fragmentPath));

			// Create shaders
			int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, vertexShaderSource);

			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, fragmentShaderSource);

			// Compile shaders
			GL.CompileShader(vertexShader);
			GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
			if (success == 0)
			{
				string infoLog = GL.GetShaderInfoLog(vertexShader);
				Console.WriteLine(infoLog);
			}

			GL.CompileShader(fragmentShader);
			GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
			if (success == 0)
			{
				string infoLog = GL.GetShaderInfoLog(fragmentShader);
				Console.WriteLine(infoLog);
			}

			// Link shaders
			handle = GL.CreateProgram();
			GL.AttachShader(handle, vertexShader);
			GL.AttachShader(handle, fragmentShader);
			GL.LinkProgram(handle);
			GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out success);
			if (success == 0)
			{
				string infoLog = GL.GetProgramInfoLog(handle);
				Console.WriteLine(infoLog);
			}

			// Cleanup
			GL.DetachShader(handle, vertexShader);
			GL.DetachShader(handle, fragmentShader);
			GL.DeleteShader(fragmentShader);
			GL.DeleteShader(vertexShader);

			foreach (VertexAttributeDescriptor desc in vertexAttributesDescriptors)
				vertexDescriptorSize += desc.valuesCount;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				GL.DeleteProgram(handle);

				disposedValue = true;
			}
		}

		~Shader()
		{
			if (disposedValue == false)
			{
				Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
			}
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Use()
		{
			GL.UseProgram(handle);
		}

		public void SetUniform1i(string name, int value)
		{
			int uniformLocation = GL.GetUniformLocation(handle, name);
			GL.Uniform1(uniformLocation, value);
		}

		public void SetUniform1f(string name, float value)
		{
			int uniformLocation = GL.GetUniformLocation(handle, name);
			GL.Uniform1(uniformLocation, value);
		}

		public void SetUniform2f(string name, float valueX, float valueY)
		{
			int uniformLocation = GL.GetUniformLocation(handle, name);
			GL.Uniform2(uniformLocation, valueX, valueY);
		}

		public void SetUniformMat3f(string name, Mat<float> matrix)
		{
			int uniformLocation = GL.GetUniformLocation(handle, name);
			GL.UniformMatrix3(uniformLocation, 1, true, matrix.values);
		}
	}
}
