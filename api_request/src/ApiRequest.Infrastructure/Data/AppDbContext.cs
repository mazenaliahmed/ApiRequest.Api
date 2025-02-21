using ApiRequest.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiRequest.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestAssignment> RequestAssignments { get; set; }
        public DbSet<RequestDocument> RequestDocuments { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("tblUsers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UserName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.PasswordSalt).IsRequired();
                entity.Property(e => e.IdImage).HasColumnType("nvarchar(max)");
                entity.Property(e => e.TradeLicenseImage).HasColumnType("nvarchar(max)");
                entity.Property(e => e.AccountType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IdType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IdNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("tblRequests");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.RequestNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(255);
                entity.Property(e => e.RequestStatus).HasMaxLength(50).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RequestAssignment>(entity =>
            {
                entity.ToTable("tblRequestAssignments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ExecutionStatus).HasMaxLength(50).IsRequired();

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestAssignments)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RequestDocument>(entity =>
            {
                entity.ToTable("tblRequestDocuments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.DocumentName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.AccountType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.DocumentContent).IsRequired();

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestDocuments)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("tblUserTokens");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Token).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Purpose).HasMaxLength(100);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
