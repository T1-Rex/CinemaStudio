using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CinemaStudio
{
    internal static class FieldChecks
    {
        public static void OnlyNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string text = textBox.Text;

            string clearText = Regex.Replace(text, @"\D", "");

            if (text != clearText)
            {
                textBox.TextChanged -= OnlyNumber_TextChanged;

                int currentPositionCaret = textBox.CaretIndex;

                textBox.Text = clearText;
                textBox.CaretIndex = Math.Min(currentPositionCaret, clearText.Length);

                textBox.TextChanged += OnlyNumber_TextChanged;
            }
        }

        public static void OnlyRealNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string text = textBox.Text;

            string clearText = Regex.Replace(text, @"[^0-9,]", "");

            // Только 1 запятая
            if (clearText.Count(x => x == ',') > 1)
            {
                int first = clearText.IndexOf(',');
                clearText = clearText.Substring(0, first + 1) + clearText.Substring(first + 1).Replace(",", "");
            }

            if (clearText.StartsWith(","))
            {
                clearText = "0" + clearText;
            }

            if (text != clearText)
            {
                textBox.TextChanged -= OnlyRealNumber_TextChanged;

                int currentPositionCaret = textBox.CaretIndex;

                if (text.StartsWith(",") && clearText.StartsWith("0,"))
                {
                    currentPositionCaret++;
                }

                textBox.Text = clearText;
                textBox.CaretIndex = Math.Min(currentPositionCaret, clearText.Length);

                textBox.TextChanged += OnlyRealNumber_TextChanged;
            }
        }

        public static void LettersWrite_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string text = textBox.Text;

            string clearText = Regex.Replace(text, @"[^a-zA-Zа-яА-ЯёЁ]", "");

            if (text != clearText)
            {
                textBox.TextChanged -= LettersWrite_TextChanged;

                textBox.Text = clearText;
                textBox.CaretIndex = clearText.Length;

                textBox.TextChanged += LettersWrite_TextChanged;
            }
        }

        public static void TbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string text = textBox.Text;

            string clearText = Regex.Replace(text, @"[^a-zA-Z0-9*!_.-]", "");

            if (text != clearText)
            {
                textBox.TextChanged -= TbLogin_TextChanged;

                textBox.Text = clearText;
                textBox.CaretIndex = clearText.Length;

                textBox.TextChanged += TbLogin_TextChanged;
            }
        }

        public static void TbPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string num = Regex.Replace(textBox.Text, @"\D", "");

            string phoneFormatted = "";

            if (num.Length > 0)
            {
                string firstNum = num[0] == '8' ? "7" : num[0].ToString();

                string textNum = num.Length > 1 ? num.Substring(1) : "";

                phoneFormatted = "+" + firstNum + textNum;
            }

            if (textBox.Text != phoneFormatted)
            {
                textBox.TextChanged -= TbPhone_TextChanged;

                textBox.Text = phoneFormatted;
                textBox.CaretIndex = phoneFormatted.Length;

                textBox.TextChanged += TbPhone_TextChanged;
            }
        }

        public static void TbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string text = textBox.Text;

            string clearText = Regex.Replace(text, @"[^a-zA-Z0-9@._-]", "");

            clearText = clearText.ToLower();

            if (text != clearText)
            {
                textBox.TextChanged -= TbEmail_TextChanged;

                textBox.Text = clearText;
                textBox.CaretIndex = clearText.Length;

                textBox.TextChanged += TbEmail_TextChanged;
            }
        }
    }
}
