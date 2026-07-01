using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.EmailTemplate
{
    public class RenderedEmailDto
    {
        public string Subject { get; set; } = null!;
        public string HtmlBody { get; set; } = null!;
    }
}
