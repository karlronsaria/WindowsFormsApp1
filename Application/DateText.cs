using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Application
{
    public static class DateText
    {
        public const string
        DATE_FORMAT = "yyyy_MM_dd";

        public static readonly int
        DATE_STRING_LENGTH = DATE_FORMAT.Length;

        public static readonly Dictionary<int, int>
        DATE_TABLE = new Dictionary<int, int>()
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

        public static readonly Regex
        DATE_PATTERN = new Regex(@"^(?<year>\d{4})_(?<month>\d{2})_(?<day>\d{2})$");

        public static bool TryGetDateString(string input, out string dateString)
        {
            if (input.Length > DATE_STRING_LENGTH)
            {
                dateString = input.Substring(0, DATE_STRING_LENGTH);
                return true;
            }

            if (input.Length == DATE_STRING_LENGTH)
            {
                dateString = ToNearestCorrectDate(input);
                return true;
            }

            dateString = "";
            return false;
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

        public static bool GetNextPartialDateString(char keyChar, string input, out string nextPartialDate)
        {
            nextPartialDate = input;

            if (keyChar == '\b' && MatchesDelimiterPostPosition(input))
            {
                nextPartialDate = input.Substring(0, input.Length - 2);
                return true;
            }

            if (Char.IsControl(keyChar))
                return false;

            if (keyChar == '-' || keyChar == '_')
            {
                if (MatchesDelimiterPrePosition(input))
                    nextPartialDate += '_';

                return true;
            }

            if (input.Length == DATE_STRING_LENGTH || !Char.IsDigit(keyChar))
                return true;

            if (MatchesDelimiterPrePosition(input))
            {
                nextPartialDate += $"_{keyChar}";
                return true;
            }

            return false;
        }

        public static bool MatchesDelimiterPrePosition(string str)
        {
            return Regex.IsMatch(str, @"^\d{4}((-|_)\d{2})?$");
        }

        public static bool MatchesDelimiterPostPosition(string str)
        {
            return Regex.IsMatch(str, @"^\d{4}(-|_)\d(\d(-|_)\d)?$");
        }

        public static Match Match(string str)
        {
            return DATE_PATTERN.Match(str);
        }
    }
}







