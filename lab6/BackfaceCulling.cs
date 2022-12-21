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
            var points = new List<Point3d>();
            var edges = new List<Edge3d>();
            var facets = new List<Facet3d>();
            var center = polyhoderon.Center;
            Point3d proec = center - viewpoint;              // вектор проекции
            foreach (var facet in polyhoderon.Facets)        // для каждой грани ищем вектор нормали
            {
                var norm = ComputeNormal(facet);              
                var scalar = norm.X * proec.X + norm.Y * proec.Y + norm.Z * proec.Z;
                var prodLength = Math.Sqrt(norm.X * norm.X + norm.Y * norm.Y + norm.Z * norm.Z) * Math.Sqrt(proec.X * proec.X + proec.Y * proec.Y + proec.Z * proec.Z);
                var cos = 0.0;
                if (prodLength != 0)
                    cos = scalar / prodLength;
                if (cos > 0)
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

            return new Polyhedron(points, edges, facets);
        }

        private static Vector3d ComputeNormal(Facet3d facet)
        {
            var vec1 = facet.Edges[0].ToVector3d();
            var vec2 = facet.Edges[1].ToVector3d();
            if (vec2.X == 0 && vec2.Y == 0 && vec2.Z == 0)
            {
                vec2 = facet.Edges[2].ToVector3d();
            }
            var normal = vec1.CrossProduct(vec2);
            return normal;
        }
    }
}
