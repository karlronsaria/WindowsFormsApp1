using System.Collections.Generic;

namespace Infrastructure
{
    public interface IData<RootT>
        where RootT : Application.Root
    {
        RootT Get();
        void Set(RootT root);
        void Add(RootT root);
    }

    public interface IJson<RootT>
        where RootT : Application.Root
    {
        RootT Get(string filePath);
        void Set(RootT root, string filePath);
    }

    public class FrameworkConnector<DataT, JsonT, RootT> : MyForms.IDataConnector
        where RootT : Application.Root
        where DataT : IData<RootT>, MyForms.IDataReader, MyForms.IDataWriter
        where JsonT : IJson<RootT>
    {
        private readonly DataT _data;
        private readonly JsonT _json;

        public FrameworkConnector(DataT dataConnector, JsonT jsonConnector)
        {
            _data = dataConnector;
            _json = jsonConnector;
        }

        public void SetFromJson(string filePath)
        {
            _data.Set(_json.Get(filePath));
        }

        public void AddFromJson(string filePath)
        {
            _data.Add(_json.Get(filePath));
        }

        public void ToJson(string filePath)
        {
            _json.Set(_data.Get(), filePath);
        }

        public IEnumerable<string> GetNames()
        {
            return _data.GetNames();
        }

        public IEnumerable<string> GetTags()
        {
            return _data.GetTags();
        }

        public IEnumerable<string> GetDates()
        {
            return _data.GetDates();
        }

        public IEnumerable<string> GetDatesMatchingName(string name, string format)
        {
            return _data.GetDatesMatchingName(name, format);
        }

        public IEnumerable<string> GetNamesMatchingDate(string date, string format, string pattern)
        {
            return _data.GetNamesMatchingDate(date, format, pattern);
        }

        public IEnumerable<string> GetNamesMatchingPattern(string pattern)
        {
            return _data.GetNamesMatchingPattern(pattern);
        }

        public IEnumerable<string> GetNamesMatchingSubstring(string substring, bool exact = true)
        {
            return _data.GetNamesMatchingSubstring(substring, exact);
        }

        public IEnumerable<string> GetNamesMatchingTag(string tag)
        {
            return _data.GetNamesMatchingTag(tag);
        }

        public IEnumerable<string> GetTagsMatchingName(string name)
        {
            return _data.GetTagsMatchingName(name);
        }

        public IEnumerable<string> GetTagsMatchingPattern(string pattern)
        {
            return _data.GetTagsMatchingPattern(pattern);
        }

        public IEnumerable<string> GetTagsMatchingSubstring(string substring, bool exact = true)
        {
            return _data.GetTagsMatchingSubstring(substring, exact);
        }

        public void AddDates(IEnumerable<string> names, IEnumerable<string> dates, string format)
        {
            _data.AddDates(names, dates, format);
        }

        public void AddTags(IEnumerable<string> names, IEnumerable<string> tags)
        {
            _data.AddTags(names, tags);
        }

        public void RemoveDates(IEnumerable<string> names, IEnumerable<string> dates, string format)
        {
            _data.RemoveDates(names, dates, format);
        }

        public void RemoveTags(IEnumerable<string> names, IEnumerable<string> tags)
        {
            _data.RemoveTags(names, tags);
        }
    }
}
