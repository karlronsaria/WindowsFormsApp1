using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infrastructure
{
    public class PersistentConnector :
        IData<Application.Root>,
        MyForms.IDataReader,
        MyForms.IDataWriter
    {
        private readonly Persistent.EncapsulatedContext
        _context;

        public PersistentConnector(
                string connectionString = Persistent.Context.DEFAULT_CONNECTION_STRING,
                bool reset = false
            )
        {
            _context = new Persistent.EncapsulatedContext(connectionString, reset);
        }

        public Application.Root
        Get()
        {
            return new Application.Root()
            {
                Documents = _context.Documents(),
                Dates = _context.Dates(),
                Tags = _context.Tags(),
                DocumentDates = _context.DocumentDates(),
                DocumentTags = _context.DocumentTags(),
            };
        }

        public void
        Set(Application.Root root)
        {
            _context.Clear();
            Add(root);
        }

        public void
        Add(Application.Root root)
        {
            _context.Add(root.Documents);
            _context.Add(root.Dates);
            _context.Add(root.Tags);
            _context.Add(root.DocumentDates);
            _context.Add(root.DocumentTags);
            _context.Push();
        }

        public IEnumerable<string>
        GetDatesMatchingName(string name, string format)
        {
            return _context.DocumentDates(
                predicate: f => f.Document.Name == name,
                selector: f => f.Date.Value.ToString(format)
            );
        }

        public IEnumerable<string>
        GetNamesMatchingDate(string date, string format, string pattern)
        {
            if (!Regex.IsMatch(date, pattern))
                return new List<string>();

            return _context.DocumentDates(
                predicate: f => f.Date.Value.ToString(format).Contains(date),
                selector: f => f.Document.Name
            );
        }

        public IEnumerable<string>
        GetNamesMatchingPattern(string pattern)
        {
            return _context.Documents(
                predicate: f => Regex.IsMatch(f.Name, pattern),
                selector: f => f.Name
            );
        }

        public IEnumerable<string>
        GetNamesMatchingSubstring(string substring, bool exact = true)
        {
            return _context.Documents(
                // // link: https://stackoverflow.com/questions/57872910/the-linq-expression-could-not-be-translated-and-will-be-evaluated-locally
                // // link: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/supported-and-unsupported-linq-methods-linq-to-entities?redirectedfrom=MSDN
                // // retrieved: 2022_02_05

                // predicate: f => (exact
                //     ? f.Name
                //     : f.Name.ToLowerInvariant()).Contains(substring),

                // predicate: f => f.Name.Contains(substring),

                predicate: f => exact
                    ? f.Name.Contains(substring)
                    : f.Name.ToLower().Contains(substring.ToLower()),

                selector: f => f.Name
            );
        }

        public IEnumerable<string>
        GetNamesMatchingTag(string tag)
        {
            return _context.DocumentTags(
                predicate: f => f.Tag.Name == tag,
                selector: f => f.Document.Name
            );
        }

        public IEnumerable<string>
        GetTagsMatchingName(string name)
        {
            return _context.DocumentTags(
                predicate: f => f.Document.Name == name,
                selector: f => f.Tag.Name
            );
        }

        public IEnumerable<string>
        GetTagsMatchingPattern(string pattern)
        {
            return _context.Tags(
                predicate: f => Regex.IsMatch(f.Name, pattern),
                selector: f => f.Name
            );
        }

        public IEnumerable<string>
        GetTagsMatchingSubstring(string substring, bool exact = true)
        {
            return _context.Tags(
                // // link: https://stackoverflow.com/questions/57872910/the-linq-expression-could-not-be-translated-and-will-be-evaluated-locally
                // // link: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/supported-and-unsupported-linq-methods-linq-to-entities?redirectedfrom=MSDN
                // // retrieved: 2022_02_05

                // predicate: f => (exact
                //     ? f.Name
                //     : f.Name.ToLowerInvariant()).Contains(substring),

                // predicate: f => f.Name.Contains(substring),

                predicate: f => exact
                    ? f.Name.Contains(substring)
                    : f.Name.ToLower().Contains(substring.ToLower()),

                selector: f => f.Name
            );
        }

        public void
        AddDates(IEnumerable<string> names, IEnumerable<string> dates, string format)
        {
            foreach (string date in dates)
                _context.Add(
                    new Persistent.Date()
                    {
                        Value = DateTime.ParseExact(date, format, null),
                    }
                );

            foreach (string name in names)
            {
                _context.Add(
                    new Persistent.Document()
                    {
                        Name = name,
                    }
                );

                var doc = _context.Documents(
                    predicate: e => e.Name == name,
                    selector: e => e
                )
                .SingleOrDefault();

                foreach (string dateStr in dates)
                {
                    var date = _context.Dates(
                        predicate: e => e.Value.ToString(format) == dateStr,
                        selector: e => e
                    )
                    .SingleOrDefault();

                    _context.Add(
                        new Persistent.DocumentDate()
                        {
                            Document = doc,
                            Date = date,
                        }
                    );
                }
            }

            _context.Push();
        }

        public void
        AddTags(IEnumerable<string> names, IEnumerable<string> tags)
        {
            foreach (string tag in tags)
                _context.Add(
                    new Persistent.Tag()
                    {
                        Name = tag,
                    }
                );

            foreach (string name in names)
            {
                _context.Add(
                    new Persistent.Document()
                    {
                        Name = name,
                    }
                );

                var doc = _context.Documents(
                    predicate: e => e.Name == name,
                    selector: e => e
                )
                .SingleOrDefault();

                foreach (string tagName in tags)
                {
                    var tag = _context.Tags(
                        predicate: e => e.Name == tagName,
                        selector: e => e
                    )
                    .SingleOrDefault();

                    _context.Add(
                        new Persistent.DocumentTag()
                        {
                            Document = doc,
                            Tag = tag,
                        }
                    );
                }
            }

            _context.Push();
        }

        public void
        RemoveDates(IEnumerable<string> names, IEnumerable<string> dates, string format)
        {
            _context.Remove(
                _context.DocumentDates(
                    predicate: e =>
                        names.Contains(e.Document.Name) &&
                        dates.Contains(e.Date.Value.ToString(format)),
                    selector: e => e
                )
            );

            _context.Push();
        }

        public void
        RemoveTags(IEnumerable<string> names, IEnumerable<string> tags)
        {
            _context.Remove(
                _context.DocumentTags(
                    predicate: e =>
                        names.Contains(e.Document.Name) &&
                        tags.Contains(e.Tag.Name),
                    selector: e => e
                )
            );

            _context.Push();
        }
    }
}

