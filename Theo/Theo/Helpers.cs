
using Theo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Theo
{
    public static class Helpers
    {
        /// <summary>
        /// Strips off any value more specific than the interval in order to compare
        /// the datetimes.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="interval">daily, hourly, monthly, etc.</param>
        /// <returns></returns>
        public static int Compare(DateTime t1, DateTime t2, TickInterval interval)
        {
            return DateTime.Compare(
                t1.NormalizeToTickInterval(interval),
                t2.NormalizeToTickInterval(interval));
        }

        /// <summary>
        /// Standard ToString for DateTime objects.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string StandardToString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// Returns a datetime with anything more specific than the tick interval stripped.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static DateTime NormalizeToTickInterval(this DateTime dt, TickInterval interval)
        {
            if (interval != TickInterval.Day)
            {
                throw new NotImplementedException();
            }

            return dt.Date;
        }

        /// <summary>
        /// Gets the next trading day or current day.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetNextTradingDayInclusive(this DateTime dt)
        {
            while (!IsTradingDay(dt))
            {
                dt = dt.AddDays(1);
            }
            return dt;
        }

        /// <summary>
        /// Gets next trading day, never same day.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetNextTradingDay(this DateTime dt)
        {
            return dt.AddDays(1).GetNextTradingDayInclusive();
        }

        /// <summary>
        /// Gets the previous trading day before a certain date.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetPreviousTradingDayInclusive(this DateTime dt)
        {
            while (!IsTradingDay(dt))
            {
                dt = dt.AddDays(-1);
            }
            return dt;
        }

        /// <summary>
        /// Gets previous trading day, never same day.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetPreviousTradingDay(this DateTime dt)
        {
            return dt.AddDays(-1).GetPreviousTradingDayInclusive();
        }

        /// <summary>
        /// Gets n trading days before date.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static DateTime GetNthPreviousTradingDay(this DateTime dt, int n)
        {
            for (int i=0; i<n; i++)
            {
                dt = dt.GetPreviousTradingDay();
            }
            return dt;
        }

        /// <summary>
        /// Determines if this date is a federal holiday.
        /// </summary>
        /// <param name="date">This date</param>
        /// <returns>True if this date is a federal holiday</returns>
        public static bool IsTradingDay(this DateTime date)
        {
            // to ease typing
            int nthWeekDay = (int)(Math.Ceiling((double)date.Day / 7.0d));
            DayOfWeek dayName = date.DayOfWeek;
            bool isThursday = dayName == DayOfWeek.Thursday;
            bool isFriday = dayName == DayOfWeek.Friday;
            bool isMonday = dayName == DayOfWeek.Monday;
            bool isWeekend = dayName == DayOfWeek.Saturday || dayName == DayOfWeek.Sunday;

            if (isWeekend) return false;

            // New Years Day
            if (date.Month == 1 && date.Day == 1) return false;

            // MLK day (3rd monday in January)
            if (date.Month == 1 && isMonday && nthWeekDay == 3) return false;

            // President’s Day (3rd Monday in February)
            if (date.Month == 2 && isMonday && nthWeekDay == 3) return false;

            // Memorial Day (Last Monday in May)
            if (date.Month == 5 && isMonday && date.AddDays(7).Month == 6) return false;

            // Independence Day (July 4, or preceding Friday/following Monday if weekend)
            if ((date.Month == 7 && date.Day == 3 && isFriday) ||
                (date.Month == 7 && date.Day == 4 && !isWeekend) ||
                (date.Month == 7 && date.Day == 5 && isMonday)) return false;

            // Labor Day (1st Monday in September)
            if (date.Month == 9 && isMonday && nthWeekDay == 1) return false;

            // Thanksgiving Day (4th Thursday in November)
            if (date.Month == 11 && isThursday && nthWeekDay == 4) return false;

            // Christmas Day (December 25, or preceding Friday/following Monday if weekend))
            if ((date.Month == 12 && date.Day == 24 && isFriday) ||
                (date.Month == 12 && date.Day == 25 && !isWeekend) ||
                (date.Month == 12 && date.Day == 26 && isMonday)) return false;

            // Good friday
            if (date.IsGoodFriday()) return false;

            return true;
        }

        /// <summary>
        /// Returns whether the day is easter sunday.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsGoodFriday(this DateTime date)
        {
            int year = date.Year;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (c / 4) - ((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (h / 28) * (1 - (h / 28) * (29 / (h + 1)) * ((21 - g) / 11));

            int day = i - ((year + (year / 4) + i + 2 - c + (c / 4)) % 7) + 28;
            int month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            DateTime easterSunday = new DateTime(year, month, day);
            return date.Date.CompareTo(easterSunday.Date.AddDays(-2)) == 0;
        }

        /// <summary>
        /// Rounds a double.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Round(this double d)
        {
            return Math.Round(d, 6);
        }

        /// <summary>
        /// Compares doubles to six decimal places.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int CompareDoubles(double a, double b)
        {
            double diff = a - b;

            if (Math.Abs(diff) < .000001)
            {
                return 0;
            }
            return a < b ? -1 : 1;
        }

        /// <summary>
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
                standardDeviation = Math.Sqrt((sum) / (values.Count() - 1));
            }

            return standardDeviation;
        }
    }
}
