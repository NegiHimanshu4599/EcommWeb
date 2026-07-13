namespace NotificationService.Application.Dtos.EmailTemplate
{
    public class RenderedEmailDto
    {
        public string Subject { get; set; } = null!;
        public string HtmlBody { get; set; } = null!;
    }
}
