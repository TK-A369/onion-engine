using System.Reflection;

using OnionEngine.Core;

namespace OnionEngine.IoC
{
	public static class IoCManager
	{
		public static Dictionary<Type, WeakReference> instances = new Dictionary<Type, WeakReference>();

		/// <summary>
		/// Get registered instance of given type.
		/// </summary>
		/// <param name="type">Type of instance to find</param>
		/// <returns>If found, returns the instance, otherwise returns null.</returns>
		public static object? GetInstanceOfType(Type type)
		{
			if (instances.ContainsKey(type))
			{
				WeakReference weakRef = instances[type];
				if (weakRef.IsAlive)
				{
					if ((weakRef.Target ?? throw new Exception("Instance already has been garbage collected")).GetType().IsAssignableTo(type))
					{
						return weakRef.Target;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Get registered instance of given type.
		/// </summary>
		/// <typeparam name="T">Type of instance to find</typeparam>
		/// <returns>If found, returns the instance, otherwise returns null.</returns>
		public static T? GetInstanceOfType<T>() where T : class
		{
			object? instance = GetInstanceOfType(typeof(T));
			if (instance == null)
			{
				return null;
			}
			else
			{
				return (T)instance;
			}
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

		/// <summary>
		/// Inject dependencies into given object.
		/// </summary>
		/// <param name="instance">Object to inject dependencies into</param>
		/// <exception cref="Exception">When dependency hasn't been found</exception>
		public static void InjectDependencies(object instance)
		{
			foreach (FieldInfo fieldInfo in instance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				if (fieldInfo.IsDefined(typeof(DependencyAttribute)))
				{
					fieldInfo.SetValue(instance, GetInstanceOfType(fieldInfo.FieldType) ?? throw new Exception("Couldn't find instance of type " + fieldInfo.FieldType.FullName));
					Console.WriteLine("Dependency injected: " + fieldInfo.FieldType.FullName + " -> " + fieldInfo.Name);
				}
			}
		}

		/// <summary>
		/// Create instance of given type, and then inject dependencies to it.
		/// Dependencies are fields marked with <see cref="DependencyAttribute" />.
		/// </summary>
		/// <typeparam name="T">Type of instance to create</typeparam>
		/// <param name="args">List of arguments to pass to constructor</param>
		/// <param name="injectDependencies">If true, dependencies will be injected</param>
		/// <returns>Instance with dependencies injected</returns>
		/// <exception cref="Exception">When couldn't create instance, or dependency hasn't been found</exception>
		public static T CreateInstance<T>(object[] args, bool injectDependencies = true) where T : class
		{
			// Create instance
			T instance = (T)(Activator.CreateInstance(typeof(T), args) ?? throw new Exception("Error occurred when creating instance of type " + typeof(T).FullName));

			InjectDependencies(instance);

			return instance;
		}
	}

	/// <summary>
	/// Attribute used to mark dependency of class that should be injected by IoC.
	/// </summary>
	public class DependencyAttribute : Attribute { }
}