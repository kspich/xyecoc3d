using lab6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab6
{
    // https://studfile.net/preview/7496068/page:28/
    public static class BackfaceCulling
    {
        public static Polyhedron RemoveBackFacets(Polyhedron polyhoderon, Point3d viewpoint)
        {
            var points = new HashSet<Point3d>();
            var edges = new HashSet<Edge3d>();
            var facets = new List<Facet3d>();
            var center = polyhoderon.Center;
            foreach (var facet in polyhoderon.Facets)
            {
                var normal = ComputeNormal(facet);
                var viewDirection = new Vector3d(1, 1, -1000);
                var dot = normal.DotProduct(viewDirection);
                if (dot < 0)
                {
                    facets.Add(facet);
                }
            }
            foreach (var facet in facets)
            {
                foreach (var p in facet.Points)
                {
                    points.Add(p);
                }
                foreach (var e in facet.Edges)
                {
                    edges.Add(e);
                }
            }
            return new Polyhedron(points.ToList(), edges.ToList(), facets);
        }

        private static Vector3d ComputeNormal(Facet3d facet)
        {
            var vec1 = facet.Edges[0].ToVector3d();
            var vec2 = facet.Edges[1].ToVector3d();
            var normal = vec1.CrossProduct(vec2);
            return normal;
        }
    }
}
