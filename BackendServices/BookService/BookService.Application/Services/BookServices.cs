using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using BookService.Domain.Entities;
using BookService.Domain.Interfaces;

namespace BookService.Application.Services
{
    public class BookServices : IBookService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly ILogger<BookServices> _logger;
        private readonly IFileService _fileService;
        public BookServices(IUnitofWork unitofwork, IMapper mapper, IFileService fileService, ILogger<BookServices> logger)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
        }
        public async Task<BookDto> AddAsync(CreateBookDto dto)
        {
            using var transaction = await _unitofwork.BeginTransactionAsync();
            try
            {
                var categoryExists = await _unitofwork.Category.GetAsync(dto.CategoryId);
                if (categoryExists == null)
                {
                    _logger.LogWarning("Invalid Category Id {CategoryId}", dto.CategoryId);
                    throw new KeyNotFoundException("Invalid Category");
                }
                var coverTypeExists = await _unitofwork.CoverType.GetAsync(dto.CoverTypeId);
                if (coverTypeExists == null)
                {
                    _logger.LogWarning("Invalid CoverType Id {CoverTypeId}", dto.CoverTypeId);
                    throw new KeyNotFoundException("Invalid CoverType");
                }
                var Books = _mapper.Map<Book>(dto);
                if (dto.ImageFile != null)
                {
                    _logger.LogInformation("Saving Image");
                    Books.ImageUrl = await _fileService.SaveFileAsync(dto.ImageFile);
                }
                await _unitofwork.Book.AddAsync(Books);
                await _unitofwork.SaveAsync();
                await transaction.CommitAsync();
                var AddedBook = (await _unitofwork.Book.GetAllAsync(p => p.Id == Books.Id,
                     includeProperties: "Category,CoverType")).FirstOrDefault();
                if (AddedBook == null)
                { 
                    _logger.LogError("Failed to retrieve the added book with Id {Id}", Books.Id);
                    throw new Exception("Book Saving failed");
                }
                return _mapper.Map<BookDto>(AddedBook);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task DeleteAsync(int id)
        {
            var existing = await _unitofwork.Book.GetAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Book not found By Id{Id}", id);
                throw new KeyNotFoundException("Book not found");
            }
            if (!string.IsNullOrEmpty(existing.ImageUrl))
            {
                await _fileService.DeleteFileAsync(existing.ImageUrl);
            }
            _logger.LogInformation("Delete Book By Id {Id}", id);
            _unitofwork.Book.Remove(existing);
            await _unitofwork.SaveAsync();
        }
        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching All Book");
            var books = await _unitofwork.Book
                .GetAllAsync( includeProperties: "Category,CoverType");
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
        public async Task<IEnumerable<BookDto>> GetBookByFilter(BookFilterDto filterDto)
        {
            _logger.LogInformation("Fetching Book with Filter {@Filter}", filterDto);
            var query =  _unitofwork.Book.GetAllAsync(includeProperties: "Category,CoverType");
            var books = (await query).AsQueryable();
            if (!string.IsNullOrEmpty(filterDto.Search))
            {
                _logger.LogInformation("Applying Search Filter: {Search}", filterDto.Search);
                books = books.Where(x => x.Title.Contains(filterDto.Search) || x.Author.Contains(filterDto.Search));
            }
            if (filterDto.CategoryId.HasValue)
            {
                _logger.LogInformation("Applying Category Filter: {CategoryId}", filterDto.CategoryId);
                books = books.Where(x => x.CategoryId == filterDto.CategoryId);
            }
            if (filterDto.CoverTypeId.HasValue)
            {
                _logger.LogInformation("Applying CoverType Filter: {CoverTypeId}", filterDto.CoverTypeId);
                books = books.Where(x => x.CoverTypeId == filterDto.CoverTypeId);
            }
            if (filterDto.MinPrice.HasValue)
            {
                _logger.LogInformation("Applying MinPrice Filter: {MinPrice}", filterDto.MinPrice);
                books = books.Where(x => x.Price >= filterDto.MinPrice);
            }
            if (filterDto.MaxPrice.HasValue)
            {
                _logger.LogInformation("Applying MaxPrice Filter: {MaxPrice}", filterDto.MaxPrice);
                books = books.Where(x => x.Price <= filterDto.MaxPrice);
            }
            _logger.LogInformation("Applying Pagination: PageNumber={PageNumber}, PageSize={PageSize}", filterDto.PageNumber, filterDto.PageSize);
            books = books.Skip((filterDto.PageNumber - 1) * filterDto.PageSize).Take(filterDto.PageSize);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
        public async Task<BookDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching Book By Id {Id}", id);
            var book = await _unitofwork.Book
                .FirstOrDefaultAsync(p => p.Id == id,
                             includeProperties: "Category,CoverType");
            if (book == null)
            {
                _logger.LogWarning("No Book Found with Id {Id}", id);
                throw new KeyNotFoundException("No Book Found");
            }
            return _mapper.Map<BookDto>(book);
        }
        public async Task UpdateAsync(UpdateBookDto dto)
        {
            using var transaction = await _unitofwork.BeginTransactionAsync();
            try
            {
                var existing = await _unitofwork.Book.GetAsync(dto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Book not found: {Id}", dto.Id);
                    throw new KeyNotFoundException("Book not found");
                }
                var categoryExists = await _unitofwork.Category.GetAsync(dto.CategoryId);
                if (categoryExists == null)
                {
                    _logger.LogWarning("Invalid Category Id {CategoryId}", dto.CategoryId);
                    throw new KeyNotFoundException("Invalid Category");
                }
                var coverTypeExists = await _unitofwork.CoverType.GetAsync(dto.CoverTypeId);
                if (coverTypeExists == null)
                {
                    _logger.LogWarning("Invalid CoverType Id {CoverTypeId}", dto.CoverTypeId);
                    throw new KeyNotFoundException("Invalid CoverType");
                }
                _mapper.Map(dto, existing);
                if (dto.ImageFile != null)
                {
                    var newImageUrl = await _fileService.SaveFileAsync(dto.ImageFile);

                    if (!string.IsNullOrEmpty(existing.ImageUrl))
                    {
                        await _fileService.DeleteFileAsync(existing.ImageUrl);
                    }

                    existing.ImageUrl = newImageUrl;
                }
                else
                {
                    // 🔥 Keep existing image
                    existing.ImageUrl = dto.ImageUrl ?? existing.ImageUrl;
                }
                await _unitofwork.SaveAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
