using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Domain.Interfaces
{
    public interface IUnitofWork
    {
        ICategoryRepository Category { get; }
        IBookRepository Book { get; }
        ICoverTypeRepository CoverType { get; }
        IDeletedBookHistoryRepository DeletedBook { get; }
        IRestoredBookHistoryRepository RestoredBook { get; }
        IDeletedCategoryHistory DeletedCategory { get; }
        IRestoredCategoryHistory RestoredCategory { get; }
        IDeletedCoverTypeHistoryRepository DeletedCoverType { get; }
        IRestoredCoverTypeHistoryRepository RestoredCoverType { get; }
        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
