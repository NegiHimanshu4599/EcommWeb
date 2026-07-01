using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) 
        {
        }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Notification and NotificationLog relationship
            modelBuilder.Entity<Notification>()
                .HasMany(n => n.NotificationLogs)
                .WithOne(l => l.Notification)
                .HasForeignKey(l => l.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.EmailTemplate)
                .WithMany()
                .HasForeignKey(n => n.EmailTemplateId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Notification>()
                .Property(x => x.Recipient)
                .IsRequired()
                .HasMaxLength(256);
            modelBuilder.Entity<Notification>()
                .Property(x => x.Body)
                .IsRequired()
                .HasMaxLength(4000);
            modelBuilder.Entity<Notification>()
                .Property(x => x.Subject)
                .HasMaxLength(250);
            modelBuilder.Entity<Notification>()
                .Property(x => x.ErrorMessage)
                .HasMaxLength(1000);
            modelBuilder.Entity<Notification>()
                .HasIndex(x => x.UserId);
            modelBuilder.Entity<Notification>()
                .HasIndex(x => x.Status);
            modelBuilder.Entity<Notification>()
                .HasIndex(x => x.NotificationType);
            modelBuilder.Entity<Notification>()
                .HasIndex(x => x.CreatedAt);
            modelBuilder.Entity<NotificationLog>()
                .Property(x => x.Provider) 
                .HasMaxLength(100)  
                .IsRequired();
            modelBuilder.Entity<NotificationLog>()
                .Property(x => x.Response)
                .HasMaxLength(4000);
            modelBuilder.Entity<NotificationLog>()
                .HasIndex(x => x.NotificationId);
            modelBuilder.Entity<NotificationLog>()
                .HasIndex(x => x.Status);

            // DeviceToken indexes
            modelBuilder.Entity<DeviceToken>()
                .Property(x => x.Platform)      
                .IsRequired()        
                .HasMaxLength(20);
            modelBuilder.Entity<DeviceToken>()
                .Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(500);
            modelBuilder.Entity<DeviceToken>()
                .HasIndex(x => x.UserId);
            modelBuilder.Entity<DeviceToken>()
                .HasIndex(x => x.Token)
                .IsUnique();

            // EmailTemplate indexes and constraints
            modelBuilder.Entity<EmailTemplate>()
                .Property(x => x.Name)  
                .IsRequired()    
                .HasMaxLength(100);
            modelBuilder.Entity<EmailTemplate>()
                .Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(250);
            modelBuilder.Entity<EmailTemplate>()
                .Property(x => x.HtmlBody)
                .IsRequired();
            modelBuilder.Entity<EmailTemplate>()
                .HasIndex(x => x.Name)
                .IsUnique();

            // OTP
            modelBuilder.Entity<OtpCode>()
               .Property(x => x.UserId)
               .IsRequired()
               .HasMaxLength(450);
            modelBuilder.Entity<OtpCode>()
                .Property(x => x.Recipient)
                .IsRequired()
                .HasMaxLength(256);
            modelBuilder.Entity<OtpCode>()
                .Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(10);
            modelBuilder.Entity<OtpCode>()
                .HasIndex(x => x.UserId);
            modelBuilder.Entity<OtpCode>()
                .HasIndex(x => x.Recipient);
            modelBuilder.Entity<OtpCode>()
                .HasIndex(x => x.Type);
            modelBuilder.Entity<OtpCode>()
                .HasIndex(x => x.IsUsed);
            modelBuilder.Entity<OtpCode>()
                .HasIndex(x => x.ExpiryTime);
            modelBuilder.Entity<OtpCode>()
                .HasIndex(x => new
                {
                    x.Recipient,
                    x.Type,
                    x.IsUsed
                });
        }
    }
}