using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace lab6
{
    public class Point3d //: IIdentifiable<long>
    {
        private static long nextId = 0;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        private long _Id;
        public long Id => _Id;
        public Vector3d Normal { get; set; }

        public Point3d(Point3d other) : this(other.X, other.Y, other.Z)
        {
        }

        public Point3d(double x, double y, double z) : this(x, y, z, nextId)
        {
            nextId += 1;
        }

        private Point3d(double x, double y, double z, long Id)
        {
            X = x;
            Y = y;
            Z = z;
            _Id = Id;
            Normal = new Vector3d();
        }

        public Vector3d ToVector3d() => new Vector3d(X, Y, Z, 1);

        public Point ToPoint() => new Point((int)X, (int)Y);

        public object Clone() => new Point3d(X, Y, Z, Id);

        public bool Equals(Point3d other) => Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
    public class DeptherizedPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Depth { get; set; }
        public double Intensivity { get; set; }

        public DeptherizedPoint(int x, int y, double depth, double intensivity)
        {
            X = x;
            Y = y;
            Depth = depth;
            Intensivity = intensivity;
        }

        public static DeptherizedPoint FromPoint3D(Point3d point, Point3d lightpoint)
        {
            var p = point.ToPoint();
            var intensivity = ComputeIntensivityLambert(point, lightpoint);
            return new DeptherizedPoint(p.X, p.Y, point.Z, intensivity);
        }

        private static double ComputeIntensivityLambert(Point3d direction, Point3d lightpoint)
        {
            var lightDirection = new Vector3d(direction.X - lightpoint.X, direction.Y - lightpoint.Y, direction.Z - lightpoint.Z);
            double intensivity = Math.Max(direction.Normal.DotProduct(lightDirection) / (direction.Normal.Length * lightDirection.Length), 0);
            return intensivity;
        }
    }

    public static class DeptherizetPointsMethods
    {
        public static DeptherizedPoint FetchLeftMost(this List<DeptherizedPoint> collection)
        {
            var leftMost = collection.First();
            for (int i = 1; i < collection.Count(); i++)
            {
                if (collection[i].X < leftMost.X)
                {
                    leftMost = collection[i];
                }
            }
            collection.Remove(leftMost);
            return leftMost;
        }

        public static DeptherizedPoint FetchRightMost(this List<DeptherizedPoint> collection)
        {
            var rightMost = collection.First();
            for (int i = 1; i < collection.Count(); i++)
            {
                if (collection[i].X > rightMost.X)
                {
                    rightMost = collection[i];
                }
            }
            collection.Remove(rightMost);
            return rightMost;
        }

        public static Point3d CommonCenter(this List<Polyhedron> polyhedrons)
        {
            double x = 0, y = 0, z = 0;
            int count = 0;
            foreach (var pol in polyhedrons)
            {
                var p = pol.Center;
                x += p.X;
                y += p.Y;
                z += p.Z;
                count++;
            }
            x /= count;
            y /= count;
            z /= count;
            return new Point3d(x, y, z);
        }
    }
}
