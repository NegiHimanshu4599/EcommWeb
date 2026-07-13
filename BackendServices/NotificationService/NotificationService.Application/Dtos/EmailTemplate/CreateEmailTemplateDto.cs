namespace NotificationService.Application.Dtos.EmailTemplate
{
    public class CreateEmailTemplateDto 
    {
        public string Name { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string HtmlBody { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }
}
