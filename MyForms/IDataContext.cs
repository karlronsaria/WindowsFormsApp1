using System.Collections.Generic;

namespace MyForms
{
    public interface IDataContext
    {
        IEnumerable<string> GetTagsMatchingSubstring(string substring, bool exact = true);

        IEnumerable<string> GetTagsMatchingPattern(string pattern);

        IEnumerable<string> GetNamesMatchingSubstring(string substring, bool exact = true);

        IEnumerable<string> GetNamesMatchingPattern(string pattern);

        IEnumerable<string> GetNamesMatchingTag(string tag);

        IEnumerable<string> GetTagsMatchingName(string name);

        IEnumerable<string> GetNamesMatchingDate(string date, string format, string pattern);

        IEnumerable<string> GetDatesMatchingName(string name, string format);

        void SetTags(IEnumerable<string> names, IEnumerable<string> tags);

        void SetDates(IEnumerable<string> names, IEnumerable<string> dates, string format);

        void ToJson(string filePath);

        void FromJson(string filePath);
    }
}





