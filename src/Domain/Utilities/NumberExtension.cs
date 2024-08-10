using System.Text;

namespace Domain.Utilities
{
    public static class NumberExtension
    {

        public static string ConvertPersianToEnglish(this string persianNumber)
        {
            if (string.IsNullOrWhiteSpace(persianNumber))
                return string.Empty;

            // Mapping Persian digits to English digits
            char[] persianDigits = { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };
            char[] englishDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            StringBuilder result = new StringBuilder();

            foreach (char c in persianNumber)
            {
                int index = Array.IndexOf(persianDigits, c);

                if (index >= 0)
                    result.Append(englishDigits[index]);
                else
                    result.Append(c);
            }

            return result.ToString();
        }
    }
}
