using System.Globalization;
using System.Linq;
using System.Text;

namespace Core.Utils
{
    public static class StringUtils
    {
        public static string ApenasNumeros(this string str, string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        public static string RemoverAcentuacao(this string text)
        {
            return new string(text
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        public static string RemoverAcentuacaoEspacoUpper(this string text)
        {
            return new string(text.Normalize(NormalizationForm.FormD).Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark).ToArray()).Trim().ToUpper();
        }

        public static string RemoverTodosEspacoUpper(this string text)
        {
            return new string(text.Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark).ToArray()).Trim().ToUpper();
        }
    }
}