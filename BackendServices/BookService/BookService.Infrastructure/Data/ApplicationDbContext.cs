using BookService.Domain.Entities.Book;
using BookService.Domain.Entities.Category;
using BookService.Domain.Entities.CoverType;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Infrastructure.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<DeletedBookHistory> DeletedBookHistories { get; set; }
        public DbSet<RestoredBookHistory> RestoredBookHistories { get; set; }
        public DbSet<DeletedCategoryHistory> DeletedCategories { get; set; }
        public DbSet<RestoredCategoryHistory> RestoredCategories { get; set; }
        public DbSet<DeletedCoverTypeHistory> DeletedCoverTypes { get; set; }
        public DbSet<RestoredCoverTypeHistory> RestoredCoverTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>()
                .HasIndex(x => x.ISBN)
                .IsUnique();
            builder.Entity<Book>()
                .HasIndex(x => x.Title);
            builder.Entity<Book>()
                .HasIndex(x => x.Author);
            builder.Entity<Book>()
                .HasOne(x => x.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(x => x.CategoryId);
            builder.Entity<Book>()
                .HasOne(x => x.CoverType)
                .WithMany(c => c.Books)
                .HasForeignKey(x => x.CoverTypeId);
            builder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany()
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Category>()
                .HasIndex(c => new { c.Name, c.ParentCategoryId })
                .IsUnique();
            builder.Entity<Book>()
                .Property(b => b.RowVersion)
                .IsRowVersion();
        }
    }
}
