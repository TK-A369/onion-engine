namespace OnionEngine.Core
{
	public class Event<TArg>
	{
		HashSet<WeakReference> subscribers = new HashSet<WeakReference>();

		public void Fire(TArg arg)
		{
			HashSet<WeakReference> toBeDeleted = new HashSet<WeakReference>();
			foreach (WeakReference subscriberWeakRef in subscribers)
			{
				if (subscriberWeakRef.IsAlive)
				{
					object obj = subscriberWeakRef.Target!;
					EventSubscriber<TArg> subscriber = (EventSubscriber<TArg>)obj;
					subscriber(arg);
				}
				else
				{
					toBeDeleted.Add(subscriberWeakRef);
				}
			}
			foreach (WeakReference toBeDeletedWeakRef in toBeDeleted)
			{
				Console.WriteLine("Removing event subscriber");
				subscribers.Remove(toBeDeletedWeakRef);
			}
		}
		public void RegisterSubscriber(EventSubscriber<TArg> subscriber)
		{
			subscribers.Add(new WeakReference(subscriber));
		}
		public void UnregisterSubscriber(EventSubscriber<TArg> subscriber)
		{
			subscribers.RemoveWhere((WeakReference wr) => ((EventSubscriber<TArg>)wr.Target!) == subscriber);
		}
	}

	public delegate void EventSubscriber<TArg>(TArg arg);
}