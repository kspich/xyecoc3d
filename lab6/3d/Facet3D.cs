using lab6;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace lab6
{
    public class Facet3d
    {
        public List<Point3d> Points { get; set; }
        public List<Edge3d> Edges { get; set; }
        //public List<List<Point>> Texture { get; set; }
        public Vector3d Normal { get; set; }

        public Facet3d() : this(new List<Point3d>(), new List<Edge3d>())
        {
        }

        public Facet3d(List<Point3d> points, List<Edge3d> edges)
        {
            Points = points;
            Edges = edges;
            //Texture = new List<List<Point>>();
            Normal = ComputeNormal(edges[0], edges[1]);
            Normal.NormalizeInplace();
            foreach (var p in Points)
            {
                p.Normal.Add(Normal);
            }
        }

        private static Vector3d ComputeNormal(Edge3d edge1, Edge3d edge2)
        {
            var v1 = edge1.ToVector3d();
            var v2 = edge2.ToVector3d();
            return v1.CrossProduct(v2);
        }

        public void AddPoint(Point3d p)
        {
            Points.Add(p);
        }

        public void AddEdge(Edge3d edge)
        {
            Edges.Add(edge);
        }

        public void LoadTexture(string path)
        {
            //var texture = Bitmap.FromFile(path);
            //Texture = new List<List<Point>>();
        }
    }
}