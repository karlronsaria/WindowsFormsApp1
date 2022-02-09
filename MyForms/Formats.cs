using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyForms
{
    public static class Formats
    {
        public const string
        DELIMITER = "_";

        public static readonly string
        DATE_FORMAT = $"yyyy{DELIMITER}MM{DELIMITER}dd";

        public const string
        TIME_FORMAT = "HHmmss";

        public static readonly string
        DATETIME_FORMAT = $"{DATE_FORMAT}{DELIMITER}{TIME_FORMAT}";

        public static readonly int
        DATE_STRING_LENGTH = DATE_FORMAT.Length;

        public static readonly int
        TIME_STRING_LENGTH = TIME_FORMAT.Length;

        public static readonly int
        DATETIME_STRING_LENGTH = DATETIME_FORMAT.Length;

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

        public static readonly string
        DATE_PATTERN_NONCAPTURE =
               @"\d{4}" + DELIMITER + @"?|" + DELIMITER + @"?\d{2}" + DELIMITER + @"?"
            + @"|\d{4}" + DELIMITER + @"\d{2}" + DELIMITER + @"\d{2}";
 
        public static readonly string
        DATE_PATTERN =
              @"(?<year>\d{4})" + DELIMITER
            + @"(?<month>\d{2})" + DELIMITER
            + @"(?<day>\d{2})";

        public static readonly string
        TIME_PATTERN = @"(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})";

        public static readonly string
        DATETIME_PATTERN = $@"{DATE_PATTERN}{DELIMITER}{TIME_PATTERN}";

        public static readonly Regex
        EXACT_DATE_FSM = new Regex($@"^{DATE_PATTERN}$");

        public static readonly Regex
        EXACT_TIME_FSM = new Regex($@"^{TIME_PATTERN}$");

        public static readonly Regex
        EXACT_DATETIME_FSM = new Regex($@"^{DATETIME_PATTERN}$");

        // TODO: unit test
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

        // TODO: unit test
        public static bool TryGetDateTimeString(string input, out string outString)
        {
            var capture = Regex.Match(input, DATETIME_PATTERN);

            if (!capture.Success)
            {
                outString = input;
                return false;
            }

            input = capture.Value;

            if (input.Length > DATETIME_STRING_LENGTH)
            {
                outString = input.Substring(0, DATETIME_STRING_LENGTH);
                return true;
            }

            if (input.Length == DATETIME_STRING_LENGTH)
            {
                outString = ToNearestCorrectDateTime(input);
                return true;
            }

            return TryGetDateString(input, out outString);
        }

        // TODO: unit test
        public static bool TryReplaceDateTimeString(string input, out string outString)
        {
            bool success = TryGetDateTimeString(input, out string replacement);

            if (!success)
            {
                outString = input;
                return false;
            }

            // outString = Regex.Replace(input, DATETIME_PATTERN, replacement); 

            outString = Regex.Replace(
                input: input,
                pattern: DATETIME_PATTERN,
                replacement: DateTime.Now.ToString(DATETIME_FORMAT)
            ); 

            return true;
        }

        // TODO: unit test
        public static string ToNearestCorrectDate(string dateString)
        {
            return ToNearestCorrectDate(EXACT_DATE_FSM.Match(dateString));
        }

        // TODO: unit test
        public static string ToNearestCorrectTime(string timeString)
        {
            return ToNearestCorrectTime(EXACT_TIME_FSM.Match(timeString));
        }

        // TODO: unit test
        public static string ToNearestCorrectDateTime(string inputStr)
        {
            Match capture = EXACT_DATETIME_FSM.Match(inputStr);
            string date = ToNearestCorrectDate(capture);
            string time = ToNearestCorrectTime(capture);
            return $"{date}{DELIMITER}{time}";
        }

        // TODO: unit test
        public static string ToNearestCorrectDate(Match capture)
        {
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

            return $"{year:D4}{DELIMITER}{month:D2}{DELIMITER}{day:D2}";
        }

        // TODO: unit test
        public static string ToNearestCorrectTime(Match capture)
        {
            if (!capture.Success)
                return DateTime.Now.ToString(TIME_FORMAT);

            int.TryParse(capture.Groups["hour"].Value, out int hour);
            int.TryParse(capture.Groups["minute"].Value, out int minute);
            int.TryParse(capture.Groups["second"].Value, out int second);

            if (hour < 0)
                hour = 0;
            else if (hour > 23)
                hour = 23;

            if (minute < 0)
                minute = 0;
            else if (minute > 59)
                minute = 59;

            if (second < 0)
                second = 0;
            else if (second > 59)
                second = 59;

            return $"{hour:D2}{minute:D2}{second:D2}";
        }

        // TODO: unit test
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

            bool isValid =
                   input.Length == DATE_STRING_LENGTH
                || input.Length == DATETIME_STRING_LENGTH
                || !Char.IsDigit(keyChar);

            if (isValid)
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
            return Regex.IsMatch(str, @"^\d{4}((-|_)\d{2}){0,2}$");
        }

        public static bool MatchesDelimiterPostPosition(string str)
        {
            return Regex.IsMatch(str, @"^\d{4}(-|_)\d(\d(-|_)\d){0,2}$");
        }

        public static Match MatchDate(string str)
        {
            return EXACT_DATE_FSM.Match(str);
        }

        public static Match MatchTime(string str)
        {
            return EXACT_TIME_FSM.Match(str);
        }

        public static Match MatchDateTime(string str)
        {
            return EXACT_DATETIME_FSM.Match(str);
        }
        public static bool
        GetContainerAndLeaf(string path, out string container, out string leaf)
        {
            var nodes = System.Linq.Enumerable.ToList(
                path.Split(System.IO.Path.DirectorySeparatorChar)
            );

            if (!nodes.Any())
            {
                container = "";
                leaf = path;
                return false;
            }

            leaf = nodes[nodes.Count - 1];
            nodes.RemoveAt(nodes.Count - 1);
            container = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), nodes);
            return true;
        }

        public static bool
        GetNameAndExtension(string filename, out string name, out string extension)
        {
            var split = System.Linq.Enumerable.ToList(
                filename.Split('.')
            );

            if (!split.Any())
            {
                name = filename;
                extension = "";
                return false;
            }

            extension = split[split.Count - 1];
            split.RemoveAt(split.Count - 1);
            name = string.Join(".", split);
            return true;
        }

        public static void
        NewDatedFilePath(string filePath, out string newName, out string newPath)
        {
            bool hasContainer = GetContainerAndLeaf(
                path: filePath,
                out string container,
                out string leaf
            );

            bool hasExtension = GetNameAndExtension(
                filename: leaf,
                out string name,
                out string extension
            );

            bool hasDateTime = TryReplaceDateTimeString(
                input: name,
                out newName
            );

            if (!hasDateTime)
                newName += DELIMITER + DateTime.Now.ToString(DATETIME_FORMAT);

            if (hasExtension)
                newName = $"{newName}.{extension}";

            newPath = hasContainer
                ? System.IO.Path.Combine(container, newName)
                : newName;
        }
    }
}







