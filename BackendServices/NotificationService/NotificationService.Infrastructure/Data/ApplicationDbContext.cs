using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

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
                .HasIndex(x => x.Priority);
            modelBuilder.Entity<Notification>()
                .HasIndex(x => x.CreatedAt);
            modelBuilder.Entity<Notification>()
                .Property(x => x.NotificationType)
                .HasConversion<int>();
            modelBuilder.Entity<Notification>()
                .Property(x => x.Status)
                .HasConversion<int>();
            modelBuilder.Entity<Notification>()
                .Property(x => x.Priority)
                .HasConversion<int>();
            modelBuilder.Entity<Notification>()
                .Property(x => x.RequestId) 
                .IsRequired()  
                .HasMaxLength(100);
            modelBuilder.Entity<Notification>()
                .HasIndex(x => x.RequestId)
                .IsUnique();
            modelBuilder.Entity<NotificationLog>()
                .Property(x => x.Provider) 
                .HasMaxLength(100)  
                .IsRequired();
            modelBuilder.Entity<Notification>()
                .HasIndex(x => new               
                {       
                    x.Status, 
                    x.Priority,
                    x.CreatedAt
                });
            modelBuilder.Entity<NotificationLog>()
                .Property(x => x.Response)
                .HasMaxLength(4000);
            modelBuilder.Entity<NotificationLog>()
                .HasIndex(x => x.NotificationId);
            modelBuilder.Entity<NotificationLog>()
                .HasIndex(x => x.Status); 
            modelBuilder.Entity<NotificationLog>()
                .Property(x => x.Status)
                .HasConversion<int>();

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
            modelBuilder.Entity<DeviceToken>()
                .Property(x => x.DeviceId)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<DeviceToken>()
                .HasIndex(x => x.DeviceId);

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
                .Property(x => x.Description) 
                .HasMaxLength(500);
            modelBuilder.Entity<EmailTemplate>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<EmailTemplate>()
                .HasIndex(x => x.IsActive);

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
                .Property(x => x.Type)
                .HasConversion<int>();
            modelBuilder.Entity<OtpCode>()
                .Property(x => x.AttemptCount)
                .HasDefaultValue(0);
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