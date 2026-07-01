using AutoMapper;
using NotificationService.Application.Dtos.EmailTemplate;
using NotificationService.Application.Interface.Services;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmailTemplateService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Template name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.Subject))
                throw new ArgumentException("Subject is required.", nameof(dto.Subject));
            if (string.IsNullOrWhiteSpace(dto.HtmlBody))
                throw new ArgumentException("Html body is required.", nameof(dto.HtmlBody));
            var existing = await _unitOfWork.EmailTemplateRepository.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new InvalidOperationException($"Template '{dto.Name}' already exists.");
            var template = _mapper.Map<EmailTemplate>(dto);
            template.IsActive = true;
            template.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.EmailTemplateRepository.AddAsync(template);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<EmailTemplateDto>(template);
        }
        public async Task<IEnumerable<EmailTemplateDto>> GetAllAsync()
        {
            var templates =await _unitOfWork.EmailTemplateRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmailTemplateDto>>(templates);
        }
        public async Task<IEnumerable<EmailTemplateDto>> GetActiveAsync()
        {
            var templates =await _unitOfWork.EmailTemplateRepository.GetAllAsync();
            var activeTemplates = templates.Where(x => x.IsActive);
            return _mapper.Map<IEnumerable<EmailTemplateDto>>(activeTemplates);
        }
        public async Task<EmailTemplateDto?> GetByIdAsync(int id)
        {
            var template = await _unitOfWork.EmailTemplateRepository.GetByIdAsync(id);
            return template == null ? null : _mapper.Map<EmailTemplateDto>(template);
        }
        public async Task<EmailTemplateDto?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Template name is required.", nameof(name));
            var template = await _unitOfWork.EmailTemplateRepository.GetByNameAsync(name);
            return template == null ? null: _mapper.Map<EmailTemplateDto>(template);
        }
        public async Task UpdateAsync(int id, UpdateEmailTemplateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            var template = await _unitOfWork.EmailTemplateRepository.GetByIdAsync(id);
            if (template == null)
                throw new KeyNotFoundException( $"Template with Id '{id}' was not found.");
            var existing = await _unitOfWork.EmailTemplateRepository.GetByNameAsync(dto.Name);
            if (existing != null && existing.Id != id)
                throw new InvalidOperationException($"Template '{dto.Name}' already exists.");
            _mapper.Map(dto, template);
            template.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.EmailTemplateRepository.UpdateAsync(template);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task ActivateAsync(int id)
        {
            var template =await _unitOfWork.EmailTemplateRepository.GetByIdAsync(id);
            if (template == null)
                throw new KeyNotFoundException($"Template with Id '{id}' was not found.");
            if (template.IsActive)
                return;
            template.IsActive = true;
            template.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.EmailTemplateRepository.UpdateAsync(template);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeactivateAsync(int id)
        {
            var template= await _unitOfWork.EmailTemplateRepository.GetByIdAsync(id);
            if (template == null)
                throw new KeyNotFoundException($"Template with Id '{id}' was not found.");
            if (!template.IsActive)
                return;
            template.IsActive = false;
            template.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.EmailTemplateRepository.UpdateAsync(template);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}