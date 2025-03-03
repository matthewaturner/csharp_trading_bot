using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Helpers;

public static class MathHelpers
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
}
