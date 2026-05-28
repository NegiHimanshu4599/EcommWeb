using BookService.Domain.Entities.Category;
using BookService.Domain.Interfaces;
using BookService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Infrastructure.Repository
{
    public  class DeletedCategoryHistoryRepository: Repository<DeletedCategoryHistory> , IDeletedCategoryHistory
    {
        private readonly ApplicationDbContext _context;
        public DeletedCategoryHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
