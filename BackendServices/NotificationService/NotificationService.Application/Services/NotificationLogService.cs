//using AutoMapper;
//using NotificationService.Application.Dtos.NotificationLog;
//using NotificationService.Application.Interface.Services;
//using NotificationService.Domain.Interfaces;

//namespace NotificationService.Application.Services
//{
//    public class NotificationLogService : INotificationLogService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        public NotificationLogService(IUnitOfWork unitOfWork,IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//        public async Task<NotificationLogDto?> GetByIdAsync(int id)
//        {
//            if (id <= 0)
//                throw new ArgumentException("Notification log id must be greater than zero.", nameof(id));
//            var notificationLog =await _unitOfWork.NotificationLogRepository.GetByIdAsync(id);
//            return notificationLog == null? null: _mapper.Map<NotificationLogDto>(notificationLog);
//        }
//        public async Task<IEnumerable<NotificationLogDto>> GetByNotificationIdAsync(int notificationId)
//        {
//            if (notificationId <= 0)
//                throw new ArgumentException("Notification id must be greater than zero.",nameof(notificationId));
//            var notificationLogs =await _unitOfWork.NotificationLogRepository.GetByNotificationIdAsync(notificationId);
//            return _mapper.Map<IEnumerable<NotificationLogDto>>(notificationLogs);
//        }
//    }
//}