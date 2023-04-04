using System;

namespace SatSys
{
    public static class SatDate
    {
        /// <summary>
        /// Start time of the system
        /// Jan, 1st, 2023, 0:00:00
        /// </summary>
        public static double startJulianDate => GetJulianDate(2023, 1, 1, 0, 0, 0);

        /// <summary>
        /// End time of the system
        /// Jan, 2nd, 2023, 0:00:00
        /// </summary>
        /// <returns></returns>
        public static double endJulianDate => GetJulianDate(2023, 1, 2, 0, 0, 0);

        public static double GetJulianDate(
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second
        )
        {
            DateTime date = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            double julianDate = date.ToOADate() + 2415018.5;
            return julianDate;
        }

        public static double GetJulianDate(DateTime date)
        {
            double julianDate = date.ToOADate() + 2415018.5;
            return julianDate;
        }

        public static DateTime GetDateTime(double julianDate)
        {
            return DateTime.FromOADate(julianDate - 2415018.5);
        }
    }
}
