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
        Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<EmailTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailTemplate>> GetAllAsync( CancellationToken cancellationToken = default);
        Task AddAsync(EmailTemplate emailTemplate, CancellationToken cancellationToken = default);
        void Update(EmailTemplate emailTemplate);
        void Delete(EmailTemplate emailTemplate);
    }
}