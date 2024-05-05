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
    }
}
