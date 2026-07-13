using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Repository
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly ApplicationDbContext _context;
        public EmailTemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(EmailTemplate emailTemplate, CancellationToken cancellationToken = default)
        {
            await _context.EmailTemplates.AddAsync(emailTemplate, cancellationToken);
        }
        public void Delete(EmailTemplate emailTemplate)
        {
            _context.EmailTemplates.Remove(emailTemplate);
        }
        public async Task<IEnumerable<EmailTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.EmailTemplates.AsNoTracking().ToListAsync(cancellationToken);
        }
        public async Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.EmailTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        }
        public async Task<EmailTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.EmailTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name,cancellationToken);
        }
        public void Update(EmailTemplate emailTemplate)
        {
            _context.EmailTemplates.Update(emailTemplate);
        }
    }
}