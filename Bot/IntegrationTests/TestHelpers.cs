// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

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
