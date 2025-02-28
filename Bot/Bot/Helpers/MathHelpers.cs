using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Helpers
{
    public static class MathHelpers
    {
        /// Standard deviation calculation.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal StandardDeviation(IEnumerable<decimal> values)
        {
            decimal standardDeviation = 0;

            if (values.Any())
            {
                // Compute the average.     
                decimal avg = values.Average();

                // Perform the Sum of (value-avg)_2_2.      
                decimal sum = values.Sum(d => (decimal)Math.Pow((double)(d - avg), 2));

                // Put it all together.      
                standardDeviation = (decimal)Math.Sqrt((double)sum / (values.Count() - 1));
            }

            return standardDeviation;
        }
    }
}
