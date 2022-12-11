using lab6;
using System;
using System.Collections.Generic;

namespace lab6
{
    public static class Const
    {
        public static readonly double sqrt2 = Math.Sqrt(2);
        public static readonly double sqrt3 = Math.Sqrt(3);
        public static readonly double sqrt6 = sqrt2 * sqrt3;
    }
    public enum Projection
    {
        Perspective, Axonometric
    }

    public enum Axis
    {
        OX,
        OY,
        OZ
    }

    public static class ProjectionMethods
    {
        public static Matrix CreateMatrix(this Projection projectionType)
        {
            switch (projectionType)
            {
                case Projection.Perspective:
                    return AffineTransformations.MakePerspectiveProjectionMatrix(1000);
                case Projection.Axonometric:
                    return AffineTransformations.MakeAxonometricProjectionMatrix();
                default:
                    throw new ArgumentException("Unknown projection type");
            }
        }

        public static string ProjectionName(this Projection projectionType)
        {
            switch (projectionType)
            {
                case Projection.Perspective:
                    return "Перспективная";
                case Projection.Axonometric:
                    return "Аксонометрическая";
                default:
                    throw new ArgumentException("Unknown projection type");
            }
        }
    }

    public static class AxisMethods
    {
        public static string GetAxisName(this Axis axisType)
        {
            switch (axisType)
            {
                case Axis.OX:
                    return "OX";
                case Axis.OY:
                    return "OY";
                case Axis.OZ:
                    return "OZ";
                default:
                    throw new ArgumentException("Unknown axis type");
            }
        }

        public static Matrix CreateRotationMatrix(this Axis axisType, double degrees)
        {
            switch (axisType)
            {
                case Axis.OX:
                    return AffineTransformations.MakeXRotationMatrix(degrees);
                case Axis.OY:
                    return AffineTransformations.MakeYRotationMatrix(degrees);
                case Axis.OZ:
                    return AffineTransformations.MakeZRotationMatrix(degrees);
                default:
                    throw new ArgumentException("Unknown axis type");
            }
        }
    }

    public static class TransformationMethods
    {
        public static void ApplyTransformationInplace(Polyhedron polyhedron, Matrix transformation)
        {
            TransformMultiplePointsInplace(polyhedron.Vertices, transformation);
            TransformPointInplace(polyhedron.Center, transformation);
            foreach (var vertex in polyhedron.Vertices)
            {
                TransformVectorInplace(vertex.Normal, transformation);
                var v = vertex.Normal;
                Console.WriteLine($"{v.X}, {v.Y}, {v.Z}");
            }
            foreach (var facet in polyhedron.Facets)
            {
                TransformVectorInplace(facet.Normal, transformation);
            }
        }

        public static void TransformMultiplePointsInplace(List<Point3d> points, Matrix transformation)
            => points.ForEach(p => TransformPointInplace(p, transformation));

        public static void TransformPointInplace(Point3d point, Matrix transformation)
        {
            var product = point.ToVector3d() * transformation;
            double x = product[0, 0];
            double y = product[0, 1];
            double z = product[0, 2];
            double w = product[0, 3];
            var transformedPoint = new Point3d(x / w, y / w, z / w);
            point.X = transformedPoint.X;
            point.Y = transformedPoint.Y;
            point.Z = transformedPoint.Z;
        }

        public static void TransformVectorInplace(Vector3d vector, Matrix transformation)
        {
            var product = vector * transformation;
            double x = product[0, 0];
            double y = product[0, 1];
            double z = product[0, 2];
            var transformedVector = new Vector3d(x, y, z);
            vector.X = transformedVector.X;
            vector.Y = transformedVector.Y;
            vector.Z = transformedVector.Z;
        }
    }

    public static class AffineTransformations
    {   
        public static Matrix MakeXYZRotationMatrix(double degreesX, double degreesY, double degreesZ)
        {
            return MakeXRotationMatrix(degreesX)
                * MakeYRotationMatrix(degreesY)
                * MakeZRotationMatrix(degreesZ);
        }

        public static Matrix MakeXRotationMatrix(double degrees)
        {
            double radians = degrees * Math.PI / 180;

            double angleCos = Math.Cos(radians);
            double angleSin = Math.Sin(radians);

            var elements = new double[,] {
                { 1, 0, 0, 0 },
                { 0, angleCos, -angleSin, 0 },
                { 0, angleSin, angleCos, 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeYRotationMatrix(double degrees)
        {
            double radians = degrees * Math.PI / 180;

            double angleCos = Math.Cos(radians);
            double angleSin = Math.Sin(radians);

            var elements = new double[,] {
                { angleCos, 0, angleSin, 0 },
                { 0, 1, 0, 0 },
                { -angleSin, 0, angleCos, 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeZRotationMatrix(double degrees)
        {
            double radians = degrees * Math.PI / 180;

            double angleCos = Math.Cos(radians);
            double angleSin = Math.Sin(radians);

            var elements = new double[,] {
                { angleCos, -angleSin, 0, 0 },
                { angleSin,  angleCos, 0, 0 },
                {  0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeRotateAroundEdgeMatrix(Edge3d edge, double angle)
        {
            double radians = angle * Math.PI / 180;

            double angleCos = Math.Cos(radians);
            double angleSin = Math.Sin(radians);

            var vector3D = edge.ToVector3d();

            double l = vector3D.X / vector3D.Length;
            double m = vector3D.Y / vector3D.Length;
            double n = vector3D.Z / vector3D.Length;

            double lSqr = l * l; 
            double mSqr = m * m;
            double nSqr = n * n;

            var elements = new double[,]
            {
                { lSqr + angleCos*(1 - lSqr), l * (1 - angleCos)*m - n*angleSin, l*(1 - angleCos)*n+m*angleSin, 0 },
                { l*(1 - angleCos)*m + n * angleSin,  mSqr + angleCos*(1 - mSqr), m*(1 - angleCos)*n - l*angleSin, 0 },
                {l*(1 - angleCos)*n - m * angleSin, m*(1 - angleCos)*n + l*angleSin, nSqr + angleCos*(1 - nSqr), 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeScalingMatrix(double mx, double my, double mz)
        {
            var elements = new double[,] {
                { mx, 0, 0, 0 },
                { 0, my, 0, 0 },
                { 0, 0, mz, 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeTranslationMatrix(double dx, double dy, double dz)
        {
            var elements = new double[,] {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { dx, dy, dz, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakePerspectiveProjectionMatrix(double c)
        {
            var elements = new double[,] {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 1/c },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeAxonometricProjectionMatrix()
        {
            var elements = new double[,] {
                { 1 / Const.sqrt2, 1 / Const.sqrt6, 1 / Const.sqrt3, 0 },
                { 0, 2 / Const.sqrt6, -1 / Const.sqrt3, 0 },
                { -1 / Const.sqrt2, 1 / Const.sqrt6, 1 / Const.sqrt3, 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeXYReflectionMatrix()
        {
            var elements = new double[,] {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeYZReflectionMatrix()
        {
            var elements = new double[,] {
                { -1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }

        public static Matrix MakeZXReflectionMatrix()
        {
            var elements = new double[,] {
                { 1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            return new Matrix(elements);
        }
    }
}

