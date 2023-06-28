using OnionEngine.Core;
using OnionEngine.Graphics;
using OnionEngine.Physics;
using OnionEngine.IoC;
using OnionEngine.Prototypes;

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
		RigidBodyComponent rigidBodyComponent = default!;

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

			// Turn on debug mode
			GameManager.debugMode = true;

			// Create GameManager
			GameManager gameManager = new GameManager();

			// Register component types
			gameManager.AutoRegisterComponentTypes();
			Console.WriteLine();

			// Register entity systems
			gameManager.AutoRegisterEntitySystemTypes();
			Console.WriteLine();

			// Load prototypes
			gameManager.prototypeManager.LoadPrototypes(File.ReadAllText("Resources/Prototypes/Test1.xml"));
			Console.WriteLine();
			gameManager.prototypeManager.entityPrototypes.Add("physical-body", new EntityPrototype("entity1", new List<ComponentPrototype>() {
				new ComponentPrototype("PositionComponent", new Dictionary<string, PrototypeParameter> () {}),
				new ComponentPrototype("PhysicalBodyComponent", new Dictionary<string, PrototypeParameter> () {
					{"mass", new PrototypeParameter(PrototypeParameter.ParameterType.Number, "3.2")}
				})
			}));
			gameManager.prototypeManager.entityPrototypes.Add("entity1", new EntityPrototype("entity1", new List<ComponentPrototype>() {
				new ComponentPrototype("CollidableComponent", new Dictionary<string, PrototypeParameter> () {}),
				new ComponentPrototype("RigidBodyComponent", new Dictionary<string, PrototypeParameter> () {})
			}, new List<string>() { "physical-body" }));

			// Create and remove some entities and components
			Int64 entity1 = gameManager.AddEntity("entity1");
			// Console.Write("New entity has id ");
			// Console.WriteLine(entity1);

			Int64 renderComponent = gameManager.AddComponent(new RenderComponent());
			// Console.Write("New RenderComponent has id ");
			// Console.WriteLine(renderComponent);

			Int64 rigidBodyComponent = gameManager.AddComponent(new RigidBodyComponent());
			// Console.Write("New RigidBodyComponent has id ");
			// Console.WriteLine(rigidBodyComponent);

			Int64 collidableComponent = gameManager.AddComponent(new CollidableComponent());
			// Console.Write("New CollidableComponent has id ");
			// Console.WriteLine(collidableComponent);

			Int64 spawnedEntity = gameManager.prototypeManager.SpawnEntityPrototype("entity1");
			Console.Write("Spawned entity has id ");
			Console.WriteLine(spawnedEntity);
			gameManager.prototypeManager.SpawnEntityPrototype("myent1");

			Console.WriteLine();
			Console.WriteLine(gameManager.DumpEntitiesAndComponents());

			gameManager.RemoveComponent(rigidBodyComponent);

			Console.WriteLine();

			// Console.WriteLine("Mass: " + ((PhysicalBodyComponent)gameManager.components[6]).mass);

			// foreach (Int64 entity in gameManager.QueryEntitiesOwningComponents(new HashSet<Type> { typeof(RenderComponent), typeof(CollidableComponent) }))
			// {
			// 	Console.Write("Entity ");
			// 	Console.Write(entity);
			// 	Console.WriteLine(" matches query");
			// }

			using (Window win = IoCManager.CreateInstance<Window>(new object[] { 800, 600, "Onion engine demo" }))
			{
				win.Run();
			}
		}
	}
}