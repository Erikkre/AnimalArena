
using System;
using UnityEngine;
using Random = System.Random;

public class MathHelpers{
    public static double NextGaussianDouble(Random r)
    {
    double u, v, s;

        do
    {
        u = 2.0 * r.NextDouble() - 1.0;
        v = 2.0 * r.NextDouble() - 1.0;
        s = u * u + v * v;
    }
    while (s >= 1); //was s>=1

    double fac = Math.Sqrt(-2.0 * Math.Log(s) / s);

    return u * fac;
    }
}