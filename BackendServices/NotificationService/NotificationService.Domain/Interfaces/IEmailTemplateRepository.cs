using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces
{
    public interface IEmailTemplateRepository
    {
        Task<EmailTemplate?> GetByIdAsync(int id);
        Task<EmailTemplate?> GetByNameAsync(string name);
        Task<IEnumerable<EmailTemplate>> GetAllAsync();
        Task AddAsync(EmailTemplate emailTemplate);
        Task UpdateAsync(EmailTemplate emailTemplate);
        Task DeleteAsync(EmailTemplate emailTemplate);
    }
}