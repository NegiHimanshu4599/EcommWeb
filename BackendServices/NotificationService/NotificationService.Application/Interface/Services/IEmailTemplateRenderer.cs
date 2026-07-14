using NotificationService.Application.Dtos.EmailTemplate;

namespace NotificationService.Application.Interface.Services
{
    public interface IEmailTemplateRenderer
    {
        Task<RenderedEmailDto> RenderAsync(string templateName, Dictionary<string, string> placeholders);
    }
}
