using System;
using System.Collections.Generic;
using System.Linq;

namespace GizCore
{
    public class VertexCore : IEquatable<VertexCore>
    {
        private static int LastId = 0;
        public int Id { get; private set; }
        public List<EdgeCore> Edges { get; private set; } = new List<EdgeCore>();
        public List<VertexCore> AdjacentVertices { get; private set; } = new List<VertexCore>();
        public int Degree { get; internal set; }

        public VertexCore(int id)
        {
            Id = id;
            ++LastId;
        }

        private void RemoveAdjacentVertex(VertexCore vertex, EdgeCore edge)
        {
            Edges.Remove(edge);
            AdjacentVertices.Remove(vertex);
            Degree = AdjacentVertices.Count;
        }


        public EdgeCore AddEdge(VertexCore vertex)
        {
            if (vertex == null)
                return null;

            var edge = new EdgeCore(this, vertex);


            this.Edges.Add(edge);
            this.AdjacentVertices.Add(vertex);
            this.Degree = this.AdjacentVertices.Count;


            return edge;
        }


        public EdgeCore RemoveEdge(VertexCore vertex)
        {
            if (vertex == null)
                return null;

            var edge = Edges.Where(x => (x.First == vertex && x.Second == this) || (x.First == this && x.Second == vertex)).FirstOrDefault();
            if (edge == null)
                return null;

            edge.First.RemoveAdjacentVertex(edge.Second, edge);
            edge.Second.RemoveAdjacentVertex(edge.First, edge);

            return edge;
        }

        public override bool Equals(object obj)
        {
            if (obj is VertexCore)
            {
                return Equals((VertexCore)obj);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(VertexCore first, VertexCore second)
        {
            if ((object)first == null)
            {
                return (object)second == null;
            }

            return first.Equals(second);
        }

        public static bool operator !=(VertexCore first, VertexCore second)
        {
            return !(first == second);
        }

        public bool Equals(VertexCore other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 47;
                hashCode = (hashCode * 53) ^ Id.GetHashCode();
                return hashCode;
            }
        }
        public override string ToString()
        {
            return $"Id: {Id}, Degree: {Degree}";
        }

    }
}
