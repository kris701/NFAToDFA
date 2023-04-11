using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFAToDFA.Models
{
    public class Set<T> : IEquatable<object> where T : notnull
    {
        public HashSet<T> Items { get; set; }

        public int Count => Items.Count;

        #region Constructors
        public Set(HashSet<T> items)
        {
            Items = items;
        }

        public Set(T[] items)
        {
            Items = new HashSet<T>();
            foreach (var item in items)
                Items.Add(item);
        }

        public Set(Set<T> set2)
        {
            Items = new HashSet<T>();
            foreach (var item in set2.Items)
                Items.Add(item);
        }

        public Set(Set<T> set2, Set<T> set3)
        {
            Items = new HashSet<T>();
            foreach (var item in set2.Items)
                Items.Add(item);
            foreach (var item in set3.Items)
                Items.Add(item);
        }

        public Set(Set<T> set2, Set<T> set3, Set<T> set4)
        {
            Items = new HashSet<T>();
            foreach (var item in set2.Items)
                Items.Add(item);
            foreach (var item in set3.Items)
                Items.Add(item);
            foreach (var item in set4.Items)
                Items.Add(item);
        }

        public Set()
        {
            Items = new HashSet<T>();
        }

        public Set(T item1)
        {
            Items = new HashSet<T>();
            Items.Add(item1);
        }

        public Set(T item1, T item2)
        {
            Items = new HashSet<T>();
            Items.Add(item1);
            Items.Add(item2);
        }

        public Set(T item1, T item2, T item3)
        {
            Items = new HashSet<T>();
            Items.Add(item1);
            Items.Add(item2);
            Items.Add(item3);
        }
        #endregion

        public override bool Equals(object? obj)
        {
            if (obj is Set<T> set2)
                return GetHashCode() == set2.GetHashCode();
            return false;
        }

        public static bool operator ==(Set<T> obj1, Set<T> obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (ReferenceEquals(obj1, null))
                return false;
            if (ReferenceEquals(obj2, null))
                return false;
            return obj1.Equals(obj2);
        }
        public static bool operator !=(Set<T> obj1, Set<T> obj2) => !(obj1 == obj2);

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var item in Items.Order())
                hash += HashCode.Combine(item);
            return hash;
        }

        public override string? ToString()
        {
            string retStr = "(";
            int counter = 0;
            foreach (var item in Items)
            {
                retStr += item.ToString();
                if (counter != Items.Count - 1)
                    retStr += ",";
                counter++;
            }
            retStr += ")";
            return retStr;
        }

        public void Add(T item)
        {
            Items.Add(item);
        }

        public void Add(Set<T> set)
        {
            foreach (var item in set.Items)
                Items.Add(item);
        }
    }
}
