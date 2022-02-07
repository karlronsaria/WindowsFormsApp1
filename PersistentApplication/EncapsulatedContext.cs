using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Persistent
{
    public class EncapsulatedContext
    {
        private readonly Persistent.Context
        _context;

        public EncapsulatedContext(Persistent.Context context)
        {
            _context = context;
        }

        public EncapsulatedContext(
                string connectionString = Persistent.Context.DEFAULT_CONNECTION_STRING,
                bool reset = false
            )
        {
            _context = new Persistent.Context(connectionString, reset);
        }

        public Application.MyEnumerable<T>
        Documents<T>() where T : Application.Document
        {
            var collection = new Application.MyEnumerable<T>();

            foreach (var document in _context.Documents)
                collection.Add(document as T);

            return collection;
        }

        public Application.MyEnumerable<T>
        Dates<T>() where T : Application.Date
        {
            var collection = new Application.MyEnumerable<T>();

            foreach (var date in _context.Dates)
                collection.Add(date as T);

            return collection;
        }

        public Application.MyEnumerable<T>
        Tags<T>() where T : Application.Tag
        {
            var collection = new Application.MyEnumerable<T>();

            foreach (var tag in _context.Tags)
                collection.Add(tag as T);

            return collection;
        }

        public Application.MyEnumerable<T>
        DocumentDates<T>() where T : Application.DocumentDate
        {
            var collection = new Application.MyEnumerable<T>();

            foreach (var e in _context.DocumentDates)
                collection.Add(e as T);

            return collection;
        }

        public Application.MyEnumerable<T>
        DocumentTags<T>() where T : Application.DocumentTag
        {
            var collection = new Application.MyEnumerable<T>();

            foreach (var e in _context.DocumentTags)
                collection.Add(e as T);

            return collection;
        }

        public IEnumerable<ColumnT>
        Documents<ColumnT>(
                Expression<System.Func<Persistent.Document, bool>> predicate,
                Expression<System.Func<Persistent.Document, ColumnT>> selector
            )
            where ColumnT : class
        {
            return _context.Documents.Where(predicate).Select(selector);
        }

        public IEnumerable<ColumnT>
        Dates<ColumnT>(
                Expression<System.Func<Persistent.Date, bool>> predicate,
                Expression<System.Func<Persistent.Date, ColumnT>> selector
            )
            where ColumnT : class
        {
            return _context.Dates.Where(predicate).Select(selector);
        }

        public IEnumerable<ColumnT>
        Tags<ColumnT>(
                Expression<System.Func<Persistent.Tag, bool>> predicate,
                Expression<System.Func<Persistent.Tag, ColumnT>> selector
            )
            where ColumnT : class
        {
            return _context.Tags.Where(predicate).Select(selector);
        }

        public IEnumerable<ColumnT>
        DocumentDates<ColumnT>(
                Expression<System.Func<Persistent.DocumentDate, bool>> predicate,
                Expression<System.Func<Persistent.DocumentDate, ColumnT>> selector
            )
            where ColumnT : class
        {
            return _context.DocumentDates.Where(predicate).Select(selector);
        }

        public IEnumerable<ColumnT>
        DocumentTags<ColumnT>(
                Expression<System.Func<Persistent.DocumentTag, bool>> predicate,
                Expression<System.Func<Persistent.DocumentTag, ColumnT>> selector
            )
            where ColumnT : class
        {
            return _context.DocumentTags.Where(predicate).Select(selector);
        }

        public void
        Clear()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public void
        Push()
        {
            _context.SaveChanges();
        }

        /*
        // TODO
        public void
        Add<P, A>(A value)
            where P : class, new()
            where A : class
        {
            foreach (var prop in _context.GetType().GetProperties())
                if (prop is Microsoft.EntityFrameworkCore.DbSet<P>
                    && prop.Name == typeof(A).Name)
                    (prop.GetValue(_context) as Microsoft.EntityFrameworkCore.DbSet<P>)
                        .AddIfNotExists(value as P);
        }

        public void
        Add<P, A>(IEnumerable<A> values)
            where P : class, new()
            where A : class
        {
            foreach (var value in values)
                Add<P, A>(value)
        }

        public void
        Remove<P, A>(A value)
            where P : class, new()
            where A : class
        {
            foreach (var prop in _context.GetType().GetProperties())
                if (prop is Microsoft.EntityFrameworkCore.DbSet<P>
                    && prop.Name == typeof(A).Name)
                    (prop.GetValue(_context) as Microsoft.EntityFrameworkCore.DbSet<P>)
                        .Remove(value as P);
        }

        public void
        Remove<P, A>(IEnumerable<A> values)
            where P : class, new()
            where A : class
        {
            foreach (var value in values)
                Remove<P, A>(value)
        }
        */

        public void
        Add(Persistent.Document document)
        {
            _context.Documents.AddIfNotExists(document);
        }

        public void
        Add(Persistent.Date date)
        {
            _context.Dates.AddIfNotExists(date);
        }

        public void
        Add(Persistent.Tag tag)
        {
            _context.Tags.AddIfNotExists(tag);
        }

        public void
        Add(Persistent.DocumentDate documentDate)
        {
            _context.DocumentDates.AddIfNotExists(documentDate);
        }

        public void
        Add(Persistent.DocumentTag documentTag)
        {
            _context.DocumentTags.AddIfNotExists(documentTag);
        }

        public void
        Add(Application.Document document)
        {
            _context.Documents.AddIfNotExists(
                new Persistent.Document()
                {
                    Name = document.Name,
                    Description = document.Description,
                }
            );
        }

        public void
        Add(Application.Date date)
        {
            _context.Dates.AddIfNotExists(
                new Persistent.Date()
                {
                    Value = date.Value,
                }
            );
        }

        public void
        Add(Application.Tag tag)
        {
            _context.Tags.AddIfNotExists(
                new Persistent.Tag()
                {
                    Name = tag.Name,
                }
            );
        }

        public bool
        Add(Application.DocumentDate documentDate)
        {
            Document myDocument = _context
                .Documents
                .SingleOrDefault(e => e.Id == documentDate.DocumentId);

            Date myDate = _context
                .Dates
                .SingleOrDefault(e => e.Id == documentDate.DateId);

            bool success = myDocument != null && myDate != null;

            if (!success)
                return false;

            _context.DocumentDates.Add(
                new Persistent.DocumentDate()
                {
                    DocumentId = documentDate.DocumentId,
                    Document = myDocument,
                    DateId = documentDate.DateId,
                    Date = myDate,
                }
            );

            return success;
        }

        public bool
        Add(Application.DocumentTag documentTag)
        {
            Document myDocument = _context
                .Documents
                .SingleOrDefault(e => e.Id == documentTag.DocumentId);

            Tag myTag = _context
                .Tags
                .SingleOrDefault(e => e.Id == documentTag.TagId);

            bool success = myDocument != null && myTag != null;

            if (!success)
                return false;

            _context.DocumentTags.Add(
                new Persistent.DocumentTag()
                {
                    DocumentId = documentTag.DocumentId,
                    Document = myDocument,
                    TagId = documentTag.TagId,
                    Tag = myTag,
                }
            );

            return success;
        }

        public void
        Add(IEnumerable<Persistent.Document> documents)
        {
            foreach (var document in documents)
                _context.Documents.AddIfNotExists(document);
        }

        public void
        Add(IEnumerable<Persistent.Date> dates)
        {
            foreach (var date in dates)
                _context.Dates.AddIfNotExists(date);
        }

        public void
        Add(IEnumerable<Persistent.Tag> tags)
        {
            foreach (var tag in tags)
                _context.Tags.AddIfNotExists(tag);
        }

        public void
        Add(IEnumerable<Persistent.DocumentDate> documentDates)
        {
            foreach (var e in documentDates)
                _context.DocumentDates.AddIfNotExists(e);
        }

        public void
        Add(IEnumerable<Persistent.DocumentTag> documentTags)
        {
            foreach (var e in documentTags)
                _context.DocumentTags.AddIfNotExists(e);
        }

        public void
        Add(IEnumerable<Application.Document> documents)
        {
            foreach (var document in documents)
                Add(document);
        }

        public void
        Add(IEnumerable<Application.Date> dates)
        {
            foreach (var date in dates)
                Add(date);
        }

        public void
        Add(IEnumerable<Application.Tag> tags)
        {
            foreach (var tag in tags)
                Add(tag);
        }

        public void
        Add(IEnumerable<Application.DocumentDate> documentDates)
        {
            foreach (var e in documentDates)
                Add(e);
        }

        public void
        Add(IEnumerable<Application.DocumentTag> documentTags)
        {
            foreach (var e in documentTags)
                Add(e);
        }

        public void
        Remove(Persistent.Document document)
        {
            _context.Documents.Remove(document);
        }

        public void
        Remove(Persistent.Date date)
        {
            _context.Dates.Remove(date);
        }

        public void
        Remove(Persistent.Tag tag)
        {
            _context.Tags.Remove(tag);
        }

        public void
        Remove(Persistent.DocumentDate documentDate)
        {
            _context.DocumentDates.Remove(documentDate);
        }

        public void
        Remove(Persistent.DocumentTag documentTag)
        {
            _context.DocumentTags.Remove(documentTag);
        }

        public void
        Remove(IEnumerable<Persistent.Document> documents)
        {
            foreach (var document in documents)
                _context.Documents.Remove(document);
        }

        public void
        Remove(IEnumerable<Persistent.Date> dates)
        {
            foreach (var date in dates)
                _context.Dates.Remove(date);
        }

        public void
        Remove(IEnumerable<Persistent.Tag> tags)
        {
            foreach (var tag in tags)
                _context.Tags.Remove(tag);
        }

        public void
        Remove(IEnumerable<Persistent.DocumentDate> documentDates)
        {
            foreach (var e in documentDates)
                _context.DocumentDates.Remove(e);
        }

        public void
        Remove(IEnumerable<Persistent.DocumentTag> documentTags)
        {
            foreach (var e in documentTags)
                _context.DocumentTags.Remove(e);
        }
    }
}
