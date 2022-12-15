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
        Tetrahedron, Oktahedron, Geksahedron, //Ikosahedron, Dodahedron
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
               // case PolyhedronType.Ikosahedron:
                  //  return "Икосаэдр";
               // case PolyhedronType.Dodahedron:
                 //   return "Додекаэдр";
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
            var v = new List<Point3d>();
            for (int i = 1; i <= 3; i++)
            {
                v.Add(P.Vertices[i]);
            }
            v.Add(P.Vertices[7]);
            var e = new List<Edge3d>();
            for (int i = 0; i <= 3; i++)
            {
                for (int j = i + 1; j <= 3; j++)
                {
                    e.Add(new Edge3d(v[i], v[j]));
                }
            }
            var facets = new List<Facet3d>() {
                new Facet3d(new List<Point3d> { v[0], v[1], v[3] }, new List<Edge3d> { e[0], e[4], e[2] }),
                new Facet3d(new List<Point3d> { v[0], v[3], v[2] }, new List<Edge3d> { e[2], e[5], e[1] }),
                new Facet3d(new List<Point3d> { v[1], v[2], v[3] }, new List<Edge3d> { e[3], e[4], e[5] }),
                new Facet3d(new List<Point3d> { v[0], v[1], v[2] }, new List<Edge3d> { e[1], e[0], e[3] })
            };
            return new Polyhedron(v, e, facets);
        }

        public static Polyhedron Geksahedron()
        {
            var p = new List<Point3d>() {
                new Point3d(0, 0, 0),
                new Point3d(0, 0, 300),
                new Point3d(0, 300, 0),
                new Point3d(300, 0, 0),
                new Point3d(300, 0, 300),
                new Point3d(300, 300, 0),
                new Point3d(0, 300, 300),
                new Point3d(300, 300, 300)
            };
            var e = new List<Edge3d>() {
                new Edge3d(p[0], p[1]),
                new Edge3d(p[0], p[2]),
                new Edge3d(p[0], p[3]),
                new Edge3d(p[1], p[4]),
                new Edge3d(p[1], p[6]),
                new Edge3d(p[2], p[5]),
                new Edge3d(p[2], p[6]),
                new Edge3d(p[3], p[5]),
                new Edge3d(p[3], p[4]),
                new Edge3d(p[4], p[7]),
                new Edge3d(p[5], p[7]),
                new Edge3d(p[6], p[7])
            };
            var facets = new List<Facet3d>() {
                new Facet3d(new List<Point3d> { p[1], p[4], p[7], p[6] },
                            new List<Edge3d>  { e[3], e[4], e[11], e[9] }),
                new Facet3d(new List<Point3d> { p[0], p[2], p[5], p[3] },
                            new List<Edge3d>  { e[2], e[1], e[5], e[7] }),
                new Facet3d(new List<Point3d> { p[0], p[3], p[4], p[1] },
                            new List<Edge3d>  { e[2], e[8], e[3], e[0] }),
                new Facet3d(new List<Point3d> { p[2], p[5], p[7], p[6] },
                            new List<Edge3d>  { e[10], e[11], e[6], e[5] }),
                new Facet3d(new List<Point3d> { p[3], p[4], p[7], p[5] },
                            new List<Edge3d>  { e[8], e[9], e[10], e[7] }),
                new Facet3d(new List<Point3d> { p[0], p[1], p[6], p[2] },
                            new List<Edge3d>  { e[0], e[4], e[6], e[1] })
            };
            return new Polyhedron(p, e, facets);
        }

        public static Polyhedron Oktahedron()
        {
            Polyhedron P = Geksahedron();
            var v = new List<Point3d>();
            for (int i = 4; i <= 6; i++)
            {
                v.Add(Middle(P.Vertices[0], P.Vertices[i]));
            }
            for (int i = 1; i <= 3; i++)
            {
                v.Add(Middle(P.Vertices[7], P.Vertices[i]));
            }

            var e = new List<Edge3d>();
            for (int i = 1; i <= 3; i++)
            {
                e.Add(new Edge3d(v[0], v[i]));
            }
            e.Add(new Edge3d(v[0], v[5]));
            e.Add(new Edge3d(v[1], v[2]));
            e.Add(new Edge3d(v[1], v[4]));
            e.Add(new Edge3d(v[1], v[5]));
            e.Add(new Edge3d(v[2], v[3]));
            e.Add(new Edge3d(v[2], v[4]));
            e.Add(new Edge3d(v[3], v[4]));
            e.Add(new Edge3d(v[3], v[5]));
            e.Add(new Edge3d(v[4], v[5]));
            var facets = new List<Facet3d>() {
                new Facet3d(new List<Point3d> { v[0], v[1], v[2] },
                            new List<Edge3d>  { e[0], e[1], e[4] }),
                new Facet3d(new List<Point3d> { v[0], v[1], v[5] },
                            new List<Edge3d>  { e[0], e[3], e[6] }),
                new Facet3d(new List<Point3d> { v[0], v[3], v[5] },
                            new List<Edge3d>  { e[3], e[2], e[10] }),
                new Facet3d(new List<Point3d> { v[0], v[2], v[3] },
                            new List<Edge3d>  { e[1], e[2], e[7] }),
                new Facet3d(new List<Point3d> { v[4], v[1], v[2] },
                            new List<Edge3d>  { e[4], e[5], e[8] }),
                new Facet3d(new List<Point3d> { v[4], v[1], v[5] },
                            new List<Edge3d>  { e[5], e[6], e[11] }),
                new Facet3d(new List<Point3d> { v[4], v[3], v[5] },
                            new List<Edge3d>  { e[9], e[10], e[11] }),
                new Facet3d(new List<Point3d> { v[4], v[2], v[3] },
                            new List<Edge3d>  { e[7], e[8], e[9] })
            };
            return new Polyhedron(v, e, facets);
        }
    }
}
