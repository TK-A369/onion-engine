namespace OnionEngine.Core
{
	public class Event<TArg>
	{
		private HashSet<WeakReference> subscribers = new();

		public void Fire(TArg arg)
		{
			HashSet<WeakReference> toBeDeleted = new();
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