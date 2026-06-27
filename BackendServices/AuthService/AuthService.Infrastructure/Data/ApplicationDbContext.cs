using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RefreshToken>()
                .Property(x => x.Token)
                .IsRequired();
            builder.Entity<RefreshToken>()
                .HasIndex(x => x.Token)
                .IsUnique();
            builder.Entity<RefreshToken>()
                .HasIndex(x => x.UserId);
            builder.Entity<UserProfile>()
                .HasKey(x => x.UserId);
            builder.Entity<UserProfile>()
                .HasOne(x => x.User)
                .WithOne(x => x.UserProfile)
                .HasForeignKey<UserProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Address>()
                .HasOne(x => x.User)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Address>()
                .HasIndex(x => x.UserId);
            builder.Entity<RefreshToken>()
                .HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Address>()
                .Property(x => x.ContactPhoneNumber)
                .HasMaxLength(20);
            builder.Entity<Address>()
                .Property(x => x.AddressLine1)
                .HasMaxLength(250)
                .IsRequired();
            builder.Entity<Address>()
                .Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();
            builder.Entity<Address>()
                .Property(x => x.State)
                .HasMaxLength(100)
                .IsRequired();
            builder.Entity<Address>()
                .Property(x => x.PostalCode)
                .HasMaxLength(20)
                .IsRequired();
            builder.Entity<Address>()
                .Property(x => x.Country)
                .HasMaxLength(100)
                .IsRequired();
            builder.Entity<UserProfile>()
                .Property(x => x.FullName)
                .HasMaxLength(150) 
                .IsRequired();
            builder.Entity<Address>()
                .HasIndex(x => new { x.UserId, x.IsDefault });
            builder.Entity<UserProfile>()
                .Property(x => x.PrimaryPhoneNumber)
                .HasMaxLength(20)
                .IsRequired();
            builder.Entity<Address>()
                .Property(x => x.AddressType)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
