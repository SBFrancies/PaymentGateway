using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PaymentGateway.Api.Data.Entities;
using System;

namespace PaymentGateway.Api.Data
{
    public class PaymentApiDbContext : DbContext
    {
        private const string Schema = "dbo";

        public DbSet<PaymentTable> Payments { get; set; }

        public PaymentApiDbContext(DbContextOptions<PaymentApiDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentTable>(a =>
            {
                a.ToTable("Payments", Schema);
                a.HasKey(a => a.Id);
                a.Property(a => a.Id).ValueGeneratedNever();
                a.Property(a => a.DateCreated).HasDefaultValueSql("GETUTCDATE()");
                a.Property(a => a.CurrencyCode).HasMaxLength(10).IsRequired();
                a.Property(a => a.CardNumber).HasMaxLength(30).IsRequired();
                a.Property(a => a.Reference).HasMaxLength(50);
                a.Property(a => a.CardName).HasMaxLength(100).IsRequired();
                a.Property(a => a.CreatedBy).HasMaxLength(200).IsRequired();
                a.Property(a => a.BankCode).HasMaxLength(30);
                a.Property(a => a.Amount).HasColumnType("DECIMAL(20,8)");
            });
        }

        public override int SaveChanges()
        {
            throw new NotImplementedException("Operation not supported, use SaveChangesAsync instead");
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotImplementedException("Operation not supported, use SaveChangesAsync instead");
        }
    }
}
