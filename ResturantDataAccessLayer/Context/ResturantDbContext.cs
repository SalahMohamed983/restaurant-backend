using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ResturantDataAccessLayer.Entities;
using System;

namespace ResturantDataAccessLayer.Context
{
    public class ResturantDbContext : IdentityDbContext<User, AspNetRole, Guid>
    {
        public ResturantDbContext(DbContextOptions<ResturantDbContext> options) : base(options) { }

        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public DbSet<MenuCategory> MenuCategories { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;

        public DbSet<DiningTable> DiningTables { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<ReservationStatusHistory> ReservationStatusHistory { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; } = null!;

        public DbSet<Payment> Payments { get; set; } = null!;

        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<TransactionLog> TransactionLogs { get; set; } = null!;

        public DbSet<AnalyticsDailyKpi> AnalyticsDailyKpis { get; set; } = null!;
        public DbSet<AnalyticsMonthlyKpi> AnalyticsMonthlyKpis { get; set; } = null!;
        public DbSet<AnalyticsTopItemsDaily> AnalyticsTopItemsDaily { get; set; } = null!;
        public DbSet<AnalyticsCategoryRevenueDaily> AnalyticsCategoryRevenueDaily { get; set; } = null!;
        public DbSet<AnalyticsReservationsByHourDaily> AnalyticsReservationsByHourDaily { get; set; } = null!;
        public DbSet<AnalyticsTableUtilizationDaily> AnalyticsTableUtilizationDaily { get; set; } = null!;
        public DbSet<DashboardSnapshot> DashboardSnapshots { get; set; } = null!;
        public DbSet<AnalyticsJobRun> AnalyticsJobRuns { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all IEntityTypeConfiguration<T> implementations from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResturantDbContext).Assembly);
        }
    }
}
