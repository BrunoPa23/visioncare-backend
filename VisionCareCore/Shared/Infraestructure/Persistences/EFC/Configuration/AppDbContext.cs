using System.Collections.Immutable;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.HealthCare.Domain.Model.Entities;
using VisionCareCore.HealthCare.Domain.Model.ValueObjects;
using VisionCareCore.User.Domain.Model.Aggregates;


namespace VisionCareCore.Shared.Infraestructure.Persistences.EFC.Configuration
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

     
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            builder.EnableSensitiveDataLogging();
            builder.AddCreatedUpdatedInterceptor();

            base.OnConfiguring(builder);
        }
        
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserRefreshToken> AuthUsersRefreshTokens { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineTime> MedicineTimes { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<AuthUser>(authUser =>
            {
                authUser.HasKey(u => u.Id);
                authUser.Property(u => u.Id).IsRequired();
                authUser.Property(u => u.Email).IsRequired().HasMaxLength(255);
                authUser.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_AuthUser_Email");
                authUser.Property(u => u.PasswordHash).IsRequired();
            });
            
            builder.Entity<AuthUserRefreshToken>(refreshToken =>
            {
                refreshToken.HasKey(rt => rt.Id);

                refreshToken.HasOne(rt => rt.AuthUser)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Medicine
            builder.Entity<Medicine>(medicine =>
            {
                medicine.HasKey(m => m.Id);
                medicine.Property(m => m.Nombre).IsRequired().HasMaxLength(100);
                medicine.Property(m => m.Description).HasMaxLength(500);
                medicine.Property(m => m.SideEffects).HasMaxLength(500);
                medicine.Property(m => m.Warnings).HasMaxLength(500);
                medicine.Property(m => m.IsDeleted).IsRequired();
                medicine.Property(m => m.Instruccions).HasMaxLength(500);
                medicine.Property(m => m.UserId).IsRequired();
                medicine.Property(m => m.ExpirationDate);
            });

            builder.Entity<MedicineTime>(mt =>
            {
                mt.HasKey(x => x.Id);

                mt.Property(x => x.Foods)
                    .HasConversion<int>()
                    .IsRequired(false);

                mt.Property(x => x.Interval)
                    .HasConversion<int>()
                    .IsRequired(false);

                mt.Property(x => x.SpecificTime)
                    .HasColumnType("time")
                    .IsRequired(false);

                mt.Property(x => x.Day)
                    .IsRequired()
                    .HasMaxLength(20);

                mt.Property(x => x.TypeRemember)
                    .HasConversion<string>() 
                    .IsRequired();

                mt.HasOne(x => x.Medicine)
                    .WithMany(m => m.MedicineTimes)
                    .HasForeignKey(x => x.MedicineId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }


    }
}
