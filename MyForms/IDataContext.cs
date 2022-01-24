using System.Collections.Generic;

namespace MyForms
{
    public interface IDataContext
    {
        IEnumerable<string> GetTagsMatchingSubstring(string substring);

        IEnumerable<string> GetTagsMatchingPattern(string pattern);

        IEnumerable<string> GetNamesMatchingSubstring(string substring);

        IEnumerable<string> GetNamesMatchingPattern(string pattern);

        IEnumerable<string> GetNamesMatchingTag(string tag);

        IEnumerable<string> GetTagsMatchingName(string name);

        IEnumerable<string> GetNamesMatchingDate(string date);

        IEnumerable<string> GetDatesMatchingName(string name);

        void SetTags(IEnumerable<string> names, IEnumerable<string> tags);

        void SetDates(IEnumerable<string> names, IEnumerable<string> dates, string format);
    }
}

