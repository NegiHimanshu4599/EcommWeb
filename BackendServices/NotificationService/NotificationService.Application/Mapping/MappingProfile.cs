using AutoMapper;
using NotificationService.Application.Dtos.DeviceToken;
using NotificationService.Application.Dtos.EmailTemplate;
using NotificationService.Application.Dtos.Notification;
using NotificationService.Application.Dtos.NotificationLog;
using NotificationService.Application.Dtos.Otp;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Notification
            CreateMap<CreateNotificationDto, Notification>();
            CreateMap<UpdateNotificationStatusDto, Notification>();
            CreateMap<Notification, NotificationDto>();

            // DeviceToken
            CreateMap<RegisterDeviceTokenDto, DeviceToken>();
            CreateMap<UpdateDeviceTokenDto, DeviceToken>();
            CreateMap<DeviceToken, DeviceTokenDto>();

            // EmailTemplate
            CreateMap<CreateEmailTemplateDto, EmailTemplate>();
            CreateMap<UpdateEmailTemplateDto, EmailTemplate>();
            CreateMap<EmailTemplate, EmailTemplateDto>();

            // NotificationLog
            CreateMap<NotificationLog, NotificationLogDto>();

            //OTP
            CreateMap<GenerateOtpDto, OtpCode>();
            CreateMap<OtpCode, OtpDto>();
            CreateMap<ResendOtpDto, OtpCode>();
        }
    }
}