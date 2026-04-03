using BookService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Domain.Interfaces
{
    public interface ICategoryRepository:IRepository<Category>
    {
    }
}
