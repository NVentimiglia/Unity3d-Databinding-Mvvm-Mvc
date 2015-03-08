// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Databinding.Model
{
    /// <summary>
    ///     Just a simple thread safe collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <value>Version 1.5</value>
    public class ThreadSafeList<T> : IList<T>
    {
        private readonly List<T> _items = new List<T>();

        public ThreadSafeList(IEnumerable<T> items) {
			this.Add(items); 
		}
		
		public ThreadSafeList() {
		}

        public long LongCount
        {
            get
            {
                lock (this._items)
                {
                    return this._items.LongCount();
                }
            }
        }

        public IEnumerator<T> GetEnumerator() { return this.Clone().GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        public void Add(T item)
        {
            if (Equals(default(T), item))
            {
                return;
            }
            lock (this._items)
            {
                this._items.Add(item);
            }
        }

        public Boolean TryAdd(T item)
        {
            try
            {
                if (Equals(default(T), item))
                {
                    return false;
                }
                lock (this._items)
                {
                    this._items.Add(item);
                    return true;
                }
            }
            catch (NullReferenceException) { }
            catch (ObjectDisposedException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (ArgumentException) { }
            return false;
        }

        public void Clear()
        {
            lock (this._items)
            {
                this._items.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this._items)
            {
                return this._items.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this._items)
            {
                this._items.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (this._items)
            {
                return this._items.Remove(item);
            }
        }

        public void RemoveAll(Predicate<T> match)
        {
            lock (this._items)
            {
                this.Where(entity => match(entity)).ToList().ForEach(entity => _items.Remove(entity));
            }
        }

        public int Count
        {
            get
            {
                lock (this._items)
                {
                    return this._items.Count;
                }
            }
        }

        public bool IsReadOnly { get { return false; } }

        public int IndexOf(T item)
        {
            lock (this._items)
            {
                return this._items.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (this._items)
            {
                this._items.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (this._items)
            {
                this._items.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (this._items)
                {
                    return this._items[index];
                }
            }
            set
            {
                lock (this._items)
                {
                    this._items[index] = value;
                }
            }
        }

        /// <summary>
        ///     Add in an enumerable of items.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="asParallel"></param>
        public void Add(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return;
            }
            lock (_items)
            {
                _items.AddRange(collection.Where(arg => !Equals(default(T), arg)));
            }
        }
        

        /// <summary>
        ///     Returns a new copy of all items in the <see cref="List{T}" />.
        /// </summary>
        /// <returns></returns>
        public List<T> Clone()
        {
            lock (_items)
            {
                return new List<T>(this._items);
            }
        }
    }
}