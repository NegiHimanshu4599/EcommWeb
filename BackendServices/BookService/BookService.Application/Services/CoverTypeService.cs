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
    public class CoverTypeService : ICoverTypeService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly ILogger<CoverTypeService> _logger;
        public CoverTypeService(IUnitofWork unitofwork, IMapper mapper, ILogger<CoverTypeService> logger)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<CoverTypeDto> AddAsync(CoverTypeDto dto)
        {
            _logger.LogInformation("Checking if CoverType with name {Name} already exists", dto.Name);
            var exists = await _unitofwork.CoverType.
                FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == dto.Name.ToLower().Trim());
            if (exists != null)
            {
                _logger.LogWarning("CoverType already Exists");
                throw new InvalidOperationException("CoverType already exists");
            }
            var coverType = _mapper.Map<CoverType>(dto);
            _logger.LogInformation("Adding new CoverType with name {Name}", dto.Name);
            await _unitofwork.CoverType.AddAsync(coverType);
            await _unitofwork.SaveAsync();
            return _mapper.Map<CoverTypeDto>(coverType);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Checking if CoverType with Id {Id} exists", id);
            var existing = await _unitofwork.CoverType.GetAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("CoverType not found with Id {Id}", id);
                throw new KeyNotFoundException("CoverType not found");
            }
            _unitofwork.CoverType.Remove(existing);
            await _unitofwork.SaveAsync();
        }

        public async Task<IEnumerable<CoverTypeDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching All CoverTypes");
            var coverTypes = await _unitofwork.CoverType.GetAllAsync();
            return _mapper.Map<IEnumerable<CoverTypeDto>>(coverTypes);
        }

        public async Task<CoverTypeDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching CoverType By Id {Id}", id);
            var coverType = await _unitofwork.CoverType.GetAsync(id);
            if (coverType == null)
            {
                _logger.LogWarning("No CoverType is Founded with Id {Id}", id);
                throw new KeyNotFoundException("No CoverType is Founded By this Id");
            }
            return _mapper.Map<CoverTypeDto>(coverType);
        }

        public async Task UpdateAsync(CoverTypeDto dto)
        {
            _logger.LogInformation("Checking if CoverType with Id {Id} exists", dto.Id);
            var existing = await _unitofwork.CoverType.GetAsync(dto.Id);
            if (existing == null)
            {
                _logger.LogWarning("CoverType Not Found");
                throw new KeyNotFoundException($"CoverType {dto.Id} not found");
            }
            _logger.LogInformation("Checking for duplicate CoverType name {Name} excluding Id {Id}", dto.Name, dto.Id);
            var duplicate = await _unitofwork.CoverType
               .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == dto.Name.ToLower().Trim() && c.Id != dto.Id);
            if (duplicate != null)
            {
                _logger.LogWarning("CoverType Name Already Exists");
                throw new InvalidOperationException("CoverType name already exists");
            }
            _mapper.Map(dto, existing);
            await _unitofwork.SaveAsync();
        }
    }
}
