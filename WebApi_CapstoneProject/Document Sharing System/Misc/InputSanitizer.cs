using System.Text.RegularExpressions;

namespace DSS.Misc
{
    public static class InputSanitizer
    {
        public static string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim();
            input = Regex.Replace(input, "<.*?>", string.Empty); // Strip HTML tags
            input = input.Length > 200 ? input.Substring(0, 200) : input;

            return input;
        }
    }
}