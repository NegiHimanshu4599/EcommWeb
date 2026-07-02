using NotificationService.Application.Dtos.Notification;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interface.Services
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
        Task<IEnumerable<NotificationDto>> GetAllAsync();
        Task<NotificationDto?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, UpdateNotificationStatusDto dto);
        Task DeleteAsync(int id);
        Task ProcessNotificationAsync(Notification notification);
    }
}
