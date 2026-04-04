using BookService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<BookDto> GetByIdAsync(int id);
        Task<IEnumerable<BookDto>> GetBookByFilter(BookFilterDto filterDto);
        Task<BookDto> AddAsync(CreateBookDto dto);
        Task UpdateAsync(UpdateBookDto dto);
        Task DeleteAsync(int id);
    }
}
