using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResturantDataAccessLayer.Entities;

namespace ResturantDataAccessLayer.Configurations
{
    internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.HasOne(rp => rp.Role)
                   .WithMany(r => r.RolePermissions)
                   .HasForeignKey(rp => rp.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Permission)
                   .WithMany()
                   .HasForeignKey(rp => rp.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
    {
        public void Configure(EntityTypeBuilder<MenuCategory> builder)
        {
            builder.HasOne(mc => mc.Creator)
                .WithMany(u => u.CreatedMenuCategories)
                .HasForeignKey(mc => mc.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mc => mc.Updater)
                .WithMany()
                .HasForeignKey(mc => mc.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.HasOne(mi => mi.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(mi => mi.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mi => mi.Creator)
                .WithMany(u => u.CreatedMenuItems)
                .HasForeignKey(mi => mi.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mi => mi.Updater)
                .WithMany()
                .HasForeignKey(mi => mi.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(mi => mi.Price).HasPrecision(18, 2);
        }
    }

    internal class DiningTableConfiguration : IEntityTypeConfiguration<DiningTable>
    {
        public void Configure(EntityTypeBuilder<DiningTable> builder)
        {
            builder.HasIndex(dt => dt.TableNumber).IsUnique();

            builder.HasOne(dt => dt.Creator)
                .WithMany(u => u.CreatedDiningTables)
                .HasForeignKey(dt => dt.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dt => dt.Updater)
                .WithMany()
                .HasForeignKey(dt => dt.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Table)
                .WithMany()
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class ReservationStatusHistoryConfiguration : IEntityTypeConfiguration<ReservationStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ReservationStatusHistory> builder)
        {
            builder.HasOne(h => h.Reservation)
                .WithMany()
                .HasForeignKey(h => h.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Reservation)
                .WithMany()
                .HasForeignKey(o => o.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(o => o.Subtotal).HasPrecision(18, 2);
            builder.Property(o => o.Tax).HasPrecision(18, 2);
            builder.Property(o => o.ServiceCharge).HasPrecision(18, 2);
            builder.Property(o => o.Discount).HasPrecision(18, 2);
            builder.Property(o => o.Total).HasPrecision(18, 2);
        }
    }

    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(oi => oi.MenuItem)
                .WithMany()
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            builder.Property(oi => oi.LineTotal).HasPrecision(18, 2);
        }
    }

    internal class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.HasOne(h => h.Order)
                .WithMany()
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Amount).HasPrecision(18, 2);
        }
    }

    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class TransactionLogConfiguration : IEntityTypeConfiguration<TransactionLog>
    {
        public void Configure(EntityTypeBuilder<TransactionLog> builder)
        {
            builder.HasOne(t => t.PerformedBy)
                .WithMany()
                .HasForeignKey(t => t.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.Amount).HasPrecision(18, 2);
        }
    }

    internal class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasOne(a => a.ActorUser)
                .WithMany()
                .HasForeignKey(a => a.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class AnalyticsDailyKpiConfiguration : IEntityTypeConfiguration<AnalyticsDailyKpi>
    {
        public void Configure(EntityTypeBuilder<AnalyticsDailyKpi> builder)
        {
            builder.HasIndex(a => a.Date).IsUnique();

            builder.Property(a => a.Revenue).HasPrecision(18, 2);
            builder.Property(a => a.TaxTotal).HasPrecision(18, 2);
            builder.Property(a => a.DiscountTotal).HasPrecision(18, 2);
        }
    }

    internal class AnalyticsMonthlyKpiConfiguration : IEntityTypeConfiguration<AnalyticsMonthlyKpi>
    {
        public void Configure(EntityTypeBuilder<AnalyticsMonthlyKpi> builder)
        {
            builder.HasIndex(a => new { a.Year, a.Month }).IsUnique();

            builder.Property(a => a.Revenue).HasPrecision(18, 2);
        }
    }

    internal class AnalyticsTopItemsDailyConfiguration : IEntityTypeConfiguration<AnalyticsTopItemsDaily>
    {
        public void Configure(EntityTypeBuilder<AnalyticsTopItemsDaily> builder)
        {
            builder.HasIndex(a => new { a.Date, a.MenuItemId }).IsUnique();

            builder.Property(a => a.Revenue).HasPrecision(18, 2);
        }
    }

    internal class AnalyticsCategoryRevenueDailyConfiguration : IEntityTypeConfiguration<AnalyticsCategoryRevenueDaily>
    {
        public void Configure(EntityTypeBuilder<AnalyticsCategoryRevenueDaily> builder)
        {
            builder.HasIndex(a => new { a.Date, a.CategoryId }).IsUnique();

            builder.Property(a => a.Revenue).HasPrecision(18, 2);
        }
    }

    internal class AnalyticsReservationsByHourDailyConfiguration : IEntityTypeConfiguration<AnalyticsReservationsByHourDaily>
    {
        public void Configure(EntityTypeBuilder<AnalyticsReservationsByHourDaily> builder)
        {
            builder.HasIndex(a => new { a.Date, a.Hour }).IsUnique();
        }
    }

    internal class AnalyticsTableUtilizationDailyConfiguration : IEntityTypeConfiguration<AnalyticsTableUtilizationDaily>
    {
        public void Configure(EntityTypeBuilder<AnalyticsTableUtilizationDaily> builder)
        {
            builder.HasIndex(a => new { a.Date, a.TableId }).IsUnique();

            builder.Property(a => a.UtilizationPercent).HasPrecision(18, 2);
        }
    }
}
