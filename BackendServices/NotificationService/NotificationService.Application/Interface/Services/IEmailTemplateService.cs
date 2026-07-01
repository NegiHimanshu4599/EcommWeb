using NotificationService.Application.Dtos.EmailTemplate;

namespace NotificationService.Application.Interface.Services
{
    public interface IEmailTemplateService
    {
        Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateDto dto);
        Task<IEnumerable<EmailTemplateDto>> GetAllAsync();
        Task<IEnumerable<EmailTemplateDto>> GetActiveAsync();
        Task<EmailTemplateDto?> GetByIdAsync(int id);
        Task<EmailTemplateDto?> GetByNameAsync(string name);
        Task UpdateAsync(int id, UpdateEmailTemplateDto dto);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
    }
}