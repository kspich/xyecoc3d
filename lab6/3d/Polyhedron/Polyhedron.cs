using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using lab6;

namespace lab6
{
    using static AffineTransformations;
    using static TransformationMethods;

    public class Polyhedron : ICloneable
    {
        public List<Point3d> Vertices { get; set; }
        public List<Edge3d> Edges { get; set; }
        public List<Facet3d> Facets { get; set; }
        private Point3d _center;
        public Point3d Center => _center;
        public Color Color { get; set; }

        public Polyhedron(List<Point3d> vertices, List<Edge3d> edges, List<Facet3d> facets)
        {
            Vertices = vertices;
            Edges = edges;
            Facets = facets;
            _center = ComputeCenter();
            var random = new Random();
            Color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
            foreach (var v in Vertices)
            {
                v.Normal.NormalizeInplace();
            }
        }

        public void SaveToFile(string path)
        {
            var sb = new StringBuilder();
            foreach (var v in Vertices)
            {
                sb.Append($"v {v.X} {v.Y} {v.Z}\n");
            }
            foreach (var e in Edges)
            {
                int beginIndex = Vertices.FindIndex(v => v == e.Begin);
                int endIndex = Vertices.FindIndex(v => v == e.End);
                sb.Append($"e {beginIndex} {endIndex}\n");
            }
            foreach (var facet in Facets)
            {
                sb.Append("facet\n");
                foreach (var p in facet.Points)
                {
                    int pointIndex = Vertices.FindIndex(v => p == v);
                    sb.Append($"vi {pointIndex}\n");
                }
                foreach (var edge in facet.Edges)
                {
                    int edgeIndex = Edges.FindIndex(e => e == edge);
                    sb.Append($"ei {edgeIndex}\n");
                }
                sb.Append("endfacet\n");
            }
            File.WriteAllText(path, sb.ToString());
        }

        public static Polyhedron ReadFromFile(string path)
        {
            var vertices = new List<Point3d>();
            var edges = new List<Edge3d>();
            var facets = new List<Facet3d>();
            Facet3d currentFacet = null;
            foreach (var line in File.ReadLines(path))
            {
                var splited = line.Split();

                if (currentFacet != null)
                {
                    if (splited[0] == "vi")
                    {
                        int vertexIndex = int.Parse(splited[1]);
                        currentFacet.AddPoint(vertices[vertexIndex]);
                    }
                    else if (splited[0] == "ei")
                    {
                        int edgeIndex = int.Parse(splited[1]);
                        currentFacet.AddEdge(edges[edgeIndex]);
                    }
                }
                else if (splited[0] == "v")
                {
                    double x = double.Parse(splited[1]);
                    double y = double.Parse(splited[2]);
                    double z = double.Parse(splited[3]);
                    vertices.Add(new Point3d(x, y, z));
                }
                else if (splited[0] == "e")
                {
                    int beginIndex = int.Parse(splited[1]);
                    int endIndex = int.Parse(splited[2]);
                    edges.Add(new Edge3d(vertices[beginIndex], vertices[endIndex]));
                }
                else if (splited[0] == "facet")
                {
                    currentFacet = new Facet3d();
                }
                else if (splited[0] == "endfacet")
                {
                    facets.Add(currentFacet);
                    currentFacet = null;
                }
            }
            return new Polyhedron(vertices, edges, facets);
        }

        public Polyhedron ComputeProjection(Projection projectionType)
        {
            var clone = Clone() as Polyhedron;
            var perspectiveProjectionMatrix = projectionType.CreateMatrix();
            ApplyTransformationInplace(clone, perspectiveProjectionMatrix);
            return clone;
        }

        public void Translate(double dx, double dy, double dz)
        {
            var translationTransformation = MakeTranslationMatrix(dx, dy, dz);
            ApplyTransformationInplace(this, translationTransformation);
        }

        public void RotateAroundEdge(Edge3d edge, double degrees)
        {
            var rotationAroundEdgeCenteredTransformation = MakeRotateAroundEdgeMatrix(edge, degrees);
            ApplyTransformationInplace(this, rotationAroundEdgeCenteredTransformation);
        }

        public void RotateAxis(double xDegrees, double yDegrees, double zDegrees)
        {
            var axisRotationTransformation = MakeXRotationMatrix(xDegrees)
                * MakeYRotationMatrix(yDegrees)
                * MakeZRotationMatrix(zDegrees);
            ApplyTransformationInplace(this, axisRotationTransformation);
        }

