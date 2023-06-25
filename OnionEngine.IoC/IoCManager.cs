using OnionEngine.Core;

namespace OnionEngine.IoC
{
	public static class IoCManager
	{
		public static Dictionary<Type, WeakReference> instances = new Dictionary<Type, WeakReference>();

		/// <summary>
		/// Get registered instance of given type.
		/// </summary>
		/// <typeparam name="T">Type of instance to find</typeparam>
		/// <returns>If found, returns the instance, otherwise returns null.</returns>
		public static T? GetInstanceOfType<T>() where T : class
		{
			if (instances.ContainsKey(typeof(T)))
			{
				WeakReference weakRef = instances[typeof(T)];
				if (weakRef.IsAlive)
				{
					if (weakRef.Target is T instance)
					{
						return instance;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Register given instance as dependency of given type.
		/// </summary>
		/// <typeparam name="T">Type of instance</typeparam>
		/// <param name="instance">Instance to register</param>
		/// <param name="asType">Additional type (possibly abstract class or interface) to register instance as</param>
		public static void RegisterInstance<T>(T instance, Type asType) where T : class
		{
			instances[typeof(T)] = new WeakReference(instance);
			instances[asType] = new WeakReference(instance);

			if (GameManager.debugMode)
			{
				Console.WriteLine("Registered instance of " + typeof(T).FullName + " as " + asType.FullName);
			}
		}

		/// <summary>
		/// Register given instance as dependency of given type.
		/// </summary>
		/// <typeparam name="T">Type of instance</typeparam>
		/// <param name="instance">Instance to register</param>
		public static void RegisterInstance<T>(T instance) where T : class
		{
			RegisterInstance<T>(instance, typeof(T));
		}

		public static T CreateInstance<T>(object[] args, bool injectDependencies = true) where T : class
		{
			T instance = (T)(Activator.CreateInstance(typeof(T), args) ?? throw new Exception("Error occurred when creating instance of type " + typeof(T).FullName));

			return instance;
		}
	}

	/// <summary>
	/// Attribute used to mark dependency of class that should be injected by IoC.
	/// </summary>
	public class DependencyAttribute : Attribute { }
}