// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Helpers;

public static class MathFunctions
{
    /// <summary>
    /// Standard deviation calculation.
    /// </summary>
    /// <param name="values">Values to calculate on.</param>
    /// <param name="useBesselCorrection">Used when we want "sample" stdev as opposed to "population".</param>
    public static double StdDev(IEnumerable<double> values, bool useBesselCorrection = true)
    {
        double avg = values.Average();
        double variance = values.Sum(v => Math.Pow(v - avg, 2)) / (useBesselCorrection ? values.Count() - 1 : values.Count());
        return Math.Sqrt(variance);
    }

    /// <summary>
    /// Calculates the Ordinary Least Squares (OLS) regression coefficient (slope), without an intercept.
    /// </summary>
    public static double OLS(double[] xs, double[] ys)
    {
        if (xs.Length != ys.Length)
            throw new ArgumentException("The length of xs and ys must be the same.");

        int n = xs.Length;
        double sumX2 = xs.Select(x => x * x).Sum();
        double sumXY = xs.Zip(ys, (x, y) => x * y).Sum();

        double m = sumXY / sumX2;
        return m;
    }

    /// <summary>
    /// Calculates the Ordinary Least Squares (OLS) regression coefficients, with an intercept.
    /// </summary>
    public static (double m, double b) OLS_WithIntercept(double[] xs, double[] ys)
    {
        if (xs.Length != ys.Length)
            throw new ArgumentException("The length of xs and ys must be the same.");

        int n = xs.Length;
        double sumX = xs.Sum();
        double sumY = ys.Sum();
        double sumX2 = xs.Select(x => x * x).Sum();
        double sumXY = xs.Zip(ys, (x, y) => x * y).Sum();

        double m = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        double b = (sumY - m * sumX) / n;

        return (m, b);
    }

}
