using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaStudio
{
    internal static class PasswordChecked
    {
        // Проверка сложности пароля для ProgressBar.
        public static int CheckedPassword(string text)
        {
            int count = 0;
            // Длина пароля больше 7.
            if (text.Length >= 8)
                count++;
            // В пароле есть цифра.
            if (text.Any(char.IsDigit))
                count++;
            // В пароле есть заглавные и строчные буквы.
            if (text.Any(char.IsLower) && text.Any(char.IsUpper))
                count++;
            // В пароле есть спец символ.
            if (text.Any(char.IsSymbol)
                || text.Any(char.IsPunctuation))
                count++;

            return count;
        }

        // Проверка сложности пароля для TextBlock.
        public static string TextCheckedPassword(int number)
        {
            switch (number)
            {
                case 0:
                    return "Плохой пароль";
                case 1:
                    return "Слабый пароль";
                case 2:
                    return "Средний пароль";
                case 3:
                    return "Хороший пароль";
                case 4:
                    return "Надежный пароль";
            }
            return "";
        }
    }
}
