using NotificationService.Application.Dtos.EmailTemplate;
using NotificationService.Application.Interface.Services;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Services
{
    public class EmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmailTemplateRenderer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<RenderedEmailDto> RenderAsync(string templateName,Dictionary<string, string> placeholders)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Template name is required.",nameof(templateName));
            if (placeholders == null)
                throw new ArgumentNullException(nameof(placeholders));
            var template =await _unitOfWork.EmailTemplateRepository.GetByNameAsync(templateName);
            if (template == null)
                throw new KeyNotFoundException($"Email template '{templateName}' was not found.");
            if (!template.IsActive)
                throw new InvalidOperationException($"Email template '{templateName}' is inactive.");
            var subject = template.Subject;
            var body = template.HtmlBody;
            foreach (var placeholder in placeholders)
            {
                var token = $"{{{{{placeholder.Key}}}}}";
                subject = subject.Replace(token,placeholder.Value ?? string.Empty,StringComparison.OrdinalIgnoreCase);
                body = body.Replace(token,placeholder.Value ?? string.Empty,StringComparison.OrdinalIgnoreCase);
            }
            return new RenderedEmailDto
            {
                Subject = subject,
                HtmlBody = body
            };
        }
    }
}