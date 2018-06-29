using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizCore
{
     public class TopologicalSorting
    {
        public static List<T> Sort<T>(IList<T> nodes, IList<Tuple<T, T>> edges) where T : IEquatable<T>
        {
            var L = new List<T>();
            var S = new HashSet<T>(nodes.Where(n => edges.All(e => e.Item2.Equals(n) == false)));
            while (S.Any())
            {
                var n = S.First();
                S.Remove(n);
                L.Add(n);
                foreach (var e in edges.Where(e => e.Item1.Equals(n)).ToList())
                {
                    var m = e.Item2;
                    edges.Remove(e);
                    if (edges.All(me => me.Item2.Equals(m) == false))
                    {
                        S.Add(m);
                    }
                }
            }
            if (edges.Any())
            {
                return null;
            }
            else
            {
                return L;
            }
        }
    }
}

