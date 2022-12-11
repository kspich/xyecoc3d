using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using lab6;

namespace lab6
{
    public class NearestOrdinateComparer : IComparer<DeptherizedPoint>
    {
        public int Compare(DeptherizedPoint lhs, DeptherizedPoint rhs)
        {
            return lhs.Y.CompareTo(rhs.Y);
        }
    }

    public class ColoredPointsGroupPair
    {
        public List<DeptherizedPoint> Left { get; }
        public List<DeptherizedPoint> Right { get; }

        public ColoredPointsGroupPair(List<DeptherizedPoint> left, List<DeptherizedPoint> right)
        {
            Left = left;
            Right = right;
        }
    }

    public static class TriangleRasterization
    {
        private const int IndexVertex1 = 0, IndexVertex2 = 1, IndexVertex3 = 2;

        public static IEnumerable<DeptherizedPoint> RasteriseTriangle(DeptherizedPoint v1, DeptherizedPoint v2, DeptherizedPoint v3)
        {
            var vertices = new DeptherizedPoint[] { v1, v2, v3 };
            Array.Sort(vertices, new NearestOrdinateComparer());

            var pointsGroupPairs = MakeBresenhamMovement(vertices[IndexVertex1], vertices[IndexVertex2])
                    .Concat(MakeBresenhamMovement(vertices[IndexVertex2], vertices[IndexVertex3]))
                    .Zip(MakeBresenhamMovement(vertices[IndexVertex1], vertices[IndexVertex3]), (pointsGroupLeft, pointsGroupRight) => new ColoredPointsGroupPair(pointsGroupLeft, pointsGroupRight));
            foreach (var pointsGroupPair in pointsGroupPairs)
            {
                var pointsGroupLeft = pointsGroupPair.Left;
                var pointsGroupRight = pointsGroupPair.Right;

                var leftSideMostRightPoint = pointsGroupLeft.FetchRightMost();
                var rightSideMostLeftPoint = pointsGroupRight.FetchLeftMost();

                int coordinateDeltaX = Math.Abs(leftSideMostRightPoint.X - rightSideMostLeftPoint.X);
                yield return leftSideMostRightPoint;
                int y = leftSideMostRightPoint.Y;
                for (int x = leftSideMostRightPoint.X + 1; x < rightSideMostLeftPoint.X; x += 1)
                {
                    var currentDepth = ApplyLinearInterpolation(leftSideMostRightPoint.Depth, rightSideMostLeftPoint.Depth, x, rightSideMostLeftPoint.X, coordinateDeltaX);
                    var currentIntensivity = ApplyLinearInterpolation(leftSideMostRightPoint.Intensivity, rightSideMostLeftPoint.Intensivity, x, rightSideMostLeftPoint.X, coordinateDeltaX);
                    yield return new DeptherizedPoint(x, y, currentDepth, currentIntensivity);
                }
                for (int x = rightSideMostLeftPoint.X + 1; x < leftSideMostRightPoint.X; x += 1)
                {
                    var currentDepth = ApplyLinearInterpolation(leftSideMostRightPoint.Depth, rightSideMostLeftPoint.Depth, x, rightSideMostLeftPoint.X, coordinateDeltaX);
                    var currentIntensivity = ApplyLinearInterpolation(leftSideMostRightPoint.Intensivity, rightSideMostLeftPoint.Intensivity, x, rightSideMostLeftPoint.X, coordinateDeltaX);
                    yield return new DeptherizedPoint(x, y, currentDepth, currentIntensivity);
                }
                yield return rightSideMostLeftPoint;

                foreach (var pointGroupLeft in pointsGroupLeft)
                {
                    yield return pointGroupLeft;
                }
                foreach (var pointGroupRight in pointsGroupRight)
                {
                    yield return pointGroupRight;
                }
            }
        }

        private static IEnumerable<List<DeptherizedPoint>> MakeBresenhamMovement(DeptherizedPoint v1, DeptherizedPoint v2)
        {
            int x1 = v1.X, x2 = v2.X;
            int y1 = v1.Y, y2 = v2.Y;
            double d1 = v1.Depth, d2 = v2.Depth;
            double i1 = v1.Intensivity, i2 = v2.Intensivity;

            int diffX = x2 - x1;
            int diffY = y2 - y1;

            int growthDirectionX = diffX > 0 ? 1 : -1;
            int growthDirectionY = diffY > 0 ? 1 : -1;

            int deltaX = Math.Abs(diffX);
            int deltaY = Math.Abs(diffY);

            int axisGrowthDirection = deltaX > deltaY ? 1 : -1;

            int decision = 2 * axisGrowthDirection * (deltaY - deltaX);

            int refereeY = Math.Max(axisGrowthDirection, 0);
            int refereeX = Math.Min(axisGrowthDirection, 0);

            int interpolationEndCoordinate = x2 * refereeY - y2 * refereeX;
            int interpolationCoordinateDelta = deltaX * refereeY - deltaY * refereeX;

            int previousY = y1;

            var currentPointsQueue = new Queue<DeptherizedPoint>();

            List<DeptherizedPoint> pointsGroup;

            while (x1 != x2 || y1 != y2)
            {
                var depthCurrent = ApplyLinearInterpolation(d1, d2, x1 * refereeY - y1 * refereeX, interpolationEndCoordinate, interpolationCoordinateDelta);
                var intensivityCurrent = Math.Max(ApplyLinearInterpolation(i1, i2, x1 * refereeY - y1 * refereeX, interpolationEndCoordinate, interpolationCoordinateDelta), 0);
                var pointCurrent = new DeptherizedPoint(x1, y1, depthCurrent, intensivityCurrent);

                currentPointsQueue.Enqueue(pointCurrent);

                if (pointCurrent.Y != previousY)
                {
                    previousY = pointCurrent.Y;
                    pointsGroup = new List<DeptherizedPoint>();
                    while (currentPointsQueue.Count > 1)
                    {
                        pointsGroup.Add(currentPointsQueue.Dequeue());
                    }
                    yield return pointsGroup;
                }

                x1 += growthDirectionX * refereeY;
                y1 -= growthDirectionY * refereeX;

                if (decision < 0)
                {
                    decision += 2 * (deltaY * refereeY - deltaX * refereeX);
                }
                else
                {
                    y1 += growthDirectionY * refereeY;
                    x1 -= growthDirectionX * refereeX;
                    decision += 2 * axisGrowthDirection * (deltaY - deltaX);
                }
            }

            pointsGroup = new List<DeptherizedPoint>();
            while (currentPointsQueue.Count > 0)
            {
                pointsGroup.Add(currentPointsQueue.Dequeue());
            }
            if (pointsGroup.Count > 0)
            {
                yield return pointsGroup;
            }
        }

        private static double ApplyLinearInterpolation(double beginDepth, double endDepth, int currentCoordinate, int endCoordinate, int coordinateDelta)
        {
            int coordinateDeltaCurrent = Math.Abs(currentCoordinate - endCoordinate);
            double depth = beginDepth * coordinateDeltaCurrent / coordinateDelta + endDepth * (coordinateDelta - coordinateDeltaCurrent) / coordinateDelta;
            return depth;
        }
    }
}
