using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Helpers
{
    public static class MathHelpers
    {
        /// <summary>
        /// Polymorphism.
        /// </summary>
        public static decimal StandardDeviation(IEnumerable<decimal> values)
        {
            return (decimal)StandardDeviation(values.Select(v => (double)v));
        }

        /// Standard deviation calculation.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double StandardDeviation(IEnumerable<double> values)
        {
            double standardDeviation = 0;

            if (values.Any())
            {
                // Compute the average.     
                double avg = values.Average();

                // Perform the Sum of (value-avg)_2_2.      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));

                // Put it all together.      
                standardDeviation = Math.Sqrt(sum / (values.Count() - 1));
            }

            return standardDeviation;
        }
    }
}
