using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void RemoveEdge(VertexCore vertexFirst, VertexCore vertexSecond)
        {
            if(vertexFirst!=null && vertexSecond!=null)
                Edges.Remove(vertexFirst.RemoveEdge(vertexSecond));
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



        public static GraphCore LoadGraphFromFile(string path)
        {
            GraphCore graph = new GraphCore();

            var lines = File.ReadLines(path).ToList();
            int numberOfVertex = 0;
            int numberOfEdges = 0;
            if (lines.Count>1)
            {
                var splitFirstLine = lines[0].Split(' ');
                if(splitFirstLine.Length == 2)
                {
                    numberOfVertex = int.Parse(splitFirstLine[0]);
                    numberOfEdges = int.Parse(splitFirstLine[1]);
                }

                for (int i = 1; i < lines.Count; i++)
                {
                    var splitLine = lines[i].Split(' ');
                    if (splitLine.Length == 2)
                    { 
                        graph.ConnectVertex(graph.AddVertexIfNotExist(int.Parse(splitLine[1])), graph.AddVertexIfNotExist(int.Parse(splitLine[0])));
                    }
                }
            }
            return graph;
        }



        private static IEnumerable<CycleCore> FindAllCycles(List<VertexCore> alreadyVisited, VertexCore a)
        {
            foreach (EdgeCore e in a.Edges)
            {
                if (alreadyVisited.Contains(e.Second))
                    yield return new CycleCore(e);
                else
                {
                    List<VertexCore> newSet = new List<VertexCore>(alreadyVisited);
                    newSet.Add(e.Second);
                    foreach (CycleCore c in FindAllCycles(newSet, e.Second))
                    {
                        c.Build(e);
                        yield return c;
                    }
                }
            }
        }

        public bool IsAnyCycleExist()
        {
            List<VertexCore> alreadyVisited = new List<VertexCore>();
            alreadyVisited.Add(Vertices.First());
            var cycles = FindAllCycles(alreadyVisited, Vertices.First()).ToList();
            cycles.ForEach(x => Debug.WriteLine(x));
            return cycles.Count>0;
        }
    }

}
