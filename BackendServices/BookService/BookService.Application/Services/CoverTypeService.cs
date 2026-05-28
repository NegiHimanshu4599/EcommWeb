using AutoMapper;
using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using BookService.Domain.Entities.CoverType;
using BookService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookService.Application.Services
{
    public class CoverTypeService : ICoverTypeService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly ILogger<CoverTypeService> _logger;
        public CoverTypeService(IUnitofWork unitofwork, IMapper mapper,
            ILogger<CoverTypeService> logger)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<CoverTypeDto> AddAsync(CoverTypeDto dto)
        {
            var exists = await _unitofwork.CoverType.
                FirstOrDefaultAsync(c =>!c.IsDeleted&& c.Name.ToLower().Trim() == dto.Name.ToLower().Trim());
            if (exists != null)
            {
                _logger.LogWarning("CoverType already Exists");
                throw new InvalidOperationException("CoverType already exists");
            }
            var coverType = _mapper.Map<CoverType>(dto);
            await _unitofwork.CoverType.AddAsync(coverType);
            await _unitofwork.SaveAsync();
            return _mapper.Map<CoverTypeDto>(coverType);
        }
        public async Task DeleteAsync(int id)
        {
            var existing = await _unitofwork.CoverType.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (existing == null)
            {
                throw new KeyNotFoundException("CoverType not found");
            }
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            await _unitofwork.DeletedCoverType.AddAsync(new DeletedCoverTypeHistory
            {
                CoverTypeId = existing.Id,
                Name = existing.Name,
                DeletedAt = DateTime.UtcNow,
                IsPermanentDeleted = false
            });
            await _unitofwork.SaveAsync();
        }
        public async Task<IEnumerable<CoverTypeDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching All CoverTypes");
            var coverTypes = await _unitofwork.CoverType.GetAllAsync(x=>!x.IsDeleted);
            return _mapper.Map<IEnumerable<CoverTypeDto>>(coverTypes);
        }
        public async Task<CoverTypeDto> GetByIdAsync(int id)
        {
            var coverType = await _unitofwork.CoverType.FirstOrDefaultAsync(x=>x.Id==id&& !x.IsDeleted);
            if (coverType == null)
            {
                _logger.LogWarning("No CoverType is Founded with Id {Id}", id);
                throw new KeyNotFoundException("No CoverType is Founded By this Id");
            }
            return _mapper.Map<CoverTypeDto>(coverType);
        }
        public async Task UpdateAsync(CoverTypeDto dto)
        {
            var existing = await _unitofwork.CoverType.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == dto.Id);
            if (existing == null)
            {
                _logger.LogWarning("CoverType Not Found");
                throw new KeyNotFoundException($"CoverType {dto.Id} not found");
            }
            var duplicate = await _unitofwork.CoverType
               .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == dto.Name.ToLower().Trim()&& !c.IsDeleted && c.Id != dto.Id);
            if (duplicate != null)
            {
                _logger.LogWarning("CoverType Name Already Exists");
                throw new InvalidOperationException("CoverType name already exists");
            }
            _mapper.Map(dto, existing);
            await _unitofwork.SaveAsync();
        }
        public async Task RestoreAsync(int id)
        {
            var existing = await _unitofwork.CoverType.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
            if (existing == null)
            {
                throw new KeyNotFoundException("Deleted CoverType not found");
            }
            existing.IsDeleted = false;
            existing.DeletedAt = null;
            existing.RestoredAt = DateTime.UtcNow;
            await _unitofwork.RestoredCoverType.AddAsync(new RestoredCoverTypeHistory
            {
                CoverTypeId = existing.Id,
                Name = existing.Name,
                RestoredAt = DateTime.UtcNow
            });
            await _unitofwork.SaveAsync();
        }
        public async Task PermanentDeleteAsync(int id)
        {
            var existing = await _unitofwork.CoverType.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
            if (existing == null)
            {
                throw new KeyNotFoundException("CoverType not found");
            }
            var history = await _unitofwork.DeletedCoverType.FirstOrDefaultAsync(x => x.CoverTypeId == existing.Id);
            if (history != null)
            {
                history.IsPermanentDeleted = true;
            }
            _unitofwork.CoverType.Remove(existing);
            await _unitofwork.SaveAsync();
        }
        public async Task<IEnumerable<CoverTypeDto>> GetAllSoftDeleteCoverType()
        {
            var Covertype = await _unitofwork.CoverType.Query().AsNoTracking().Where(x => x.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<CoverTypeDto>>(Covertype);
        }
        public async Task<CoverTypeTrashDashboardDto> GetTrashDashboardAsync()
        {
            return new CoverTypeTrashDashboardDto
            {
                TotalDeletedCoverTypes = await _unitofwork.DeletedCoverType.Query().CountAsync(),
                RestoredCoverTypes = await _unitofwork.RestoredCoverType.Query().CountAsync(),
                PermanentlyDeletedCoverTypes = await _unitofwork.DeletedCoverType.Query().CountAsync(x => x.IsPermanentDeleted),
                LastDeletedAt = await _unitofwork.DeletedCoverType.Query().OrderByDescending(x => x.DeletedAt).Select(x => (DateTime?)x.DeletedAt).FirstOrDefaultAsync()
            };
        }
        public async Task<PagedResult<CoverTypeTrashDto>> GetTrashCoverTypes(BookFilterDto filterDto)
        {
            IQueryable<CoverType> query = _unitofwork.CoverType.Query().Where(x => x.IsDeleted).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(filterDto.Search))
            {
                query = query.Where(x => x.Name.Contains(filterDto.Search));
            }
            var totalCount = await query.CountAsync();
            var data = await query.OrderByDescending(x => x.DeletedAt).Skip((filterDto.PageNumber - 1) * filterDto.PageSize).Take(filterDto.PageSize).ToListAsync();
            return new PagedResult<CoverTypeTrashDto>
            {
                Data = data.Select(x => new CoverTypeTrashDto
                {
                    Id = x.Id,
                    Name = x.Name,
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