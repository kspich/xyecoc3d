using System;
using System.Collections.Generic;
using System.Drawing;

namespace lab6
{
    public static class Bresenham
    {
        public static void DrawLine(this Bitmap drawingSurface, Point p1, Point p2, Color color)
        {
            using (var fastDrawingSurface = new FastBitmap(drawingSurface))
                fastDrawingSurface.DrawLine(p1, p2, color);
        }

        public static void DrawLine(this FastBitmap fastDrawingSurface, Point p1, Point p2, Color color)
            => fastDrawingSurface.DrawPoints(GetBresenhamPoints(p1, p2), color);

        public static void DrawLineDashed(this Bitmap drawingSurface, Point p1, Point p2, Color color, int dashLength)
        {
            using (var fastDrawingSurface = new FastBitmap(drawingSurface))
                fastDrawingSurface.DrawPointsInterruptly(GetBresenhamPoints(p1, p2), color, dashLength);
        }

        public static void DrawLineDashed(this FastBitmap fastDrawingSurface, Point p1, Point p2, Color color, int dashLength)
            => fastDrawingSurface.DrawPointsInterruptly(GetBresenhamPoints(p1, p2), color, dashLength);

        public static void DrawPoints(this FastBitmap fastDrawingSurface, IEnumerable<Point> points, Color color)
        {
            foreach (var p in points)
            {
                fastDrawingSurface.SetPixelSafely(p, color);
            }
        }

        public static void DrawPointsInterruptly(this FastBitmap fastDrawingSurface, IEnumerable<Point> points, Color color, int interruptionLength)
        {
            EnsureInterruptionLengthIsPositive(interruptionLength);
            int currentInterruptionLength = 0;
            bool interrupt = false;
            foreach (var point in points)
            {
                if (!interrupt)
                {
                    if (currentInterruptionLength < interruptionLength)
                    {
                        fastDrawingSurface.SetPixelSafely(point, color);
                        currentInterruptionLength += 1;
                    }
                    else
                    {
                        interrupt = true;
                    }
                }
                else
                {
                    if (currentInterruptionLength > 0)
                    {
                        currentInterruptionLength -= 1;
                    }
                    else
                    {
                        interrupt = false;
                    }
                }
            }
        }

        private static void SetPixelSafely(this FastBitmap fastDrawingSurface, Point p, Color color)
        {
            if (fastDrawingSurface.IsPointOnSurface(p))
            {
                fastDrawingSurface[p.X, p.Y] = color;
            }
        }

        private static bool IsPointOnSurface(this FastBitmap fastDrawingSurface, Point p)
        {
            bool isOnX = 0 <= p.X && p.X <= fastDrawingSurface.Width;
            bool isOnY = 0 <= p.Y && p.Y <= fastDrawingSurface.Height;
            return isOnX && isOnY;
        }

        private static void EnsureInterruptionLengthIsPositive(int interruptionLength)
        {
            if (interruptionLength <= 0)
            {
                throw new ArgumentException($"Negative: {interruptionLength}");
            }
        }

        public static IEnumerable<Point> GetBresenhamPoints(Point p1, Point p2)
            => GetBresenhamPoints(p1.X, p1.Y, p2.X, p2.Y);

        public static IEnumerable<Point> GetBresenhamPoints(int x1, int y1, int x2, int y2)
        {
            int diffX = x2 - x1;
            int diffY = y2 - y1;

            int growthDirectionX = diffX > 0 ? 1 : -1;
            int growthDirectionY = diffY > 0 ? 1 : -1;

            int deltaX = Math.Abs(diffX);
            int deltaY = Math.Abs(diffY);

            // Определяем gradient <= 1 или gradient > 1
            int axisGrowthDirection = deltaX > deltaY ? 1 : -1;

            int decision = 2 * axisGrowthDirection * (deltaY - deltaX);

            int refereeY = Math.Max(axisGrowthDirection, 0);
            int refereeX = Math.Min(axisGrowthDirection, 0);

            while (x1 != x2 || y1 != y2)
            {
                yield return new Point(x1, y1);

                x1 += growthDirectionX * refereeY;  // gradient <= 1
                y1 -= growthDirectionY * refereeX;  // gradient > 1

                if (decision < 0)
                {
                    decision += 2 * (deltaY * refereeY - deltaX * refereeX);
                }
                else
                {
                    y1 += growthDirectionY * refereeY;  // gradient <= 1
                    x1 -= growthDirectionX * refereeX;  // gradient > 1
                    decision += 2 * axisGrowthDirection * (deltaY - deltaX);
                }
            }
        }
    }
}
