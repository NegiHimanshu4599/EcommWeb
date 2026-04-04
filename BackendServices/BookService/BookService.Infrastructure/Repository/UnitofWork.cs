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
        }
        public ICategoryRepository Category { private set; get; }
        public IBookRepository Book { private set; get; }
        public ICoverTypeRepository CoverType { private set; get; }
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
