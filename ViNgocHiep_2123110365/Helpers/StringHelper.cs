using System.Text;
using System.Text.RegularExpressions;

namespace ViNgocHiep_2123110365.Helpers
{
    public static class StringHelper
    {
        public static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            string str = title.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (
                    System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                    != System.Globalization.UnicodeCategory.NonSpacingMark
                )
                {
                    sb.Append(c);
                }
            }
            str = sb.ToString().Normalize(NormalizationForm.FormC);

            str = str.ToLowerInvariant().Replace("đ", "d");

            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            str = Regex.Replace(str, @"\s+", "-").Trim('-');

            return str;
        }
    }
}
