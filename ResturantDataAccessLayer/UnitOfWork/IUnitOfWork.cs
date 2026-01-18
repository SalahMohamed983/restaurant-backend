using System;
using System.Threading.Tasks;
using ResturantDataAccessLayer.Entities;
using ResturantDataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ResturantDataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<AspNetRole> Roles { get; }
        IGenericRepository<Permission> Permissions { get; }
        IGenericRepository<RolePermission> RolePermissions { get; }

        IGenericRepository<MenuCategory> MenuCategories { get; }
        IGenericRepository<MenuItem> MenuItems { get; }

        IGenericRepository<DiningTable> DiningTables { get; }

        IGenericRepository<Reservation> Reservations { get; }
        IGenericRepository<ReservationStatusHistory> ReservationStatusHistories { get; }

        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<OrderStatusHistory> OrderStatusHistories { get; }

        IGenericRepository<Payment> Payments { get; }

        IGenericRepository<RefreshToken> RefreshTokens { get; }

        IGenericRepository<TransactionLog> TransactionLogs { get; }
        IGenericRepository<AuditLog> AuditLogs { get; }
        IGenericRepository<DashboardSnapshot> DashboardSnapshots { get; }

        IGenericRepository<AnalyticsJobRun> AnalyticsJobRuns { get; }
        IGenericRepository<AnalyticsDailyKpi> AnalyticsDailyKpis { get; }
        IGenericRepository<AnalyticsMonthlyKpi> AnalyticsMonthlyKpis { get; }
        IGenericRepository<AnalyticsTopItemsDaily> AnalyticsTopItemsDaily { get; }
        IGenericRepository<AnalyticsCategoryRevenueDaily> AnalyticsCategoryRevenueDaily { get; }
        IGenericRepository<AnalyticsReservationsByHourDaily> AnalyticsReservationsByHourDaily { get; }
        IGenericRepository<AnalyticsTableUtilizationDaily> AnalyticsTableUtilizationDaily { get; }

        Task<int> SaveChangesAsync();

        // Begin a database transaction (EF Core)
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
