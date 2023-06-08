using OnionEngine.Core;
using OnionEngine.Graphics;
using OnionEngine.Physics;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OnionEngine
{
	[Component]
	class CollidableComponent : Component
	{

	}

	[EntitySystem]
	class TestEntitySystem : EntitySystem
	{
		[EntitySystemDependency]
		public RigidBodyComponent? rigidBodyComponent = default!;

		public override void OnCreate()
		{
			Console.WriteLine("Creating TestEntitySystem!");

			base.OnCreate();
		}

		public override void OnDestroy()
		{
			Console.WriteLine("Destroying TestEntitySystem!");

			base.OnDestroy();
		}
	}

	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");

			// Create GameManager
			GameManager gameManager = new GameManager();
			GameManager.debugMode = true;

			// Register component types
			gameManager.AutoRegisterComponentTypes();
			Console.WriteLine();

			// Register entity systems
			gameManager.AutoRegisterEntitySystemTypes();
			Console.WriteLine();

			// Load prototypes
			gameManager.prototypeManager.LoadPrototypes(File.ReadAllText("Resources/Prototypes/Test1.xml"));
			Console.WriteLine();

			// Create and remove some entities and components
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

			Console.WriteLine();
			Console.WriteLine(gameManager.DumpEntitiesAndComponents());

			gameManager.RemoveComponent(rigidBodyComponent);

			Console.WriteLine();

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