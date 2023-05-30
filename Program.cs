using OnionEngine.Core;
using OnionEngine.Graphics;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine
{
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

			using (Window win = new Window(800, 600, "Onion engine demo", gameManager))
			{
				win.Run();
			}
		}
	}
}