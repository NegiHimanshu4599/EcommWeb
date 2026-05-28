using BookService.Application.DTOs;
namespace BookService.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookListDto>> GetAllAsync();
        Task<BookDetailDto> GetByIdAsync(int id);
        Task<PagedResult<BookListDto>> GetBookByFilter(BookFilterDto filterDto);
        Task<BookListDto> AddAsync(CreateBookDto dto);
        Task UpdateAsync(UpdateBookDto dto);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task PermanentDeleteAsync(int id);
        Task<IEnumerable<BookListDto>>GetBulkBooks(IEnumerable<int> ids);
        Task<IEnumerable<BookDetailDto>> GetAllSoftDeleteBooks();
        Task<PagedResult<TrashBookDto>> GetTrashBooks(BookFilterDto dto);
        Task<TrashDashboardDto> GetTrashDashboardAsync();
    }
}