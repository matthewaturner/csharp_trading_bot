using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests;

public static class TestHelpers
{
    public static void IsApproximately(this double actual, decimal expected)
    {
        string expectedStr = expected.ToString("G17");
        string actualStr = actual.ToString("G17").Substring(0, expectedStr.Length);
        Assert.That(actualStr.Equals(expectedStr));
    }
}
