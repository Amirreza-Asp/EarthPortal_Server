using System.Globalization;

namespace Domain.Utilities
{
    public static class DateTimeExtension
    {
        public static DateTime ConvertShamsiStringToMiladiDateTime(String shamsiDate)
        {

            var pc = new PersianCalendar();
            var parts = shamsiDate.Split('/');

            var hour = Convert.ToInt32(parts[0]);
            var min = Convert.ToInt32(parts[1]);
            var sec = Convert.ToInt32(parts[2]);

            return new DateTime(hour, min, sec, pc);
        }

        public static DateTime GetFirstDayOfMonth()
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            DateTime currentDate = DateTime.Now;

            int persianYear = persianCalendar.GetYear(currentDate);
            int persianMonth = persianCalendar.GetMonth(currentDate);

            DateTime firstDayOfPersianMonth = persianCalendar.ToDateTime(persianYear, persianMonth, 1, 0, 0, 0, 0);

            return firstDayOfPersianMonth;
        }

        public static DateTime GetLastDayOfMonth()
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            DateTime currentDate = DateTime.Now;

            int persianYear = persianCalendar.GetYear(currentDate);
            int persianMonth = persianCalendar.GetMonth(currentDate);


            int daysInPersianMonth = persianCalendar.GetDaysInMonth(persianYear, persianMonth);

            DateTime lastDayOfPersianMonth = persianCalendar.ToDateTime(persianYear, persianMonth, daysInPersianMonth, 0, 0, 0, 0);

            return lastDayOfPersianMonth;
        }

        public static DateTime GetFirstDayOfYear()
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            DateTime currentDate = DateTime.Now;

            int persianYear = persianCalendar.GetYear(currentDate);

            DateTime firstDayOfPersianMonth = persianCalendar.ToDateTime(persianYear, 1, 1, 0, 0, 0, 0);

            return firstDayOfPersianMonth;
        }

        public static DateTime GetLastDayOfYear()
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            DateTime currentDate = DateTime.Now;

            int persianYear = persianCalendar.GetYear(currentDate);
            int daysInPersianMonth = persianCalendar.GetDaysInMonth(persianYear, 12);

            DateTime lastDayOfPersianMonth = persianCalendar.ToDateTime(persianYear, 12, daysInPersianMonth, 0, 0, 0, 0);

            return lastDayOfPersianMonth;
        }
    }
}
