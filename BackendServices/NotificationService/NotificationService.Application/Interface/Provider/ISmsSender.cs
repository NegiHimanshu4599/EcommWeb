using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interface.Provider
{
    public interface ISmsSender
    {
        Task SendAsync(string phoneNumber, string message);
    }
}
