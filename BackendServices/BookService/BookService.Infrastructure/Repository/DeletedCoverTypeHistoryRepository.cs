using BookService.Domain.Entities.CoverType;
using BookService.Domain.Interfaces;
using BookService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Infrastructure.Repository
{
    public class DeletedCoverTypeHistoryRepository: Repository<DeletedCoverTypeHistory>, IDeletedCoverTypeHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        public DeletedCoverTypeHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
