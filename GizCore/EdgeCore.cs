using System;
using System.Linq;

namespace GizCore
{
    public class EdgeCore : IEquatable<EdgeCore>
    {
        public VertexCore First { get; set; }
        public VertexCore Second { get; set; }
        public EdgeCore(VertexCore first, VertexCore second)
        {
            this.First = first;
            this.Second = second;
        }

        public override bool Equals(object obj)
        {
            if (obj is EdgeCore)
            {
                return Equals((EdgeCore)obj);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(EdgeCore first, EdgeCore second)
        {
            if ((object)first == null)
            {
                return (object)second == null;
            }

            return first.Equals(second);
        }

        public static bool operator !=(EdgeCore first, EdgeCore second)
        {
            return !(first == second);
        }

        public bool Equals(EdgeCore other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(First, other.First) && Equals(Second, other.Second);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 47;
                if (First != null)
                {
                    hashCode = (hashCode * 53) ^ First.GetHashCode();
                }

                if (Second != null)
                {
                    hashCode = (hashCode * 53) ^ Second.GetHashCode();
                }

                return hashCode;
            }
        }
    }
}
