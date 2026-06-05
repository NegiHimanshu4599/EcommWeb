using AutoMapper;
using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using BookService.Domain.Entities.Category;
using BookService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookService.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(IUnitofWork unitofwork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all categories");
            var categories = await _unitofwork.Category.Query().Include(x => x.ParentCategory).AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();
            var result = categories.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                ParentCategoryId = x.ParentCategoryId,
                ParentCategoryName = x.ParentCategory != null ? x.ParentCategory.Name : null,
                IsParentCategory = x.ParentCategoryId == null,
                DisplayName = x.ParentCategory != null ? $"{x.ParentCategory.Name} > {x.Name}" : x.Name
            }).ToList();
            var ordered = new List<CategoryDto>();
            var parents = result.Where(x => x.ParentCategoryId == null).OrderBy(x => x.Name).ToList();
            foreach (var parent in parents)
            {
                ordered.Add(parent);
                var children = result.Where(x => x.ParentCategoryId == parent.Id).OrderBy(x => x.Name).ToList();
                ordered.AddRange(children);
            }
            return ordered;
        }
        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching Category By Id {Id}", id);

            var category = await _unitofwork.Category.Query().Include(x => x.ParentCategory).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (category == null)
            {
                _logger.LogWarning("Category not found with Id {Id}", id);
                throw new KeyNotFoundException("Category not found");
            }
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory != null ? category.ParentCategory.Name : null,
                IsParentCategory = category.ParentCategoryId == null,
                DisplayName = category.ParentCategory != null ? $"{category.ParentCategory.Name} > {category.Name}" : category.Name
            };
        }
        public async Task<CategoryDto> AddAsync(CategoryDto dto)
        {
            _logger.LogInformation("Creating Category {Name}", dto.Name);
            // Prevent self parent
            if (dto.ParentCategoryId == dto.Id)
            {
                throw new InvalidOperationException("Category cannot be parent of itself");
            }
            // Duplicate validation
            var duplicate = await _unitofwork.Category.FirstOrDefaultAsync(c => c.Id != dto.Id && !c.IsDeleted && c.Name.ToLower().Trim() == dto.Name.ToLower().Trim() && ((dto.ParentCategoryId == null && c.ParentCategoryId == null) || (dto.ParentCategoryId != null && c.ParentCategoryId == dto.ParentCategoryId)));
            if (duplicate != null)
            {
                _logger.LogWarning("Duplicate category found");
                throw new InvalidOperationException("Category already exists under same parent");
            }
            // Parent validation
            Category? parent = null;
            if (dto.ParentCategoryId != null)
            {
                parent = await _unitofwork.Category.FirstOrDefaultAsync(x => x.Id == dto.ParentCategoryId && !x.IsDeleted);
                if (parent == null)
                {
                    throw new InvalidOperationException("Invalid parent category");
                }
                // Parent-child same name validation
                if (parent.Name.ToLower().Trim() == dto.Name.ToLower().Trim())
                {
                    throw new InvalidOperationException("Child category cannot have same name as parent");
                }
            }
            var category = _mapper.Map<Category>(dto);
            await _unitofwork.Category.AddAsync(category);
            await _unitofwork.SaveAsync();
            _logger.LogInformation("Category created successfully with Id {Id}", category.Id);
            return await GetByIdAsync(category.Id);
        }
        public async Task UpdateAsync(CategoryDto dto)
        {
            _logger.LogInformation("Updating Category {Id}", dto.Id);
            var existing = await _unitofwork.Category.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted);
            if (existing == null)
            {
                _logger.LogWarning("Category not found with Id {Id}", dto.Id);
                throw new KeyNotFoundException("Category not found");
            }
            // Prevent self parent
            if (dto.ParentCategoryId == dto.Id)
            {
                throw new InvalidOperationException("Category cannot be parent of itself");
            }
            // Duplicate validation
            var duplicate = await _unitofwork.Category.FirstOrDefaultAsync(c => c.Id != dto.Id && !c.IsDeleted && c.Name.ToLower().Trim() == dto.Name.ToLower().Trim() && ((dto.ParentCategoryId == null && c.ParentCategoryId == null) || (dto.ParentCategoryId != null && c.ParentCategoryId == dto.ParentCategoryId)));
            if (duplicate != null)
            {
                _logger.LogWarning("Duplicate category found");
                throw new InvalidOperationException("Category already exists under same parent");
            }
            // Parent validation
            Category? parent = null;
            if (dto.ParentCategoryId != null)
            {
                parent = await _unitofwork.Category.FirstOrDefaultAsync(x => x.Id == dto.ParentCategoryId && !x.IsDeleted);
                if (parent == null)
                {
                    throw new InvalidOperationException("Invalid parent category");
                }
                // Prevent same parent-child name
                if (parent.Name.ToLower().Trim() == dto.Name.ToLower().Trim())
                {
                    throw new InvalidOperationException("Child category cannot have same name as parent");
                }
            }
            _mapper.Map(dto, existing);
            await _unitofwork.SaveAsync();
            _logger.LogInformation("Category updated successfully");
        }
        public async Task DeleteAsync(int id)
        {
            var existing = await _unitofwork.Category.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (existing == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            var hasChildren = await _unitofwork.Category.Query().AnyAsync(x => x.ParentCategoryId == id && !x.IsDeleted);
            if (hasChildren)
            {
                throw new InvalidOperationException("Cannot delete category that contains subcategories");
            }
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            await _unitofwork.DeletedCategory.AddAsync(new DeletedCategoryHistory
            {
                CategoryId = existing.Id,
                Name = existing.Name,
                DeletedAt = DateTime.UtcNow,
                IsPermanentDeleted = false
            });
            await _unitofwork.SaveAsync();
        }
        public async Task RestoreAsync(int id)
        {
            var existing = await _unitofwork.Category.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
            if (existing == null)
            {
                throw new KeyNotFoundException("Deleted Category not found");
            }
            existing.IsDeleted = false;
            existing.DeletedAt = null;
            existing.RestoredAt = DateTime.UtcNow;
            await _unitofwork.RestoredCategory.AddAsync(new RestoredCategoryHistory
            {
                CategoryId = existing.Id,
                Name = existing.Name,
                RestoredAt = DateTime.UtcNow
            });
            await _unitofwork.SaveAsync();
        }
        public async Task PermanentDeleteAsync(int id)
        {
            var existing = await _unitofwork.Category.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
            if (existing == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            var history = await _unitofwork.DeletedCategory.FirstOrDefaultAsync(x => x.CategoryId == existing.Id);
            if (history != null)
            {
                history.IsPermanentDeleted = true;
            }
            _unitofwork.Category.Remove(existing);
            await _unitofwork.SaveAsync();
        }
        public async Task<IEnumerable<CategoryDto>> GetAllSoftDeleteCategory()
        {
            var Category = await _unitofwork.Category.Query().AsNoTracking().Where(x => x.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(Category);
        }
        public async Task<CategoryTrashDashboardDto>GetTrashDashboardAsync()
        {
            return new CategoryTrashDashboardDto
            {
                TotalDeletedCategories = await _unitofwork.DeletedCategory.Query().CountAsync(),
                RestoredCategories =await _unitofwork.RestoredCategory.Query().CountAsync(),
                PermanentlyDeletedCategories = await _unitofwork.DeletedCategory.Query().CountAsync(x => x.IsPermanentDeleted),
                LastDeletedAt =await _unitofwork.DeletedCategory.Query().OrderByDescending(x => x.DeletedAt).Select(x => (DateTime?)x.DeletedAt).FirstOrDefaultAsync()
            };
        }
        public async Task<PagedResult<CategoryTrashDto>>GetTrashCategories(BookFilterDto filterDto)
        {
            IQueryable<Category> query =_unitofwork.Category.Query().Where(x => x.IsDeleted).Include(x => x.ParentCategory).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(filterDto.Search))
            {
                query = query.Where(x =>x.Name.Contains(filterDto.Search));
            }
            var totalCount = await query.CountAsync();
            var data = await query.OrderByDescending(x => x.DeletedAt).Skip((filterDto.PageNumber - 1) * filterDto.PageSize).Take(filterDto.PageSize).ToListAsync();
            return new PagedResult<CategoryTrashDto>
            {
                Data = data.Select(x => new CategoryTrashDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentCategoryName = x.ParentCategory != null ? x.ParentCategory.Name : null,
                    IsDeleted = x.IsDeleted,
                    DeletedAt = x.DeletedAt
                }),
                TotalCount = totalCount,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize
            };
        }
    }
}