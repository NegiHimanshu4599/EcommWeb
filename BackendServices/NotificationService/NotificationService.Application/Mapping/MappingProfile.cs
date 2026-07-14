//using AutoMapper;
//using NotificationService.Application.Dtos.DeviceToken;
//using NotificationService.Application.Dtos.EmailTemplate;
//using NotificationService.Application.Dtos.Notification;
//using NotificationService.Application.Dtos.NotificationLog;
//using NotificationService.Application.Dtos.Otp;
//using NotificationService.Domain.Entities;

//namespace NotificationService.Application.Mapping
//{
//    public class MappingProfile : Profile
//    {
//        public MappingProfile()
//        {
//            CreateMap<CreateNotificationDto, Notification>();
//            CreateMap<UpdateNotificationStatusDto, Notification>();
//            CreateMap<Notification, NotificationDto>();
//            CreateMap<RegisterDeviceTokenDto, DeviceToken>();
//            CreateMap<UpdateDeviceTokenDto, DeviceToken>();
//            CreateMap<DeviceToken, DeviceTokenDto>();
//            CreateMap<CreateEmailTemplateDto, EmailTemplate>();
//            CreateMap<UpdateEmailTemplateDto, EmailTemplate>();
//            CreateMap<EmailTemplate, EmailTemplateDto>();
//            CreateMap<NotificationLog, NotificationLogDto>();
//            CreateMap<GenerateOtpDto, OtpCode>();
//            CreateMap<OtpCode, OtpDto>();
//            CreateMap<ResendOtpDto, OtpCode>();
//        }
//    }
//}