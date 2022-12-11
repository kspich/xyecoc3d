using lab6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab6
{
    public enum PolyhedronType
    {
        Tetrahedron, Oktahedron, Geksahedron, Ikosahedron, Dodahedron
    }

    public static class PolyhedronTypeExtensionMethods
    {
        public static string GetPolyhedronName(this PolyhedronType polyhedronType)
        {
            switch (polyhedronType)
            {
                case PolyhedronType.Tetrahedron:
                    return "Тетраэдр";
                case PolyhedronType.Geksahedron:
                    return "Гексаэдр";
                case PolyhedronType.Oktahedron:
                    return "Октаэдр";
                case PolyhedronType.Ikosahedron:
                    return "Икосаэдр";
                case PolyhedronType.Dodahedron:
                    return "Додекаэдр";
                default:
                    throw new ArgumentException("Unknown polyhedron type");
            }
        }

        public static Polyhedron CreatePolyhedron(this PolyhedronType polyhedronType)
        {
            switch (polyhedronType)
            {
                case PolyhedronType.Tetrahedron:
                    return Regular.Tetrahedron();
                case PolyhedronType.Geksahedron:
                    return Regular.Geksahedron();
                case PolyhedronType.Oktahedron:
                    return Regular.Oktahedron();
                /*
                case PolyhedronType.Ikosahedron:
                    return Regular.Ikosahedron();
                case PolyhedronType.Dodahedron:
                    return Regular.Dodahedron();
                */
                default:
                    throw new ArgumentException("Unknown polyhedron type");
            }
        }
    }

    public static class Regular
    {
        private static readonly int longg = 300;
        private static Point3d Middle(Point3d P1, Point3d P2)
        {
            return new Point3d((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2, (P1.Z + P2.Z) / 2);
        }
        private static Point3d Middle(Point3d P1, Point3d P2, Point3d P3)
        {
            return new Point3d((P1.X + P2.X + P3.X) / 3, (P1.Y + P2.Y + P3.Y) / 3, (P1.Z + P2.Z + P3.Z) / 3);
        }

        public static Polyhedron Tetrahedron()
        {
            Polyhedron P = Geksahedron();
            var vertices = new List<Point3d>();
            for (int i = 1; i <= 3; i++)
            {
                vertices.Add(P.Vertices[i]);
            }
            vertices.Add(P.Vertices[7]);
            var edges = new List<Edge3d>();
            for (int i = 0; i <= 3; i++)
            {
                for (int j = i + 1; j <= 3; j++)
                {
                    edges.Add(new Edge3d(vertices[i], vertices[j]));
                }
            }
            var facets = new List<Facet3d>();

            var facet0 = new Facet3d(new List<Point3d> { vertices[0], vertices[1], vertices[3] }, new List<Edge3d> { edges[0], edges[4], edges[2] });
            var facet1 = new Facet3d(new List<Point3d> { vertices[0], vertices[3], vertices[2] }, new List<Edge3d> { edges[2], edges[5], edges[1] });
            var facet2 = new Facet3d(new List<Point3d> { vertices[1], vertices[2], vertices[3] }, new List<Edge3d> { edges[3], edges[4], edges[5] });
            var facet3 = new Facet3d(new List<Point3d> { vertices[0], vertices[1], vertices[2] }, new List<Edge3d> { edges[1], edges[0], edges[3] });
            facets.Add(facet0);
            facets.Add(facet1);
            facets.Add(facet2);
            facets.Add(facet3);
            return new Polyhedron(vertices, edges, facets);
        }

        public static Polyhedron Geksahedron()
        {
            var vertices = new List<Point3d>();
            var p0 = new Point3d(0, 0, 0);
            var p1 = new Point3d(0, 0, longg);  // facet0
            var p2 = new Point3d(0, longg, 0);
            var p3 = new Point3d(longg, 0, 0);
            var p4 = new Point3d(longg, 0, longg);  // facet0
            var p5 = new Point3d(longg, longg, 0);
            var p6 = new Point3d(0, longg, longg);  // facet0
            var p7 = new Point3d(longg, longg, longg);  // facet0
            vertices.Add(p0);
            vertices.Add(p1);
            vertices.Add(p2);
            vertices.Add(p3);
            vertices.Add(p4);
            vertices.Add(p5);
            vertices.Add(p6);
            vertices.Add(p7);

            var edges = new List<Edge3d>();
            var edge0 = new Edge3d(p0, p1);
            var edge1 = new Edge3d(p0, p2);
            var edge2 = new Edge3d(p0, p3);
            var edge3 = new Edge3d(p1, p4);
            var edge4 = new Edge3d(p1, p6);
            var edge5 = new Edge3d(p2, p5);
            var edge6 = new Edge3d(p2, p6);
            var edge7 = new Edge3d(p3, p5);
            var edge8 = new Edge3d(p3, p4);
            var edge9 = new Edge3d(p4, p7);
            var edge10 = new Edge3d(p5, p7);
            var edge11 = new Edge3d(p6, p7);

            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);
            edges.Add(edge4);
            edges.Add(edge5);
            edges.Add(edge6);
            edges.Add(edge7);
            edges.Add(edge8);
            edges.Add(edge9);
            edges.Add(edge10);
            edges.Add(edge11);

            var facets = new List<Facet3d>();
            var facet0 = new Facet3d(new List<Point3d> { p1, p4, p7, p6 }, new List<Edge3d> { edge3, edge4, edge11, edge9 });
            var facet1 = new Facet3d(new List<Point3d> { p0, p2, p5, p3 }, new List<Edge3d> { edge2, edge1, edge5, edge7 });
            var facet2 = new Facet3d(new List<Point3d> { p0, p3, p4, p1 }, new List<Edge3d> { edge2, edge8, edge3, edge0 });
            var facet3 = new Facet3d(new List<Point3d> { p2, p5, p7, p6 }, new List<Edge3d> { edge10, edge11, edge6, edge5 });
            var facet4 = new Facet3d(new List<Point3d> { p3, p4, p7, p5 }, new List<Edge3d> { edge8, edge9, edge10, edge7 });
            var facet5 = new Facet3d(new List<Point3d> { p0, p1, p6, p2 }, new List<Edge3d> { edge0, edge4, edge6, edge1 });
            facets.Add(facet0);
            facets.Add(facet1);
            facets.Add(facet2);
            facets.Add(facet3);
            facets.Add(facet4);
            facets.Add(facet5);

            return new Polyhedron(vertices, edges, facets);
        }

        public static Polyhedron Oktahedron()
        {
            Polyhedron P = Geksahedron();
            var vertices = new List<Point3d>();
            for (int i = 4; i <= 6; i++)
            {
                vertices.Add(Middle(P.Vertices[0], P.Vertices[i]));
            }
            for (int i = 1; i <= 3; i++)
            {
                vertices.Add(Middle(P.Vertices[7], P.Vertices[i]));
            }

            var edges = new List<Edge3d>();
            for (int i = 1; i <= 3; i++)
            {

                edges.Add(new Edge3d(vertices[0], vertices[i]));
            }
            edges.Add(new Edge3d(vertices[0], vertices[5]));
            edges.Add(new Edge3d(vertices[1], vertices[2]));
            edges.Add(new Edge3d(vertices[1], vertices[4]));
            edges.Add(new Edge3d(vertices[1], vertices[5]));
            edges.Add(new Edge3d(vertices[2], vertices[3]));
            edges.Add(new Edge3d(vertices[2], vertices[4]));
            edges.Add(new Edge3d(vertices[3], vertices[4]));
            edges.Add(new Edge3d(vertices[3], vertices[5]));
            edges.Add(new Edge3d(vertices[4], vertices[5]));
            var facets = new List<Facet3d>();

            var facet0 = new Facet3d(new List<Point3d> { vertices[0], vertices[1], vertices[2] }, new List<Edge3d> { edges[0], edges[1], edges[4] });
            var facet1 = new Facet3d(new List<Point3d> { vertices[0], vertices[1], vertices[5] }, new List<Edge3d> { edges[0], edges[3], edges[6] });
            var facet2 = new Facet3d(new List<Point3d> { vertices[0], vertices[3], vertices[5] }, new List<Edge3d> { edges[3], edges[2], edges[10] });
            var facet3 = new Facet3d(new List<Point3d> { vertices[0], vertices[2], vertices[3] }, new List<Edge3d> { edges[1], edges[2], edges[7] });
            var facet4 = new Facet3d(new List<Point3d> { vertices[4], vertices[1], vertices[2] }, new List<Edge3d> { edges[4], edges[5], edges[8] });
            var facet5 = new Facet3d(new List<Point3d> { vertices[4], vertices[1], vertices[5] }, new List<Edge3d> { edges[5], edges[6], edges[11] });
            var facet6 = new Facet3d(new List<Point3d> { vertices[4], vertices[3], vertices[5] }, new List<Edge3d> { edges[9], edges[10], edges[11] });
            var facet7 = new Facet3d(new List<Point3d> { vertices[4], vertices[2], vertices[3] }, new List<Edge3d> { edges[7], edges[8], edges[9] });
            facets.Add(facet0);
            facets.Add(facet1);
            facets.Add(facet2);
            facets.Add(facet3);
            facets.Add(facet4);
            facets.Add(facet5);
            facets.Add(facet6);
            facets.Add(facet7);

            return new Polyhedron(vertices, edges, facets);
        }
    }
}
