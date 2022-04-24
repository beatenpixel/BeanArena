using System;
using System.Collections.Concurrent;
using System.Threading;

namespace CrewNetwork.Utils {

    public class ObjectPool<T> {

        private int numberCreated;
        public int NumberCreated => numberCreated;

        private readonly ConcurrentBag<T> pool = new ConcurrentBag<T>();
        private readonly ConcurrentDictionary<T, bool> inuse = new ConcurrentDictionary<T, bool>();

        private readonly Func<T> objectFactory;

        public ObjectPool(Func<T> objectFactory, int prewarmCount) {
            this.objectFactory = objectFactory;
            for (int i = 0; i < prewarmCount; i++) {
                CreateNew();
            }
        }

        public T Get() {
            if (!pool.TryTake(out T item)) {
                item = CreateNew();
            }

            if (!inuse.TryAdd(item, true)) {
                throw new Exception("Duplicate pull " + typeof(T).Name);
            }

            return item;
        }

        public void Return(T item) {
            if (inuse.TryRemove(item, out bool b)) {
                pool.Add(item);
            }
        }

        private T CreateNew() {
            Interlocked.Increment(ref numberCreated);
            return objectFactory.Invoke();
        }

    }

}
