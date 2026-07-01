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
        public async Task AddAsync(EmailTemplate emailTemplate)
        {
            await _context.EmailTemplates.AddAsync(emailTemplate);
        }
        public Task DeleteAsync(EmailTemplate emailTemplate)
        {
            _context.EmailTemplates.Remove(emailTemplate);
            return Task.CompletedTask;
        }
        public async Task<IEnumerable<EmailTemplate>> GetAllAsync()
        {
            return await _context.EmailTemplates.AsNoTracking().ToListAsync();
        }
        public async Task<EmailTemplate?> GetByIdAsync(int id)
        {
            return await _context.EmailTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<EmailTemplate?> GetByNameAsync(string name)
        {
            return await _context.EmailTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
        }
        public Task UpdateAsync(EmailTemplate emailTemplate)
        {
            _context.EmailTemplates.Update(emailTemplate);
                        return Task.CompletedTask;
        }
    }
}
