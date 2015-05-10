using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUutilities;

namespace Asteroids
{
    static class MatrixHelpers
    {
        public static Microsoft.Xna.Framework.Vector3 VectorToVector(BEPUutilities.Vector3 vector)
        {
            return new Microsoft.Xna.Framework.Vector3(){X = vector.X,
                                                         Y = vector.Y,
                                                         Z = vector.Z};
        }

        public static BEPUutilities.Vector3 VectorToVector( Microsoft.Xna.Framework.Vector3 vector)
        {
            return new BEPUutilities.Vector3(){X = vector.X,
                                               Y = vector.Y,
                                               Z = vector.Z};
        }

        public static Microsoft.Xna.Framework.Matrix MatrixToMatrix(BEPUutilities.Matrix matrix)
        {
            return new Microsoft.Xna.Framework.Matrix()
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M13 = matrix.M13, 
                M14 = matrix.M14,
                M21 = matrix.M21,
                M22 = matrix.M22,
                M23 = matrix.M23,
                M24 = matrix.M24,
                M31 = matrix.M31,
                M32 = matrix.M32,
                M33 = matrix.M33,
                M34 = matrix.M34,
                M41 = matrix.M41,
                M42 = matrix.M42,
                M43 = matrix.M43,
                M44 = matrix.M44,
            };
        }

        public static BEPUutilities.Matrix MatrixToMatrix(Microsoft.Xna.Framework.Matrix matrix)
        {
            return new BEPUutilities.Matrix()
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M13 = matrix.M13,
                M14 = matrix.M14,
                M21 = matrix.M21,
                M22 = matrix.M22,
                M23 = matrix.M23,
                M24 = matrix.M24,
                M31 = matrix.M31,
                M32 = matrix.M32,
                M33 = matrix.M33,
                M34 = matrix.M34,
                M41 = matrix.M41,
                M42 = matrix.M42,
                M43 = matrix.M43,
                M44 = matrix.M44,
            };
        }

        public static Microsoft.Xna.Framework.Quaternion QuaternionToQuaternion(BEPUutilities.Quaternion quat)
        {
            return new Microsoft.Xna.Framework.Quaternion()
            {
                W = quat.W,
                X = quat.X,
                Y = quat.Y,
                Z = quat.Z
            };
        }

        public static BEPUutilities.Quaternion QuaternionToQuaternion(Microsoft.Xna.Framework.Quaternion quat)
        {
            return new BEPUutilities.Quaternion()
            {
                W = quat.W,
                X = quat.X,
                Y = quat.Y,
                Z = quat.Z
            };
        }
    }
}
