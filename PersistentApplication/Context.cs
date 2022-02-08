using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

// link: https://stackoverflow.com/questions/31162576/entity-framework-add-if-not-exist-without-update
// retrieved: 2022_01_29
public static class DbSetExtensions
{
    public static Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T>
    AddIfNotExists<T>(
            this DbSet<T> dbSet,
            T entity,
            Expression<Func<T, bool>> predicate = null
        )
        where T : class, new()
    {
        var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
        return !exists ? dbSet.Add(entity) : null;
    }
}

namespace Persistent
{
    // link: https://www.learnentityframeworkcore.com/configuration/many-to-many-relationship-configuration
    // retrieved: 2022_01_28

    public class Document : Application.Document
    {
        public virtual ICollection<DocumentTag> DocumentTags { get; set; }
            = new Application.MyEnumerable<DocumentTag>();

        public virtual ICollection<DocumentDate> DocumentDates { get; set; }
            = new Application.MyEnumerable<DocumentDate>();
    }

    public class Date : Application.Date
    {
        public const string DATE_FORMAT = "yyyy_MM_dd";

        public virtual ICollection<DocumentDate> DocumentDates { get; set; }
            = new Application.MyEnumerable<DocumentDate>();

        public string DateString
        {
            get => Value.ToString(DATE_FORMAT);
            set => Value = DateTime.ParseExact(value, DATE_FORMAT, null);
        }
    }

    public class Tag : Application.Tag
    {
        public virtual ICollection<DocumentTag> DocumentTags { get; set; }
            = new Application.MyEnumerable<DocumentTag>();
    }

    public class DocumentDate : Application.DocumentDate
    {
        public Document Document { get; set; }
        public Date Date { get; set; }
    }

    public class DocumentTag : Application.DocumentTag
    {
        public Document Document { get; set; }
        public Tag Tag { get; set; }
    }

    public class Context : DbContext
    {
        protected string _connectionString { get; set; }

        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Date> Dates { get; set; }

        public virtual DbSet<DocumentDate> DocumentDates { get; set; }
        public virtual DbSet<DocumentTag> DocumentTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentTag>()
                .HasKey(dt => new { dt.DocumentId, dt.TagId });
            modelBuilder.Entity<DocumentTag>()
                .HasOne(dt => dt.Document)
                .WithMany(d => d.DocumentTags)
                .HasForeignKey(dt => dt.DocumentId);
            modelBuilder.Entity<DocumentTag>()
                .HasOne(dt => dt.Tag)
                .WithMany(t => t.DocumentTags)
                .HasForeignKey(dt => dt.TagId);

            modelBuilder.Entity<DocumentDate>()
                .HasKey(dt => new { dt.DocumentId, dt.DateId });
            modelBuilder.Entity<DocumentDate>()
                .HasOne(dd => dd.Document)
                .WithMany(d => d.DocumentDates)
                .HasForeignKey(dd => dd.DocumentId);
            modelBuilder.Entity<DocumentDate>()
                .HasOne(dt => dt.Date)
                .WithMany(t => t.DocumentDates)
                .HasForeignKey(dt => dt.DateId);

            modelBuilder
                .Entity<Document>()
                .HasIndex(d => d.Name)
                .IsUnique();

            modelBuilder
                .Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder
                .Entity<Date>()
                .Ignore(d => d.DateString);

            modelBuilder
                .Entity<Date>()
                .HasIndex(d => d.DateString)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}

