﻿using System;

static class FixMatrix4x8
{
    [ThreadStatic] private static Fix64[,] FixMatrix;

    public static bool Invert(in FixMatrix m, out FixMatrix r)
    {
        if (FixMatrix == null)
            FixMatrix = new Fix64[4, 8];
        Fix64[,] M = FixMatrix;

        M[0, 0] = m.M11;
        M[0, 1] = m.M21;
        M[0, 2] = m.M31;
        M[0, 3] = m.M41;
        M[1, 0] = m.M12;
        M[1, 1] = m.M22;
        M[1, 2] = m.M32;
        M[1, 3] = m.M42;
        M[2, 0] = m.M13;
        M[2, 1] = m.M23;
        M[2, 2] = m.M33;
        M[2, 3] = m.M43;
        M[3, 0] = m.M14;
        M[3, 1] = m.M24;
        M[3, 2] = m.M34;
        M[3, 3] = m.M44;

        M[0, 4] = Fix64.One;
        M[0, 5] = Fix64.Zero;
        M[0, 6] = Fix64.Zero;
        M[0, 7] = Fix64.Zero;
        M[1, 4] = Fix64.Zero;
        M[1, 5] = Fix64.One;
        M[1, 6] = Fix64.Zero;
        M[1, 7] = Fix64.Zero;
        M[2, 4] = Fix64.Zero;
        M[2, 5] = Fix64.Zero;
        M[2, 6] = Fix64.One;
        M[2, 7] = Fix64.Zero;
        M[3, 4] = Fix64.Zero;
        M[3, 5] = Fix64.Zero;
        M[3, 6] = Fix64.Zero;
        M[3, 7] = Fix64.One;


        if (!FixMatrix3x6.Gauss(M, 4, 8))
        {
            r = new FixMatrix();
            return false;
        }
        r = new FixMatrix(
            // m11...m14
            M[0, 4],
            M[0, 5],
            M[0, 6],
            M[0, 7],

            // m21...m24				
            M[1, 4],
            M[1, 5],
            M[1, 6],
            M[1, 7],

            // m31...m34
            M[2, 4],
            M[2, 5],
            M[2, 6],
            M[2, 7],

            // m41...m44
            M[3, 4],
            M[3, 5],
            M[3, 6],
            M[3, 7]
            );
        return true;
    }
}