        public void RotateAroundCenter(double degreesX, double degreesY, double degreesZ)
        {
            RotateAroundPoint(degreesX, degreesY, degreesZ, Center);
        }

        public void RotateAroundPoint(double degreesX, double degreesY, double degreesZ, Point3d p)
        {
            var centeredRotationTransformation = MakeTranslationMatrix(-p.X, -p.Y, -p.Z)
                * MakeXYZRotationMatrix(degreesX, degreesY, degreesZ)
                * MakeTranslationMatrix(p.X, p.Y, p.Z);
            ApplyTransformationInplace(this, centeredRotationTransformation);
        }

        public void Scale(double mx, double my, double mz)
        {
            var scalingTransformation = MakeScalingMatrix(mx, my, mz);
            ApplyTransformationInplace(this, scalingTransformation);
        }

        public void ScaleCentered(double factor)
        {
            var centeredScalingTransformation = MakeTranslationMatrix(-Center.X, -Center.Y, -Center.Z)
                * MakeScalingMatrix(factor, factor, factor)
                * MakeTranslationMatrix(Center.X, Center.Y, Center.Z);
            ApplyTransformationInplace(this, centeredScalingTransformation);
        }

        public void ReflectXY()
        {
            var xYReflectionTransformation = MakeXYReflectionMatrix();
            ApplyTransformationInplace(this, xYReflectionTransformation);
        }

        public void ReflectYZ()
        {
            var yZReflectionTransformation = MakeYZReflectionMatrix();
            ApplyTransformationInplace(this, yZReflectionTransformation);
        }

        public void ReflectZX()
        {
            var zXReflectionTransformation = MakeZXReflectionMatrix();
            ApplyTransformationInplace(this, zXReflectionTransformation);
        }

        public object Clone()
        {
            var vertices = Vertices.Select(v => v.Clone() as Point3d).ToList();
            var edges = new List<Edge3d>(Edges.Count);
            for (int i = 0; i < Edges.Count; i++)
            {
                Point3d begin = null, end = null;
                for (int j = 0; j < Vertices.Count; j++)
                {
                    if (begin == null && Edges[i].Begin.Id == vertices[j].Id)
                    {
                        begin = vertices[j];
                    }
                    if (end == null && Edges[i].End.Id == vertices[j].Id)
                    {
                        end = vertices[j];
                    }
                    if (begin != null && end != null)
                    {
                        edges.Add(new Edge3d(begin, end, Edges[i].Id));
                        break;
                    }
                }
            }
            var facets = new List<Facet3d>(Facets.Count);
            foreach (var f in Facets)
            {
                var currentPoints = new List<Point3d>();
                var currentEdges = new List<Edge3d>();

                foreach (var p in f.Points)
                {
                    var pointToAdd = vertices.Find(v => v.Id == p.Id);
                    currentPoints.Add(pointToAdd);
                }

                foreach (var edge in f.Edges)
                {
                    var edgeToAdd = edges.Find(e => e.Id == edge.Id);
                    currentEdges.Add(edgeToAdd);
                }

                var currentFacet = new Facet3d(currentPoints, currentEdges);
                facets.Add(currentFacet);
            }
            var polyhedron = new Polyhedron(vertices, edges, facets);
            polyhedron.Color = Color;
            return polyhedron;
        }

        public Polyhedron Copy() => Clone() as Polyhedron;

        private Point3d ComputeCenter()
            => ComputeCenter(this);

        private static Point3d ComputeCenter(Polyhedron polyhedron)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            int pointsTotal = polyhedron.Vertices.Count;
            foreach (var vertex in polyhedron.Vertices)
            {
                x += vertex.X;
                y += vertex.Y;
                z += vertex.Z;
            }
            return new Point3d(x / pointsTotal, y / pointsTotal, z / pointsTotal);
        }
    }

    public static class Drawing

    {
        public static void DrawPolyhedron(this Bitmap drawingSurface, Polyhedron polyhedron, Color color)
        {
            using (var fastDrawingSurface = new FastBitmap(drawingSurface))
            {
                fastDrawingSurface.DrawPolyhedron(polyhedron, color);
            }
        }

        public static void DrawPolyhedron(this FastBitmap fastDrawingSurface, Polyhedron polyhedron, Color color)
        {
            foreach (var edge in polyhedron.Edges)
            {
                fastDrawingSurface.DrawLine(edge.Begin.ToPoint(), edge.End.ToPoint(), color);
            }
        }
    }
}
