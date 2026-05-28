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
    public class RestoredCoverTypeHistoryRepository:Repository<RestoredCoverTypeHistory>,IRestoredCoverTypeHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        public RestoredCoverTypeHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
