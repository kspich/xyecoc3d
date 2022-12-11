using lab6;
using System;

namespace lab6
{
    public class Edge3d : IIdentifiable<long>, IEquatable<Edge3d>
    {
        private static long nextId = 0;
        public Point3d Begin { get; set; }
        public Point3d End { get; set; }
        private long _Id;
        public long Id => _Id;

        public Edge3d(Point3d begin, Point3d end, long? id = null)
        {
            Begin = begin;
            End = end;
            if (id == null)
            {
                this._Id = nextId;
                nextId += 1;
            }
            else
            {
                this._Id = id.Value;
            }
        }

        public Vector3d ToVector3d()
        {
            double x = End.X - Begin.X;
            double y = End.Y - Begin.Y;
            double z = End.Z - Begin.Z;
            return new Vector3d(x, y, z);
        }

        public bool Equals(Edge3d other) => Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
