using AutoMapper;
using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using BookService.Domain.Entities.Book;
using BookService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookService.Application.Services
{
    public class BookServices : IBookService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly ILogger<BookServices> _logger;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookServices(IUnitofWork unitofwork, IMapper mapper, IFileService fileService, IHttpContextAccessor httpContextAccessor, ILogger<BookServices> logger)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BookListDto> AddAsync(CreateBookDto dto)
        {
            using var transaction = await _unitofwork.BeginTransactionAsync();
            string? uploadedImagePath = null;
            try
            {
                var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"];
                _logger.LogInformation(
                    "Adding Book | CorrelationId: {CorrelationId}", correlationId);
                var categoryExists = await _unitofwork.Category.FirstOrDefaultAsync(x =>
                        !x.IsDeleted && x.Id == dto.CategoryId);
                if (categoryExists == null)
                {
                    _logger.LogWarning("Invalid Category Id {CategoryId}", dto.CategoryId);
                    throw new KeyNotFoundException("Invalid Category");
                }
                var coverTypeExists = await _unitofwork.CoverType.FirstOrDefaultAsync(x =>
                        !x.IsDeleted && x.Id == dto.CoverTypeId);
                if (coverTypeExists == null)
                {
                    _logger.LogWarning("Invalid CoverType Id {CoverTypeId}", dto.CoverTypeId);
                    throw new KeyNotFoundException("Invalid CoverType");
                }
                var book = _mapper.Map<Book>(dto);
                // Upload image first
                if (dto.ImageFile != null)
                {
                    uploadedImagePath = await _fileService.SaveFileAsync(dto.ImageFile);
                    book.ImageUrl = uploadedImagePath;
                }
                await _unitofwork.Book.AddAsync(book);
                await _unitofwork.SaveAsync();
                await transaction.CommitAsync();
                var addedBook = await _unitofwork.Book.FirstOrDefaultAsync(
                    x => x.Id == book.Id, includeProperties: "Category,CoverType");
                if (addedBook == null)
                {
                    _logger.LogError("Failed to retrieve added Book {Id}", book.Id);
                    throw new Exception("Book saving failed");
                }
                return _mapper.Map<BookListDto>(addedBook);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Delete uploaded image if DB save failed
                if (!string.IsNullOrEmpty(uploadedImagePath))
                {
                    await _fileService.DeleteFileAsync(uploadedImagePath);
                }
                _logger.LogError(ex, "Error while adding book");
                throw;
            }
        }
        public async Task UpdateAsync(UpdateBookDto dto)
        {
            using var transaction = await _unitofwork.BeginTransactionAsync();
            string? newImagePath = null;
            string? oldImagePath = null;
            try
            {
                var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"];
                _logger.LogInformation("Updating Book {Id} | CorrelationId: {CorrelationId}", dto.Id, correlationId);

                var existing = await _unitofwork.Book.FirstOrDefaultAsync(
                        x => x.Id == dto.Id && !x.IsDeleted);
                if (existing == null)
                {
                    _logger.LogWarning("Book not found {Id}", dto.Id);
                    throw new KeyNotFoundException("Book not found");
                }
                var categoryExists = await _unitofwork.Category.FirstOrDefaultAsync(x => !x.IsDeleted &&
                        x.Id == dto.CategoryId);
                if (categoryExists == null)
                {
                    _logger.LogWarning("Invalid Category Id {CategoryId}", dto.CategoryId);
                    throw new KeyNotFoundException("Invalid Category");
                }
                var coverTypeExists = await _unitofwork.CoverType.FirstOrDefaultAsync(x => !x.IsDeleted &&
                        x.Id == dto.CoverTypeId);
                if (coverTypeExists == null)
                {
                    _logger.LogWarning("Invalid CoverType Id {CoverTypeId}", dto.CoverTypeId);
                    throw new KeyNotFoundException("Invalid CoverType");
                }
                oldImagePath = existing.ImageUrl;
                _mapper.Map(dto, existing);
                // Upload new image
                if (dto.ImageFile != null)
                {
                    newImagePath = await _fileService.SaveFileAsync(dto.ImageFile);
                    existing.ImageUrl = newImagePath;
                }
                else
                {
                    existing.ImageUrl = dto.ImageUrl ?? existing.ImageUrl;
                }
                await _unitofwork.SaveAsync();
                await transaction.CommitAsync();
                // Delete old image AFTER successful commit
                if (dto.ImageFile != null && !string.IsNullOrEmpty(oldImagePath) && oldImagePath != newImagePath)
                {
                    try
                    {
                        await _fileService.DeleteFileAsync(oldImagePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete old image {ImagePath}", oldImagePath);
                    }
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                // Delete newly uploaded image if update failed
                if (!string.IsNullOrEmpty(newImagePath))
                {
                    await _fileService.DeleteFileAsync(newImagePath);
                }
                _logger.LogError(ex, "Concurrency conflict for Book {Id}", dto.Id);
                throw new Exception("This book was modified by another user.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Delete newly uploaded image if update failed
                if (!string.IsNullOrEmpty(newImagePath))
                {
                    await _fileService.DeleteFileAsync(newImagePath);
                }
                _logger.LogError(ex, "Error while updating book");
                throw;
            }
        }
        public async Task DeleteAsync(int id)
        {
            try
            {
                var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"];
                _logger.LogInformation("Deleting Book {Id} | CorrelationId: {CorrelationId}", id, correlationId);
                var existing = await _unitofwork.Book.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (existing == null)
                {
                    throw new KeyNotFoundException("Book not found");
                }
                existing.IsDeleted = true;
                existing.DeletedAt = DateTime.UtcNow;
                // SAVE HISTORY
                await _unitofwork.DeletedBook.AddAsync(new DeletedBookHistory
                {
                    BookId = existing.Id,
                    Title = existing.Title,
                    Author = existing.Author,
                    ISBN = existing.ISBN,
                    Price = existing.Price,
                    ImageUrl = existing.ImageUrl,
                    DeletedAt = DateTime.UtcNow,
                    IsPermanentDeleted = false
                });
                await _unitofwork.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting book");
                throw;
            }
        }
        public async Task<IEnumerable<BookListDto>> GetAllAsync()
        {
            //Read-only queries should not track entities.
            var books = await _unitofwork.Book.Query().AsNoTracking().Where(x => !x.IsDeleted).Include(x => x.Category).Include(x => x.CoverType).ToListAsync();
            return _mapper.Map<IEnumerable<BookListDto>>(books);
        }
        public async Task<BookDetailDto> GetByIdAsync(int id)
        {
            var book = await _unitofwork.Book.Query().AsNoTracking().Include(x => x.Category).Include(x => x.CoverType).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (book == null)
            {
                _logger.LogWarning("Book not found with Id {Id}", id);
                throw new KeyNotFoundException("Book not found");
            }
            return _mapper.Map<BookDetailDto>(book);
        }
        public async Task<PagedResult<BookListDto>> GetBookByFilter(BookFilterDto filterDto)
        {
            var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"];
            _logger.LogInformation("Fetching Books | Filter: {@Filter} | CorrelationId: {Id}", filterDto, correlationId);
            IQueryable<Book> query = _unitofwork.Book.Query().AsNoTracking().Include(x => x.Category).Include(x => x.CoverType);
            // SEARCH
            if (!string.IsNullOrWhiteSpace(filterDto.Search))
            {
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{filterDto.Search}%") || EF.Functions.Like(x.Author, $"%{filterDto.Search}%"));
            }
            // CATEGORY
            if (filterDto.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == filterDto.CategoryId);
            }
            // COVER TYPE
            if (filterDto.CoverTypeId.HasValue)
            {
                query = query.Where(x => x.CoverTypeId == filterDto.CoverTypeId);
            }
            // PRICE
            if (filterDto.MinPrice.HasValue)
            {
                query = query.Where(x => x.Price >= filterDto.MinPrice);
            }
            if (filterDto.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= filterDto.MaxPrice);
            }
            // SOFT DELETE PAGE
            if (filterDto.IsDeletedPage)
            {
                query = query.Where(x => x.IsDeleted);
            }
            else
            {
                query = query.Where(x => !x.IsDeleted);
            }
            // SORTING
            query = filterDto.SortBy?.ToLower() switch
            {
                "title" => filterDto.IsDescending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
                "price" => filterDto.IsDescending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
                "stock" => filterDto.IsDescending ? query.OrderByDescending(x => x.StockQuantity) : query.OrderBy(x => x.StockQuantity),
                _ => query.OrderByDescending(x => x.Id)
            };
            // TOTAL COUNT
            var totalCount = await query.CountAsync();
            // PAGINATION
            var result = await query.Skip((filterDto.PageNumber - 1) * filterDto.PageSize).Take(filterDto.PageSize).ToListAsync();
            return new PagedResult<BookListDto>
            {
                Data = _mapper.Map<IEnumerable<BookListDto>>(result),
                TotalCount = totalCount,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize
            };
        }
        public async Task<IEnumerable<BookListDto>> GetBulkBooks(IEnumerable<int> ids)
        {
            _logger.LogInformation("Fetching Bulk Books {@Ids}", ids);
            if (ids == null || !ids.Any())
            {
                return Enumerable.Empty<BookListDto>();
            }
            var books = await _unitofwork.Book.Query().AsNoTracking().Where(x => ids.Contains(x.Id) && !x.IsDeleted).Include(x => x.Category).Include(x => x.CoverType).ToListAsync();
            return _mapper.Map<IEnumerable<BookListDto>>(books);
        }
        public async Task RestoreAsync(int id)
        {
            try
            {
                var existing = await _unitofwork.Book.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
                if (existing == null)
                {
                    _logger.LogWarning("Deleted Book not found {Id}", id);
                    throw new KeyNotFoundException("Deleted book not found");
                }
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                existing.RestoredAt = DateTime.UtcNow;
                await _unitofwork.RestoredBook.AddAsync(
                           new RestoredBookHistory
                           {
                               BookId = existing.Id,
                               Title = existing.Title,
                               Author = existing.Author,
                               ISBN = existing.ISBN,
                               Price = existing.Price,
                               ImageUrl = existing.ImageUrl,
                               RestoredAt = DateTime.UtcNow
                           });
                await _unitofwork.SaveAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict while restoring Book {Id}", id);
                throw new Exception("Book already modified by another user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while restoring Book {Id}", id);
                throw;
            }
        }
        public async Task PermanentDeleteAsync(int id)
        {
            using var transaction = await _unitofwork.BeginTransactionAsync();
            string? imagePath = null;
            try
            {
                var existing = await _unitofwork.Book.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
                if (existing == null)
                {
                    _logger.LogWarning("Book not found {Id}", id);
                    throw new KeyNotFoundException("Book not found");
                }
                imagePath = existing.ImageUrl;
                var history = await _unitofwork.DeletedBook.FirstOrDefaultAsync(x => x.BookId == existing.Id);
                if (history != null)
                {
                    history.IsPermanentDeleted = true;
                }
                _unitofwork.Book.Remove(existing);
                await _unitofwork.SaveAsync();
                await transaction.CommitAsync();
                // DELETE IMAGE
                if (!string.IsNullOrEmpty(imagePath))
                {
                    try
                    {
                        await _fileService.DeleteFileAsync(imagePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete image {ImagePath}", imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error while permanently deleting Book {Id}", id);
                throw;
            }
        }
        public async Task<IEnumerable<BookDetailDto>> GetAllSoftDeleteBooks()
        {
            var books = await _unitofwork.Book.Query().AsNoTracking().Where(x => x.IsDeleted).Include(x => x.Category).Include(x => x.CoverType).ToListAsync();
            return _mapper.Map<IEnumerable<BookDetailDto>>(books);
        }
        public async Task<TrashDashboardDto> GetTrashDashboardAsync()
        {
            // TOTAL SOFT DELETE HISTORY
            var totalDeletedBooks = await _unitofwork.DeletedBook.Query().CountAsync();
            // TOTAL RESTORE HISTORY
            var restoredBooks = await _unitofwork.RestoredBook.Query().CountAsync();
            // TOTAL PERMANENT DELETE HISTORY
            var permanentlyDeletedBooks = await _unitofwork.DeletedBook.Query().CountAsync(x => x.IsPermanentDeleted);
            var lastDeleted = await _unitofwork.DeletedBook.Query().OrderByDescending(x => x.DeletedAt).Select(x => (DateTime?)x.DeletedAt).FirstOrDefaultAsync();
            return new TrashDashboardDto
            {
                TotalDeletedBooks = totalDeletedBooks,
                RestoredBooks = restoredBooks,
                PermanentlyDeletedBooks = permanentlyDeletedBooks,
                LastDeletedAt = lastDeleted
            };
        }
        public async Task<PagedResult<TrashBookDto>> GetTrashBooks(BookFilterDto filterDto)
        {
            IQueryable<Book> query = _unitofwork.Book.Query().AsNoTracking().Where(x => x.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filterDto.Search))
            {
                query = query.Where(x => x.Title.Contains(filterDto.Search) || x.Author.Contains(filterDto.Search) || x.ISBN.Contains(filterDto.Search));
            }
            // SORTING
            query = filterDto.SortBy?.ToLower() switch
            {
                "title" => filterDto.IsDescending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
                "price" => filterDto.IsDescending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
                "stock" => filterDto.IsDescending ? query.OrderByDescending(x => x.StockQuantity) : query.OrderBy(x => x.StockQuantity),
                _ => query.OrderByDescending(x => x.DeletedAt)
            };
            var totalCount = await query.CountAsync();
            var data = await query.Skip((filterDto.PageNumber - 1) * filterDto.PageSize).Take(filterDto.PageSize).ToListAsync();
            return new PagedResult<TrashBookDto>
            {
                Data = _mapper.Map<IEnumerable<TrashBookDto>>(data),
                TotalCount = totalCount,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize
            };
        }
    }
}
