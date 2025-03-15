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
    /// Standard deviation calculation.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double StdDev(IEnumerable<double> values)
    {
        double avg = values.Average();
        return Math.Sqrt(values.Sum(v => Math.Pow(v - avg, 2)) / (values.Count() - 1));
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
