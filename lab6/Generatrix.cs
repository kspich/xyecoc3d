using lab6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab6
{
    using static TransformationMethods;

    public class Generatrix
    {
        public Axis RotationAxis { get; set; }
        public List<Point3d> Points { get; private set; }
        public List<Edge3d> Edges { get; private set; }

        public Generatrix(List<Point3d> points, Axis rotationAxis)
        {
            Points = points;
            Edges = ConnectPoints(points);
            RotationAxis = rotationAxis;
        }

        private static List<Edge3d> ConnectPoints(List<Point3d> points)
        {
            var edges = new List<Edge3d>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                edges.Add(new Edge3d(points[i], points[i + 1]));
            }
            return edges;
        }

        /// <summary>
        /// Создает тело вращения на основе образующей
        /// </summary>
        /// <param name="partitionsCount">Число разбиений</param>
        /// <returns>Многогранник, приближенно представляющий тело вращения</returns>
        public Polyhedron CreateRotationBody(int partitionsCount)
        {
            EnsurePartitionsCountIsPositive(partitionsCount);
            var generatrices = GenerateSkeletonBase(partitionsCount);
            return ConnectGeneratrices(generatrices);
        }

        private Generatrix[] GenerateSkeletonBase(int partitionsCount)
        {
            return GenerateSkeletonBase(this, partitionsCount);
        }

        private static Generatrix[] GenerateSkeletonBase(Generatrix beginGeneratrix, int partitionsCount)
        {
            double degreesPerPartition = 360 / partitionsCount;
            var generatrices = new Generatrix[partitionsCount];
            generatrices[0] = beginGeneratrix;
            for (int i = 1; i < partitionsCount; i++)
            {
                generatrices[i] = generatrices[i - 1].Rotate(degreesPerPartition);
            }
            return generatrices;
        }

        private Generatrix Rotate(double degrees)
        {
            var rotated = Copy();
            TransformMultiplePointsInplace(rotated.Points, RotationAxis.CreateRotationMatrix(degrees));
            return rotated;
        }

        private static Polyhedron ConnectGeneratrices(Generatrix[] generatrices)
        {
            var vertices = new List<Point3d>();
            var edges = new List<Edge3d>();
            var facets = new List<Facet3d>();

            foreach (var g in generatrices)
            {
                edges.AddRange(g.Edges);
                vertices.AddRange(g.Points);
            }

            int i = 0;
            for (; i < generatrices.Length - 1; i++)
            {
                MakeConnectionStep(generatrices, vertices, edges, facets, i, i + 1);
            }
            MakeConnectionStep(generatrices, vertices, edges, facets, i, 0);

            return new Polyhedron(vertices, edges, facets);
        }

        private static void MakeConnectionStep(Generatrix[] generatrices, List<Point3d> vertices, List<Edge3d> edges, List<Facet3d> facets, int iCurrent, int iNext)
        {
            var connectionEdges = new List<Edge3d>();
            for (int j = 0; j < generatrices[iCurrent].Points.Count; j++)
            {
                var connectionEdge = new Edge3d(generatrices[iCurrent].Points[j], generatrices[iNext].Points[j]);
                connectionEdges.Add(connectionEdge);
            }
            edges.AddRange(connectionEdges);

            for (int k = 0; k < generatrices[iCurrent].Edges.Count; k++)
            {
                var connectionEdge0 = connectionEdges[k];
                var generatrixEdge0 = generatrices[iCurrent].Edges[k];
                var generatrixEdge1 = generatrices[iNext].Edges[k];
                var connectionEdge1 = connectionEdges[k + 1];

                var currentFacetEdges = new List<Edge3d>
                {
                    connectionEdge0,
                    generatrixEdge0, generatrixEdge1,
                    connectionEdge1
                };

                var currentFacetPoints = new List<Point3d>
                {
                    connectionEdge0.Begin, connectionEdge0.End,
                    connectionEdge1.Begin, connectionEdge1.End,
                };

                var currentFacet = new Facet3d(currentFacetPoints, currentFacetEdges);

                facets.Add(currentFacet);
            }
        }

        private static void EnsurePartitionsCountIsPositive(int partitionsCount)
        {
            if (partitionsCount < 1)
            {
                throw new ArgumentException("Число разиений должно быть положительно");
            }
        }

        private Generatrix Copy()
        {
            var points = Points.Select(p => new Point3d(p)).ToList();
            return new Generatrix(points, RotationAxis);
        }
    }
}
