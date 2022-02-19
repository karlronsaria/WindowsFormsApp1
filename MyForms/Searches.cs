using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyForms
{
    public static class Searches
    {
        public enum Mode : int
        {
            All,
            Exact,
            Regex,
            Document,
            Tag,
            Date,
        }

        public static bool
        GetModes(string inputText, out string newText, out HashSet<Mode> modes)
        {
            Match modesCapture;

            bool hasModes;
            bool all = false;
            bool exact = false;
            bool document = false;
            bool tag = false;
            bool date = false;

            modesCapture = Regex.Match(
                input: inputText,
                pattern: @"^\s*(?<modes>\w+(\s*,\s*\w+)*)\s*\:\s*(?<searchstr>.*)$"
            );

            hasModes = modesCapture.Success;
            modes = new HashSet<Mode>();

            if (hasModes)
            {
                var capturedModes =
                    from mode in modesCapture.Groups["modes"].Value.Split(',')
                    select mode.Trim().ToLowerInvariant();

                inputText = modesCapture.Groups["searchstr"].Value.Trim();

                foreach (var mode in capturedModes)
                {
                    switch (mode)
                    {
                        case "all":
                            all = true;
                            modes.Add(Mode.All);
                            break;
                        case "r":
                        case "re":
                        case "regex":
                            modes.Add(Mode.Regex);
                            break;
                        case "e":
                        case "ex":
                        case "exact":
                            exact = true;
                            modes.Add(Mode.Exact);
                            break;
                        case "doc":
                        case "document":
                        case "documents":
                            document = true;
                            modes.Add(Mode.Document);
                            break;
                        case "tag":
                        case "tags":
                            tag = true;
                            modes.Add(Mode.Tag);
                            break;
                        case "date":
                        case "dates":
                            date = true;
                            break;
                    }
                }
            }

            if (date)
            {
                modes.Clear();

                if (all)
                    modes.Add(Mode.All);
                
                modes.Add(Mode.Date);
                newText = inputText;
                return true;
            }

            if (!document && !tag)
            {
                modes.Add(Mode.Document);
                modes.Add(Mode.Tag);
            }

            if (all)
            {
                modes.Remove(Mode.Exact);
                modes.Remove(Mode.Regex);
                newText = inputText;
                return true;
            }

            if (!exact)
            {
                Match exactCapture = Regex.Match(
                    input: inputText,
                    pattern: "(?<=^\\s*\")(?<exacttext>.*)(?=\"\\s*$)"
                );

                if (exactCapture.Success)
                {
                    modes.Add(Mode.Exact);
                    inputText = exactCapture.Groups["exacttext"].Value;
                    hasModes = true;
                }
            }

            newText = inputText;
            return hasModes;
        }

        public static string
        GetStatusMessage(HashSet<Mode> modes, string searchStr)
        {
            string modeStatus = String.Join(
                separator: " ",
                values: (
                    from mode in modes
                    select mode == Mode.All
                        ? $"<all, ignoring query string>"
                        : $"<{mode.ToString().ToLower()}>"
                )
            );

            string queryStatus = modes.Contains(Mode.All)
                ? ""
                : $"Query: {searchStr}";

            modeStatus = $"Modes: {modeStatus}";

            if (!string.IsNullOrEmpty(queryStatus) && !string.IsNullOrEmpty(modeStatus))
                return $"{modeStatus}   {queryStatus}";
            else if (!string.IsNullOrEmpty(modeStatus))
                return $"{modeStatus}";
            else if (!string.IsNullOrEmpty(queryStatus))
                return $"{queryStatus}";

            return "";
        }
    }
}
