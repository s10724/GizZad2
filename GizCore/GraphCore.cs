using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace GizCore
{
    public class GraphCore
    {
        public List<VertexCore> Vertices { get; private set; } = new List<VertexCore>();
        public List<EdgeCore> Edges { get; private set; } = new List<EdgeCore>();
        public VertexCore AddVertex()
        {
            int id;
            if (Vertices.Count==0)
            {
                id = 1;
            }
            else
            {
                id = Vertices.Select(x => x.Id).Max() + 1;
            }
            var vertex = new VertexCore(id);
            Vertices.Add(vertex);
            return vertex;
        }


        private VertexCore AddVertexIfNotExist(int id)
        {
            var vertex = Vertices.Where(x => x.Id == id).FirstOrDefault();
            if(vertex==null)
            {
                vertex = new VertexCore(id);
                Vertices.Add(vertex);
            }
            return vertex;
        }

        public void RemoveVertex(VertexCore vertex)
        {
            var list = vertex.AdjacentVertices.ToList();
            foreach(var item in list)
            {
                Edges.Remove(item.RemoveEdge(vertex));
            }
            Vertices.Remove(vertex);
        }


        public bool ConnectVertex(VertexCore vertexFirst, VertexCore vertexSecond)
        {
            if (vertexFirst == null || vertexSecond == null)
                return false;

            if (!Vertices.Contains(vertexFirst))
            {
                Vertices.Add(vertexFirst);
            }
            if (!Vertices.Contains(vertexSecond))
            {
                Vertices.Add(vertexSecond);
            }
            Edges.Add(vertexFirst.AddEdge(vertexSecond));
            return true;
        }

        public VertexCore FindVertex(int vertextId)
        {
           return Vertices.Where(x => x.Id == vertextId).FirstOrDefault();
        }

        public VertexCore FindVertex(string vertextIdStr)
        {
            int vertextId;
            if (!string.IsNullOrWhiteSpace(vertextIdStr) && int.TryParse(vertextIdStr, out vertextId))
            {
                return FindVertex(vertextId);
            }
            return null;
        }
        public bool ConnectVertex(int vertexFirstId, int vertexSecondId)
        {
            return ConnectVertex(FindVertex(vertexFirstId), FindVertex(vertexSecondId));
        }

        public bool ConnectVertex(string vertexFirstIdStr, string vertexSecondIdStr)
        {
            return ConnectVertex(FindVertex(vertexFirstIdStr), FindVertex(vertexSecondIdStr));
        }

        public List<int> GeneratePruferCode()
        {
            if (Vertices == null || Vertices.Count < 3)
                return null;

            var list = new List<int>();
            while (Edges.Count>1)
            {
                var vertexWithOneDegree = Vertices.Where(x => x.Degree == 1).OrderBy(x => x.Id).FirstOrDefault();
                list.Add(vertexWithOneDegree.AdjacentVertices[0].Id);
                Edges.Remove(vertexWithOneDegree.RemoveEdge(vertexWithOneDegree.AdjacentVertices[0]));
            }
            DecodePruferCode(list);
            return list;
        }


        public void DecodePruferCode(List<int> pruferCode)
        {
            Vertices = new List<VertexCore>();
            Edges = new List<EdgeCore>();

            List<int> p = pruferCode.ToList();
            List<int> v = new List<int>();
            for (int i = 0; i < pruferCode.Count + 2; i++)
            {
                v.Add(i+1);
            }

            while (true)
            {
                int i = v.Except(p).Min();
                ConnectVertex(AddVertexIfNotExist(i), AddVertexIfNotExist(p[0]));
                p.Remove(p[0]);
                v.Remove(i);

                if(p.Count==0)
                {
                    ConnectVertex(AddVertexIfNotExist(v[0]), AddVertexIfNotExist(v[1]));
                    return;
                }
            }
        }

    }
}
