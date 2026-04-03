using AutoMapper;
using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using BookService.Domain.Entities;
using BookService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _logger.LogInformation("Fetching All Categories");
            var categories = await _unitofwork.Category.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching Category By Id {Id}", id);
            var category = await _unitofwork.Category.GetAsync(id);
            if (category == null)
            {
                _logger.LogWarning("No Category is founded with Id {Id}", id);
                throw new KeyNotFoundException("No Category Found");
            }
            return _mapper.Map<CategoryDto>(category);
        }
        public async Task<CategoryDto> AddAsync(CategoryDto dto)
        {
            _logger.LogInformation("Checking if Category with name {Name} already exists", dto.Name);
            var exists = await _unitofwork.Category
           .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == dto.Name.ToLower().Trim());
            if (exists != null)
            {
                _logger.LogWarning("Category already Exists");
                throw new InvalidOperationException("Category already exists");
            }
            var category = _mapper.Map<Category>(dto);
            _logger.LogInformation("Adding new Category with name {Name}", dto.Name);
            await _unitofwork.Category.AddAsync(category);
            await _unitofwork.SaveAsync();
            return _mapper.Map<CategoryDto>(category);
        }
        public async Task UpdateAsync(CategoryDto dto)
        {
            _logger.LogInformation("Checking if Category with Id {Id} exists", dto.Id);
            var existing = await _unitofwork.Category.GetAsync(dto.Id);
            if (existing == null)
            {
                _logger.LogWarning($"Category{dto.Id}not found");
                throw new KeyNotFoundException($"Category {dto.Id} not found");
            }
            _logger.LogInformation("Checking for duplicate Category name {Name} excluding Id {Id}", dto.Name, dto.Id);
            var duplicate = await _unitofwork.Category
              .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == dto.Name.ToLower().Trim() && c.Id != dto.Id);
            if (duplicate != null)
            {
                _logger.LogWarning("Category Name Already Exists");
                throw new InvalidOperationException("Category name already exists");
            }
            _mapper.Map(dto, existing);
            await _unitofwork.SaveAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var existing = await _unitofwork.Category.GetAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Category not found with Id {Id}", id);
                throw new KeyNotFoundException("Category not found");
            }
            _unitofwork.Category.Remove(existing);
            await _unitofwork.SaveAsync();
        }
    }
}
