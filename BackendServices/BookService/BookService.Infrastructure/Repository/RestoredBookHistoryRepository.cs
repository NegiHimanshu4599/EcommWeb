using BookService.Domain.Entities.Book;
using BookService.Domain.Interfaces;
using BookService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Infrastructure.Repository
{
    public class RestoredBookHistoryRepository:Repository<RestoredBookHistory>,IRestoredBookHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        public RestoredBookHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
