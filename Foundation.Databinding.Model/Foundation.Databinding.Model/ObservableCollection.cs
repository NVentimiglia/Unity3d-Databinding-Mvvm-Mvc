// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Databinding.Model
{
    /// <summary>
    /// A collection that supports change notification.
    /// Simplified to use objects.
    /// </summary>
    public interface IObservableCollection
    {
        event Action<object> OnObjectAdd;

        event Action<object> OnObjectRemove;

        event Action<int, object> OnObjectInsert;

        event Action OnClear;

        IEnumerable<object> GetObjects();
    }


    /// <summary>
    /// A collection that supports change notification
    /// </summary>
    /// <typeparam name="T">ValueType of a collection item</typeparam>
    public class ObservableCollection<T> : IEnumerable<T>, IObservableCollection
    {

        #region events
        /// <summary>
        /// For data binding
        /// </summary>
        public event Action<object> OnObjectAdd;

        /// <summary>
        /// For data binding
        /// </summary>
        public event Action<int, object> OnObjectInsert;

        /// <summary>
        /// For data binding
        /// </summary>
        public event Action<object> OnObjectRemove;

        public event Action<T> OnAdd;

        public event Action<int, T> OnInsert;


        public event Action<T> OnRemove;

        public event Action OnClear;
        #endregion


        private readonly List<T> _list = new List<T>();


        public ObservableCollection()
        {

        }

        public ObservableCollection(IEnumerable<T> set)
        {
            Add(set);
        }

        /// <summary>
        /// Returns all items cast as object
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetObjects()
        {
            if (_list.Count == 0)
                return new object[0];
            return _list.Cast<object>();
        }


        /// <summary>
        /// Convenience function. I recommend using .buffer instead.
        /// </summary>
        public T this[int i]
        {
            get { return _list[i]; }
            set { _list[i] = value; }
        }

        /// <summary>
        /// Returns 'true' if the specified item is within the list.
        /// </summary>
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Add the specified item to the end of the list.
        /// </summary>
        public void Add(T o)
        {
            _list.Add(o);

            if (OnAdd != null)
                OnAdd(o);

            if (OnObjectAdd != null)
                OnObjectAdd(o);
        }

        /// <summary>
        /// Add the specified items to the end of the list.
        /// </summary>
        public void Add(IEnumerable<T> o)
        {
            var s = o.ToArray();

            for (int i = 0;i < s.Length;i++)
            {
                Add(s[i]);
            }
        }

        /// <summary>
        /// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
        /// </summary>
        public void Remove(T o)
        {
            if (_list.Remove(o))
            {
                if (OnRemove != null)
                    OnRemove(o);

                if (OnObjectRemove != null)
                    OnObjectRemove(o);
            }
        }

        /// <summary>
        /// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
        /// </summary>
        public void Remove(IEnumerable<T> o)
        {
            var s = o.ToArray();

            for (int i = 0;i < s.Length;i++)
            {
                Remove(s[i]);
            }
        }

        /// <summary>
        /// Insert an item at the specified index, pushing the entries back.
        /// </summary>
        public void Insert(int index, T o)
        {
            _list.Insert(index, o);

            if (OnInsert != null)
                OnInsert(index, o);

            if (OnInsert != null)
                OnObjectInsert(index, o);

        }

        /// <summary>
        /// Clear the array by resetting its size to zero. Note that the memory is not actually released.
        /// </summary>
        public void Clear()
        {
            _list.Clear();

            if (OnClear != null)
                OnClear();

        }


        /// <summary>
        /// Clear the array and release the used memory.
        /// </summary>
        public void Release()
        {
            if (OnClear != null)
                Clear();
        }

        /// <summary>
        /// returns count
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
        /// </summary>

        public T[] ToArray()
        {
            return _list.ToArray();
        }
    }

}