using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Persistent
{
    public class EncapsulatedContext<ContextT>
        where ContextT : Persistent.Context, new()
    {
        public static class Clones
        {
            public static BaseT
            ToBase<BaseT, DerivT>(DerivT entity)
                where BaseT : class, new()
            {
                var baseEntity = new BaseT();

                foreach (var prop in typeof(BaseT).GetProperties())
                    prop.SetValue(baseEntity, prop.GetValue(entity));

                return baseEntity;
            }

            public static DerivT
            ToDerived<BaseT, DerivT>(BaseT entity)
                where DerivT : class, new()
            {
                var derivedEntity = new DerivT();

                foreach (var prop in typeof(BaseT).GetProperties())
                    prop.SetValue(derivedEntity, prop.GetValue(entity));

                return derivedEntity;
            }

            public static Application.MyEnumerable<BaseT>
            ToEnumerable<BaseT, DerivT>(IEnumerable<DerivT> collection)
                where BaseT : class, new()
            {
                var baseCollection = new Application.MyEnumerable<BaseT>();

                foreach (var entity in collection)
                    baseCollection.Add(Clones.ToBase<BaseT, DerivT>(entity));

                return baseCollection;
            }
        }

        private readonly ContextT
        _context;

        public EncapsulatedContext(ContextT context)
        {
            _context = context;
        }

        public Application.Root
        Root()
        {
            return new Application.Root()
            {
                Documents =
                    Clones.ToEnumerable<Application.Document, Persistent.Document>(_context.Documents),
                Dates = 
                    Clones.ToEnumerable<Application.Date, Persistent.Date>(_context.Dates),
                Tags = 
                    Clones.ToEnumerable<Application.Tag, Persistent.Tag>(_context.Tags),
                DocumentDates = 
                    Clones.ToEnumerable<Application.DocumentDate, Persistent.DocumentDate>(_context.DocumentDates),
                DocumentTags = 
                    Clones.ToEnumerable<Application.DocumentTag, Persistent.DocumentTag>(_context.DocumentTags),
            };
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

        public void
        Add(Persistent.Document document)
        {
            _context.Documents.Add(document);
        }

        public void
        Add(Persistent.Date date)
        {
            _context.Dates.Add(date);
        }

        public void
        Add(Persistent.Tag tag)
        {
            _context.Tags.Add(tag);
        }

        public void
        Add(Persistent.DocumentDate documentDate)
        {
            _context.DocumentDates.Add(documentDate);
        }

        public void
        Add(Persistent.DocumentTag documentTag)
        {
            _context.DocumentTags.Add(documentTag);
        }

        public void
        Add(Application.Document document)
        {
            _context.Documents.Add(Clones.ToDerived<Application.Document, Persistent.Document>(document));
        }

        public void
        Add(Application.Date date)
        {
            _context.Dates.Add(Clones.ToDerived<Application.Date, Persistent.Date>(date));
        }

        public void
        Add(Application.Tag tag)
        {
            _context.Tags.Add(Clones.ToDerived<Application.Tag, Persistent.Tag>(tag));
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
                _context.Documents.Add(document);
        }

        public void
        Add(IEnumerable<Persistent.Date> dates)
        {
            foreach (var date in dates)
                _context.Dates.Add(date);
        }

        public void
        Add(IEnumerable<Persistent.Tag> tags)
        {
            foreach (var tag in tags)
                _context.Tags.Add(tag);
        }

        public void
        Add(IEnumerable<Persistent.DocumentDate> documentDates)
        {
            foreach (var e in documentDates)
                _context.DocumentDates.Add(e);
        }

        public void
        Add(IEnumerable<Persistent.DocumentTag> documentTags)
        {
            foreach (var e in documentTags)
                _context.DocumentTags.Add(e);
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
