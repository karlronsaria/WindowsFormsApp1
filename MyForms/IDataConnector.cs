using System.Collections.Generic;

namespace MyForms
{
    public interface IDataReader
    {
        IEnumerable<string> GetNamesMatchingSubstring(string substring, bool exact = true);
        IEnumerable<string> GetNamesMatchingPattern(string pattern);
        IEnumerable<string> GetTagsMatchingSubstring(string substring, bool exact = true);
        IEnumerable<string> GetTagsMatchingPattern(string pattern);
        IEnumerable<string> GetNamesMatchingTag(string tag);
        IEnumerable<string> GetTagsMatchingName(string name);
        IEnumerable<string> GetNamesMatchingDate(string date, string format, string pattern);
        IEnumerable<string> GetDatesMatchingName(string name, string format);
    }

    public interface IDataFile
    {
        void ToJson(string filePath);
        void SetFromJson(string filePath);
        void AddFromJson(string filePath);
    }

    public interface IDataWriter
    {
        void AddDates(IEnumerable<string> names, IEnumerable<string> dates, string format);
        void AddTags(IEnumerable<string> names, IEnumerable<string> tags);
        void RemoveDates(IEnumerable<string> names, IEnumerable<string> dates, string format);
        void RemoveTags(IEnumerable<string> names, IEnumerable<string> tags);
    }

    public interface IDataConnector : IDataReader, IDataWriter, IDataFile { }
}
