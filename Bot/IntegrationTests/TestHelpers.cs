using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests;

public static class TestHelpers
{
    public static void IsApproximately(this double actual, double expected)
    {
        int decimalPlaces = GetDecimalPlaces(expected);
        double tolerance = Math.Pow(10, -decimalPlaces);
        Assert.IsTrue(Math.Abs(actual - expected) < tolerance);
    }

    private static int GetDecimalPlaces(double number)
    {
        string numberString = number.ToString("G17"); // Preserve precision
        int decimalIndex = numberString.IndexOf('.');

        if (decimalIndex == -1) return 0; // No decimal places

        return numberString.Length - decimalIndex - 1;
    }
}
