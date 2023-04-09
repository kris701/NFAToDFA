using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFAToDFA.Models
{
    public class Set : IEquatable<object>
    {
        public HashSet<string> Items { get; set; }

        public int Count => Items.Count;

        #region Constructors
        public Set(HashSet<string> items)
        {
            Items = items;
        }

        public Set(Set set2)
        {
            Items = new HashSet<string>();
            foreach (var item in set2.Items)
                Items.Add(item);
        }

        public Set(Set set2, Set set3)
        {
            Items = new HashSet<string>();
            foreach(var item in set2.Items)
                Items.Add(item);
            foreach (var item in set3.Items)
                Items.Add(item);
        }

        public Set(Set set2, Set set3, Set set4)
        {
            Items = new HashSet<string>();
            foreach (var item in set2.Items)
                Items.Add(item);
            foreach (var item in set3.Items)
                Items.Add(item);
            foreach (var item in set4.Items)
                Items.Add(item);
        }

        public Set()
        {
            Items = new HashSet<string>();
        }

        public Set(string item1)
        {
            Items = new HashSet<string>();
            Items.Add(item1);
        }

        public Set(string item1, string item2)
        {
            Items = new HashSet<string>();
            Items.Add(item1);
            Items.Add(item2);
        }

        public Set(string item1, string item2, string item3)
        {
            Items = new HashSet<string>();
            Items.Add(item1);
            Items.Add(item2);
            Items.Add(item3);
        }
        #endregion

        public override bool Equals(object? obj)
        {
            if (obj is Set set2)
                return GetHashCode() == set2.GetHashCode();
            return false;
        }

        public static bool operator ==(Set obj1, Set obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (ReferenceEquals(obj1, null))
                return false;
            if (ReferenceEquals(obj2, null))
                return false;
            return obj1.Equals(obj2);
        }
        public static bool operator !=(Set obj1, Set obj2) => !(obj1 == obj2);

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var item in Items.Order())
                hash += HashCode.Combine(item);
            return hash;
        }

        public override string? ToString()
        {
            string retStr = "";
            foreach (var item in Items)
                retStr += item.ToString();
            return retStr;
        }

        public void Add(string item)
        {
            Items.Add(item);
        }

        public void Add(Set set)
        {
            foreach(var item in set.Items)
                Items.Add(item);
        }
    }
}
