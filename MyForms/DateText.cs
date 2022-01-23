using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyForms
{
    public static class DateText
    {
        public const string DATE_FORMAT = "yyyy_MM_dd";
        public static readonly int DATE_STRING_LENGTH = DATE_FORMAT.Length;

        public static readonly Dictionary<int, int> DATE_TABLE = new Dictionary<int, int>()
        {
            {1, 30},
            {2, 28},
            {3, 31},
            {4, 30},
            {5, 31},
            {6, 30},
            {7, 31},
            {8, 31},
            {9, 30},
            {10, 31},
            {11, 30},
            {12, 31},
        };

        public static readonly Regex DATE_PATTERN = new Regex(@"^(?<year>\d{4})_(?<month>\d{2})_(?<day>\d{2})$");

        public static T New<T>()
            where T : TextBox, new()
        {
            return ToDateTextBox(new T());
        }

        public static T ToDateTextBox<T>(T textBox)
            where T : TextBox, new()
        {
            textBox.KeyPress += (sender, e) => HandleDateKeyPress(textBox as TextBox, e);
            textBox.TextChanged += (sender, e) => HandleDateTextChange(textBox as TextBox, e);
            return textBox;
        }

        public static void HandleDateTextChange(TextBox textBox, EventArgs e)
        {
            if (textBox.TextLength > DATE_STRING_LENGTH)
            {
                textBox.Text = textBox.Text.Substring(0, DATE_STRING_LENGTH);
                textBox.Select(textBox.TextLength, 0);
            }

            if (textBox.TextLength == DATE_STRING_LENGTH)
            {
                textBox.Text = ToNearestCorrectDate(textBox.Text);
                textBox.Select(textBox.TextLength, 0);
            }
        }

        public static string ToNearestCorrectDate(string dateString)
        {
            Match capture = DATE_PATTERN.Match(dateString);

            if (!capture.Success)
                return DateTime.Now.ToString(DATE_FORMAT);

            int.TryParse(capture.Groups["year"].Value, out int year);
            int.TryParse(capture.Groups["month"].Value, out int month);
            int.TryParse(capture.Groups["day"].Value, out int day);

            if (month == 0)
                month = 1;
            else if (month > 12)
                month = 12;

            if (day == 0)
                day = 1;
            else if (day > DATE_TABLE[month])
                day = ((month == 2) && (DateTime.IsLeapYear(year)))
                    ? 29 : DATE_TABLE[month];

            return $"{year:D4}_{month:D2}_{day:D2}";
        }

        public static bool HandleDateKeyPress(TextBox textBox, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b' && MatchesDelimiterPostPosition(textBox.Text))
            {
                textBox.Text = textBox.Text.Substring(0, textBox.TextLength - 2);
                textBox.Select(textBox.TextLength, 0);
                return e.Handled = true;
            }

            if (Char.IsControl(e.KeyChar))
                return e.Handled;

            if (e.KeyChar == '-' || e.KeyChar == '_')
            {
                if (MatchesDelimiterPrePosition(textBox.Text))
                    textBox.Text += '_';

                textBox.Select(textBox.TextLength, 0);
                return e.Handled = true;
            }

            if (textBox.Text.Length == DATE_STRING_LENGTH || !Char.IsDigit(e.KeyChar))
            {
                textBox.Select(textBox.TextLength, 0);
                return e.Handled = true;
            }

            if (MatchesDelimiterPrePosition(textBox.Text))
            {
                textBox.Text += $"_{e.KeyChar}";
                textBox.Select(textBox.TextLength, 0);
                return e.Handled = true;
            }

            return e.Handled;
        }

        public static bool MatchesDelimiterPrePosition(string str)
        {
            return Regex.IsMatch(str, @"^\d{4}((-|_)\d{2})?$");
        }

        public static bool MatchesDelimiterPostPosition(string str)
        {
            return Regex.IsMatch(str, @"^\d{4}(-|_)\d(\d(-|_)\d)?$");
        }
    }
}







