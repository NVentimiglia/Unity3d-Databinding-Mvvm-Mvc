// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Collections.Generic;

namespace Foundation.Databinding.Model
{
    /// <summary>
    /// A thread safe implementation of a dictionary that allows reads to be done
    /// even when writes are being done. Writes are serialized, though.
    /// </summary>
    public sealed class ThreadSafeDictionary<T, K> : IEnumerable<KeyValuePair<T, K>>, IDictionary<T, K>
    {
        readonly Dictionary<T, K> _items;
        readonly object _sync = new object();

        public ThreadSafeDictionary()
        {
            _items = new Dictionary<T, K>();
        }

        public ThreadSafeDictionary(Dictionary<T, K> inner)
        {
            _items = new Dictionary<T, K>(inner);
        }


        /// <summary>
        ///     Returns a new copy of all items in the <see cref="List{T}" />.
        /// </summary>
        /// <returns></returns>
        public Dictionary<T, K> Clone()
        {
            lock (_sync)
            {
                return new Dictionary<T, K>(_items);
            }
        }

        public IEnumerator<KeyValuePair<T, K>> GetEnumerator()
        {
            return this.Clone().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(T key, K value)
        {
            lock (_sync)
            {
                _items.Add(key, value);
            }
        }

        public bool ContainsKey(T key)
        {
            lock (_sync)
            {
                return _items.ContainsKey(key);
            }
        }

        public ICollection<T> Keys
        {
            get
            {
                lock (_sync)
                {
                    return _items.Keys;
                }
            }
        }

        public bool Remove(T key)
        {
            lock (_sync)
            {
                return _items.Remove(key);
            }
        }

        public bool TryGetValue(T key, out K value)
        {
            lock (_sync)
            {
                return _items.TryGetValue(key, out value);
            }
        }

        public ICollection<K> Values
        {
            get
            {
                lock (_sync)
                {
                    return _items.Values;
                }
            }
        }

        public K this[T key]
        {
            get
            {
                lock (_sync)
                {
                    return _items[key];
                }
            }
            set
            {
                lock (_sync)
                {
                    _items[key] = value;
                }
            }
        }

        public void Add(KeyValuePair<T, K> item)
        {
            lock (_sync)
            {
                _items.Add(item.Key, item.Value);
            }
        }

        public void Clear()
        {
            lock (_sync)
            {
                _items.Clear();
            }
        }

        public bool Contains(KeyValuePair<T, K> item)
        {
            lock (_sync)
            {
               return _items.ContainsKey(item.Key);
            }
        }

        public void CopyTo(KeyValuePair<T, K>[] array, int arrayIndex)
        {
            throw new NotImplementedException("");
        }

        public int Count
        {
            get
            {
                lock (_items)
                {
                    return _items.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<T, K> item)
        {
            lock (_sync)
            {
                return _items.Remove(item.Key);
            }
        }
    }
}
