using NotificationService.Application.Dtos.EmailTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interface.Services
{
    public interface IEmailTemplateRenderer
    {
        Task<RenderedEmailDto> RenderAsync(string templateName, Dictionary<string, string> placeholders);
    }
}
