using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookService.Domain.Entities;
using BookService.Domain.Interfaces;
using BookService.Infrastructure.Data;

namespace BookService.Infrastructure.Repository
{
    public class CoverTypeRepository:Repository<CoverType> , ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
