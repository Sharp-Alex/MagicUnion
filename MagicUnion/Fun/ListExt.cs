using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicUnion.Fun
{
    public static class Lst
    {
        public static List<T> New<T>()
        {
            return new List<T>();
        }

        public static List<T> Plus<T>(this List<T> list, T value)
        {
            list.Add(value);
            return list;
        }

        public static List<T> Plus<T>(this List<T> list, IEnumerable<T> value)
        {
            list.AddRange(value);
            return list;
        }

        public static List<T> New<T>(this T value)
        {
            return New<T>().Plus(value);
        }
    }
}
