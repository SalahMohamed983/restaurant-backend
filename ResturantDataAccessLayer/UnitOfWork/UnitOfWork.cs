using ResturantDataAccessLayer.Context;
using ResturantDataAccessLayer.Entities;
using ResturantDataAccessLayer.Interfaces;
using ResturantDataAccessLayer.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace ResturantDataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ResturantDbContext _db;

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<AspNetRole> Roles { get; }
        public IGenericRepository<Permission> Permissions { get; }
        public IGenericRepository<RolePermission> RolePermissions { get; }

        public IGenericRepository<MenuCategory> MenuCategories { get; }
        public IGenericRepository<MenuItem> MenuItems { get; }

        public IGenericRepository<DiningTable> DiningTables { get; }

        public IGenericRepository<Reservation> Reservations { get; }
        public IGenericRepository<ReservationStatusHistory> ReservationStatusHistories { get; }

        public IGenericRepository<Order> Orders { get; }
        public IGenericRepository<OrderItem> OrderItems { get; }
        public IGenericRepository<OrderStatusHistory> OrderStatusHistories { get; }

        public IGenericRepository<Payment> Payments { get; }

        public IGenericRepository<RefreshToken> RefreshTokens { get; }

        public IGenericRepository<TransactionLog> TransactionLogs { get; }
        public IGenericRepository<AuditLog> AuditLogs { get; }
        public IGenericRepository<DashboardSnapshot> DashboardSnapshots { get; }

        public IGenericRepository<AnalyticsJobRun> AnalyticsJobRuns { get; }
        public IGenericRepository<AnalyticsDailyKpi> AnalyticsDailyKpis { get; }
        public IGenericRepository<AnalyticsMonthlyKpi> AnalyticsMonthlyKpis { get; }
        public IGenericRepository<AnalyticsTopItemsDaily> AnalyticsTopItemsDaily { get; }
        public IGenericRepository<AnalyticsCategoryRevenueDaily> AnalyticsCategoryRevenueDaily { get; }
        public IGenericRepository<AnalyticsReservationsByHourDaily> AnalyticsReservationsByHourDaily { get; }
        public IGenericRepository<AnalyticsTableUtilizationDaily> AnalyticsTableUtilizationDaily { get; }

        public UnitOfWork(ResturantDbContext db)
        {
            _db = db;
            Users = new GenericRepository<User>(_db);
            Roles = new GenericRepository<AspNetRole>(_db);
            Permissions = new GenericRepository<Permission>(_db);
            RolePermissions = new GenericRepository<RolePermission>(_db);

            MenuCategories = new GenericRepository<MenuCategory>(_db);
            MenuItems = new GenericRepository<MenuItem>(_db);

            DiningTables = new GenericRepository<DiningTable>(_db);

            Reservations = new GenericRepository<Reservation>(_db);
            ReservationStatusHistories = new GenericRepository<ReservationStatusHistory>(_db);

            Orders = new GenericRepository<Order>(_db);
            OrderItems = new GenericRepository<OrderItem>(_db);
            OrderStatusHistories = new GenericRepository<OrderStatusHistory>(_db);

            Payments = new GenericRepository<Payment>(_db);

            RefreshTokens = new GenericRepository<RefreshToken>(_db);

            TransactionLogs = new GenericRepository<TransactionLog>(_db);
            AuditLogs = new GenericRepository<AuditLog>(_db);
            DashboardSnapshots = new GenericRepository<DashboardSnapshot>(_db);

            AnalyticsJobRuns = new GenericRepository<AnalyticsJobRun>(_db);
            AnalyticsDailyKpis = new GenericRepository<AnalyticsDailyKpi>(_db);
            AnalyticsMonthlyKpis = new GenericRepository<AnalyticsMonthlyKpi>(_db);
            AnalyticsTopItemsDaily = new GenericRepository<AnalyticsTopItemsDaily>(_db);
            AnalyticsCategoryRevenueDaily = new GenericRepository<AnalyticsCategoryRevenueDaily>(_db);
            AnalyticsReservationsByHourDaily = new GenericRepository<AnalyticsReservationsByHourDaily>(_db);
            AnalyticsTableUtilizationDaily = new GenericRepository<AnalyticsTableUtilizationDaily>(_db);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
