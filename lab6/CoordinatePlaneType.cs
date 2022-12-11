using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab6
{
    public enum CoordinatePlaneType
    {
        XY,
        YZ,
        ZX,
    }

    public static class CoordinatePlaneTypeExtensionMethods
    {
        public static Matrix CreateReflectionMatrix(this CoordinatePlaneType coordinatePlaneType)
        {
            switch (coordinatePlaneType)
            {
                case CoordinatePlaneType.XY:
                    return AffineTransformations.MakeXYReflectionMatrix();
                case CoordinatePlaneType.YZ:
                    return AffineTransformations.MakeYZReflectionMatrix();
                case CoordinatePlaneType.ZX:
                    return AffineTransformations.MakeZXReflectionMatrix();
                default:
                    throw new ArgumentException("Unknown coordinate plane type");
            }
        }

        public static string GetCoordinatePlaneName(this CoordinatePlaneType coordinatePlaneType)
        {
            switch (coordinatePlaneType)
            {
                case CoordinatePlaneType.XY:
                    return "XY";
                case CoordinatePlaneType.YZ:
                    return "YZ";
                case CoordinatePlaneType.ZX:
                    return "ZX";
                default:
                    throw new ArgumentException("Unknown coordinate plane type");
            }
        }
    }
}
