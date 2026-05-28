using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookService.Domain.Interfaces;
using BookService.Infrastructure.Data;

namespace BookService.Infrastructure.Repository
{
    public class UnitofWork : IUnitofWork
    {
        private readonly ApplicationDbContext _context;
        public UnitofWork(ApplicationDbContext context)
        {
            _context = context;
                Category = new CategoryRepository(context);
                Book = new BookRepository(context);
                CoverType = new CoverTypeRepository(context);
                DeletedBook = new DeletedBookHistoryRepository(context);
            RestoredBook = new RestoredBookHistoryRepository(context);
            DeletedCategory = new DeletedCategoryHistoryRepository(context);
            RestoredCategory = new RestoredCategoryHistoryRepository(context);
            DeletedCoverType = new DeletedCoverTypeHistoryRepository(context);
            RestoredCoverType = new RestoredCoverTypeHistoryRepository(context);
        }
        public ICategoryRepository Category { private set; get; }
        public IBookRepository Book { private set; get; }
        public ICoverTypeRepository CoverType { private set; get; }
        public IDeletedBookHistoryRepository DeletedBook { private set; get; }
        public IRestoredBookHistoryRepository RestoredBook { private set; get; }
        public IDeletedCategoryHistory DeletedCategory { private set; get; }
        public IRestoredCategoryHistory RestoredCategory { private set; get; }
        public IDeletedCoverTypeHistoryRepository DeletedCoverType { private set; get; }
        public IRestoredCoverTypeHistoryRepository RestoredCoverType { private set; get; }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
