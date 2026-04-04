using BookService.Domain.Entities;
using BookService.Domain.Interfaces;
using BookService.Infrastructure.Data;

namespace BookService.Infrastructure.Repository
{
    public class BookRepository:Repository<Book>,IBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
