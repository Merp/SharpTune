///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SamplePacketData.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NateW.Ssm
{
/*    public interface IReadOnlyList<T> : IEnumerable<T>
    {
        public int IndexOf(T item);
        
        public T this[int index]
        {
            get
            {
                return this.list[index];
            }
        }

        public bool Contains(T item)
        {
            return this.list.Contains(item);
        }

        public int Count
        {
            get { return this.list.Count; }
        }

        /// <summary>
        /// Gets an enumerator for the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
    }
    */
    /// <summary>
    /// Read-only wrapper around the generic IList class.
    /// </summary>
    /// <typeparam name="T">Type of object in the list.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710")] 
    public class ReadOnlyList<T> : IList<T>
    {
        /// <summary>
        /// The underlying list
        /// </summary>
        private IList<T> list;

        /// <summary>
        /// Creates an empty ReadOnlyList.
        /// </summary>
        public ReadOnlyList()
        {
            this.list = new T[0];
        }

        /// <summary>
        /// Creates a ReadOnlyList wrapper atop the given list.
        /// </summary>
        /// <param name="source">An IList of whatever.</param>
        public ReadOnlyList(IList<T> source)
        {
            this.list = source;
        }

        /// <summary>
        /// Creates a ReadOnlyList that contains a copy of the given collection.
        /// </summary>
        /// <param name="source">A bunch of whatever.</param>
        public ReadOnlyList(IEnumerable<T> source)
        {
            this.list = new List<T>(source);
        }

        /// <summary>
        /// Returns the index of the given item.
        /// </summary>
        /// <param name="item">An item in the list.</param>
        /// <returns>The index of the given item.</returns>
        public int IndexOf(T item)
        {
            return this.list.IndexOf(item);
        }

        /// <summary>
        /// Throws.
        /// </summary>
        /// <param name="index">Don't bother.</param>
        /// <param name="item">Really, don't.</param>
        /// <exception cref="InvalidOperationException">Every time.</exception>
        public void Insert(int index, T item)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Throws.
        /// </summary>
        /// <param name="item">Just don't.</param>
        /// <exception cref="InvalidOperationException">Every time.</exception>
        public void RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets an item at the given index.  Don't try to set.
        /// </summary>
        /// <param name="item">The index of an item in the list.</param>
        /// <exception cref="InvalidOperationException">If you try to set.</exception>
        public T this[int index]
        {
            get
            {
                return this.list[index];
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Throws.
        /// </summary>
        /// <param name="item">Just don't.</param>
        /// <exception cref="InvalidOperationException">Every time.</exception>
        public void Add(T item)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Throws.
        /// </summary>
        /// <exception cref="InvalidOperationException">Every time.</exception>
        public void Clear()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Indicates whether the list contains the given item.
        /// </summary>
        /// <param name="item">An item that might be in the list.</param>
        /// <returns>True if the item is in the list, false if not.</returns>
        public bool Contains(T item)
        {
            return this.list.Contains(item);
        }

        /// <summary>
        /// Copies the list to the given array.
        /// </summary>
        /// <param name="array">This should probably be an IList.</param>
        /// <param name="arrayIndex">Where to begin copying items to.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the size of the list.
        /// </summary>
        public int Count
        {
            get { return this.list.Count; }
        }

        /// <summary>
        /// True.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Throws.
        /// </summary>
        /// <param name="item">Just don't.</param>
        /// <exception cref="InvalidOperationException">Every time.</exception>
        public bool Remove(T item)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets an enumerator for the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
    }
}
