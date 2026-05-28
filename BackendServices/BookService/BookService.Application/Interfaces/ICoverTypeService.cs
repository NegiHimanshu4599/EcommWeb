using BookService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.Interfaces
{
    public interface ICoverTypeService
    {
        Task<IEnumerable<CoverTypeDto>> GetAllAsync();
        Task<CoverTypeDto> GetByIdAsync(int id);
        Task<CoverTypeDto> AddAsync(CoverTypeDto dto);
        Task UpdateAsync(CoverTypeDto dto);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task PermanentDeleteAsync(int id);
        Task<IEnumerable<CoverTypeDto>> GetAllSoftDeleteCoverType();
        Task<CoverTypeTrashDashboardDto> GetTrashDashboardAsync();
        Task<PagedResult<CoverTypeTrashDto>> GetTrashCoverTypes (BookFilterDto filterDto);
    }
}
