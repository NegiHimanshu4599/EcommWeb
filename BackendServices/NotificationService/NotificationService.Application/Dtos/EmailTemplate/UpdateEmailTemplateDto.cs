namespace NotificationService.Application.Dtos.EmailTemplate
{
    public class UpdateEmailTemplateDto
    {
        public string Name { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string HtmlBody { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
