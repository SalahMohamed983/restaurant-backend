using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResturantDataAccessLayer.Context;
using ResturantDataAccessLayer.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ResturantDataAccessLayer.Seeding
{
    public class DataSeeder : IDataSeeder
    {
        private readonly ResturantDbContext _db;
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public DataSeeder(ResturantDbContext db, RoleManager<AspNetRole> roleManager, UserManager<User> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
    
        public async Task SeedAsync()
        {
            await _db.Database.MigrateAsync();

            // Roles
            var roles = new[] { "User", "Admin" };
            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new AspNetRole { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName.ToUpperInvariant() };
                    await _roleManager.CreateAsync(role);
                }
            }

            // Admin user
            var adminEmail = "admin@local";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator",
                    CreatedDate = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Other users
            var userEmails = new[] { "user1@local", "user2@local", "user3@local" };
            var createdUsers = new List<User> { adminUser };
            foreach (var email in userEmails)
            {
                var u = await _userManager.FindByEmailAsync(email);
                if (u == null)
                {
                    u = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FullName = email.Split('@')[0],
                        CreatedDate = DateTime.UtcNow
                    };
                    var r = await _userManager.CreateAsync(u, "User@1234");
                    if (r.Succeeded)
                        await _userManager.AddToRoleAsync(u, "User");
                }
                createdUsers.Add(u);
            }

            // Permissions and role-permissions
            if (!await _db.Permissions.AnyAsync())
            {
                var perms = new List<Permission>
                    {
                        new Permission { Code = "VIEW_ORDERS", Description = "View orders" },
                        new Permission { Code = "MANAGE_MENUS", Description = "Manage menu items and categories" },
                        new Permission { Code = "MANAGE_RESERVATIONS", Description = "Approve or reject reservations" }
                    };
                await _db.Permissions.AddRangeAsync(perms);
                await _db.SaveChangesAsync();

                var adminRole = await _roleManager.FindByNameAsync("Admin");
                var userRole = await _roleManager.FindByNameAsync("User");
                var savedPerms = await _db.Permissions.ToListAsync();

                var rolePerms = new List<RolePermission>();
                foreach (var p in savedPerms)
                    rolePerms.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id });

                var view = savedPerms.FirstOrDefault(p => p.Code == "VIEW_ORDERS");
                if (view != null && userRole != null)
                    rolePerms.Add(new RolePermission { RoleId = userRole.Id, PermissionId = view.Id });

                await _db.RolePermissions.AddRangeAsync(rolePerms);
                await _db.SaveChangesAsync();
            }

            // Menu categories and items
            if (!await _db.MenuCategories.AnyAsync())
            {
                var categories = new List<MenuCategory>
                    {
                        new MenuCategory { Id = Guid.NewGuid(), Name = "Starters", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id },
                        new MenuCategory { Id = Guid.NewGuid(), Name = "Mains", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id },
                        new MenuCategory { Id = Guid.NewGuid(), Name = "Desserts", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[2].Id }
                    };
                await _db.MenuCategories.AddRangeAsync(categories);
                await _db.SaveChangesAsync();

                var items = new List<MenuItem>
                    {
                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[0].Id, Name = "Fries", Price = 10.0m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id },
                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[0].Id, Name = "Salad", Price = 8.5m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id },
                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[0].Id, Name = "Bruschetta", Price = 12.0m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[2].Id },

                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[1].Id, Name = "Steak", Price = 25.0m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id },
                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[1].Id, Name = "Pasta", Price = 18.0m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[2].Id },
                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[1].Id, Name = "Grilled Fish", Price = 22.0m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id },

                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[2].Id, Name = "Ice Cream", Price = 6.0m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[2].Id },
                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[2].Id, Name = "Cheesecake", Price = 7.5m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[2].Id },
                        new MenuItem { Id = Guid.NewGuid(), CategoryId = categories[2].Id, Name = "Brownie", Price = 6.5m, IsAvailable = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id }
                    };

                await _db.MenuItems.AddRangeAsync(items);
                await _db.SaveChangesAsync();
            }

            // Dining tables
            if (!await _db.DiningTables.AnyAsync())
            {
                var tables = new List<DiningTable>
                    {
                        new DiningTable { Id = Guid.NewGuid(), TableNumber = 1, Capacity = 2, Location = "Window", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id },
                        new DiningTable { Id = Guid.NewGuid(), TableNumber = 2, Capacity = 4, Location = "Center", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[2].Id },
                        new DiningTable { Id = Guid.NewGuid(), TableNumber = 3, Capacity = 6, Location = "Patio", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = createdUsers[1].Id }
                    };
                await _db.DiningTables.AddRangeAsync(tables);
                await _db.SaveChangesAsync();
            }

            // Reservations and reservation status history
            if (!await _db.Reservations.AnyAsync())
            {
                var users = await _db.Users.Take(3).ToListAsync();
                var tablesList = await _db.DiningTables.Take(3).ToListAsync();
                var reservations = new List<Reservation>();
                for (int i = 0; i < 3; i++)
                {
                    var res = new Reservation
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = users[Math.Min(i, users.Count - 1)].Id,
                        TableId = tablesList[Math.Min(i, tablesList.Count - 1)].Id,
                        ReservationStart = DateTime.UtcNow.AddDays(i + 1).AddHours(18),
                        ReservationEnd = DateTime.UtcNow.AddDays(i + 1).AddHours(20),
                        GuestsCount = 2 + i,
                        Status = ReservationStatus.Pending,
                        RequestedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    };
                    reservations.Add(res);
                }
                await _db.Reservations.AddRangeAsync(reservations);
                await _db.SaveChangesAsync();

                var histories = new List<ReservationStatusHistory>();
                foreach (var r in reservations)
                {
                    histories.Add(new ReservationStatusHistory { Id = Guid.NewGuid(), ReservationId = r.Id, OldStatus = ReservationStatus.Pending, NewStatus = ReservationStatus.Approved, ChangedAt = DateTime.UtcNow, ChangedByUserId = createdUsers[0].Id });
                    histories.Add(new ReservationStatusHistory { Id = Guid.NewGuid(), ReservationId = r.Id, OldStatus = ReservationStatus.Approved, NewStatus = ReservationStatus.Approved, ChangedAt = DateTime.UtcNow.AddMinutes(1), ChangedByUserId = createdUsers[0].Id });
                }
                await _db.ReservationStatusHistory.AddRangeAsync(histories);
                await _db.SaveChangesAsync();
            }

            // Orders, order items and order status history
            if (!await _db.Orders.AnyAsync())
            {
                var customers = await _db.Users.Take(3).ToListAsync();
                var menuItems = await _db.MenuItems.Take(6).ToListAsync();

                var orders = new List<Order>();
                for (int i = 0; i < 3; i++)
                {
                    var ord = new Order
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customers[Math.Min(i, customers.Count - 1)].Id,
                        ReservationId = null,
                        OrderType = OrderType.DineIn,
                        Status = OrderStatus.Pending,
                        Subtotal = 0m,
                        Tax = 0m,
                        ServiceCharge = 0m,
                        Discount = 0m,
                        Total = 0m,
                        CreatedAt = DateTime.UtcNow
                    };
                    orders.Add(ord);
                }
                await _db.Orders.AddRangeAsync(orders);
                await _db.SaveChangesAsync();

                var orderItems = new List<OrderItem>();
                for (int i = 0; i < orders.Count; i++)
                {
                    var ord = orders[i];
                    var item1 = menuItems[(i * 2) % menuItems.Count];
                    var item2 = menuItems[(i * 2 + 1) % menuItems.Count];
                    var oi1 = new OrderItem { Id = Guid.NewGuid(), OrderId = ord.Id, MenuItemId = item1.Id, UnitPrice = item1.Price, Quantity = 1 + i, LineTotal = item1.Price * (1 + i) };
                    var oi2 = new OrderItem { Id = Guid.NewGuid(), OrderId = ord.Id, MenuItemId = item2.Id, UnitPrice = item2.Price, Quantity = 1, LineTotal = item2.Price };
                    orderItems.Add(oi1);
                    orderItems.Add(oi2);

                    ord.Subtotal = oi1.LineTotal + oi2.LineTotal;
                    ord.Tax = Math.Round(ord.Subtotal * 0.1m, 2);
                    ord.ServiceCharge = Math.Round(ord.Subtotal * 0.05m, 2);
                    ord.Total = ord.Subtotal + ord.Tax + ord.ServiceCharge - ord.Discount;
                }
                await _db.OrderItems.AddRangeAsync(orderItems);
                _db.Orders.UpdateRange(orders);
                await _db.SaveChangesAsync();

                var orderHistories = new List<OrderStatusHistory>();
                foreach (var o in orders)
                {
                    orderHistories.Add(new OrderStatusHistory { Id = Guid.NewGuid(), OrderId = o.Id, OldStatus = OrderStatus.Pending, NewStatus = OrderStatus.Confirmed, ChangedAt = DateTime.UtcNow, ChangedByUserId = createdUsers[0].Id });
                    orderHistories.Add(new OrderStatusHistory { Id = Guid.NewGuid(), OrderId = o.Id, OldStatus = OrderStatus.Confirmed, NewStatus = OrderStatus.Preparing, ChangedAt = DateTime.UtcNow.AddMinutes(2), ChangedByUserId = createdUsers[0].Id });
                }
                await _db.OrderStatusHistory.AddRangeAsync(orderHistories);
                await _db.SaveChangesAsync();
            }

            // Payments
            if (!await _db.Payments.AnyAsync())
            {
                var ordersList = await _db.Orders.Take(3).ToListAsync();
                var payments = new List<Payment>();
                for (int i = 0; i < ordersList.Count; i++)
                {
                    var o = ordersList[i];
                    payments.Add(new Payment { Id = Guid.NewGuid(), OrderId = o.Id, Amount = o.Total, Method = PaymentMethod.Card, Status = PaymentStatus.Paid, ProviderRef = $"PAY-{i + 1}", PaidAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedByUserId = o.CustomerId });
                }
                await _db.Payments.AddRangeAsync(payments);
                await _db.SaveChangesAsync();
            }

            // Refresh tokens
            if (!await _db.RefreshTokens.AnyAsync())
            {
                var usersAll = await _db.Users.Take(3).ToListAsync();
                var tokens = new List<RefreshToken>();
                for (int i = 0; i < usersAll.Count; i++)
                {
                    tokens.Add(new RefreshToken { Id = Guid.NewGuid(), UserId = usersAll[i].Id, TokenHash = $"hash{i}", JwtId = Guid.NewGuid().ToString(), ExpiresOn = DateTime.UtcNow.AddDays(30), CreatedOn = DateTime.UtcNow, CreatedByIp = "127.0.0.1", UserAgent = "Seeder" });
                }
                await _db.RefreshTokens.AddRangeAsync(tokens);
                await _db.SaveChangesAsync();
            }

            // Audit logs
            if (!await _db.AuditLogs.AnyAsync())
            {
                var audits = new List<AuditLog>
                    {
                        new AuditLog { Id = Guid.NewGuid(), ActorUserId = createdUsers[0].Id, Action = "Seed", EntityType = "MenuCategory", EntityId = null, NewValuesJson = "{}", CreatedAt = DateTime.UtcNow },
                        new AuditLog { Id = Guid.NewGuid(), ActorUserId = createdUsers[1].Id, Action = "Seed", EntityType = "MenuItem", EntityId = null, NewValuesJson = "{}", CreatedAt = DateTime.UtcNow },
                        new AuditLog { Id = Guid.NewGuid(), ActorUserId = createdUsers[2].Id, Action = "Seed", EntityType = "Order", EntityId = null, NewValuesJson = "{}", CreatedAt = DateTime.UtcNow }
                    };
                await _db.AuditLogs.AddRangeAsync(audits);
                await _db.SaveChangesAsync();
            }

            // Transaction logs
            if (!await _db.TransactionLogs.AnyAsync())
            {
                var txs = new List<TransactionLog>
                    {
                        new TransactionLog { Id = Guid.NewGuid(), TransactionType = "OrderPayment", ReferenceType = "Order", ReferenceId = null, Amount = 100m, PerformedByUserId = createdUsers[0].Id, OccurredAt = DateTime.UtcNow },
                        new TransactionLog { Id = Guid.NewGuid(), TransactionType = "Refund", ReferenceType = "Payment", ReferenceId = null, Amount = 20m, PerformedByUserId = createdUsers[1].Id, OccurredAt = DateTime.UtcNow },
                        new TransactionLog { Id = Guid.NewGuid(), TransactionType = "Topup", ReferenceType = "Wallet", ReferenceId = null, Amount = 50m, PerformedByUserId = createdUsers[2].Id, OccurredAt = DateTime.UtcNow }
                    };
                await _db.TransactionLogs.AddRangeAsync(txs);
                await _db.SaveChangesAsync();
            }

            // Analytics and snapshots
            if (!await _db.AnalyticsDailyKpis.AnyAsync())
            {
                var daily = new List<AnalyticsDailyKpi>
                    {
                        new AnalyticsDailyKpi { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)), OrdersCount = 5, CompletedOrdersCount = 4, Revenue = 200m, CreatedAt = DateTime.UtcNow },
                        new AnalyticsDailyKpi { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), OrdersCount = 8, CompletedOrdersCount = 7, Revenue = 350m, CreatedAt = DateTime.UtcNow },
                        new AnalyticsDailyKpi { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow), OrdersCount = 3, CompletedOrdersCount = 2, Revenue = 120m, CreatedAt = DateTime.UtcNow }
                    };
                await _db.AnalyticsDailyKpis.AddRangeAsync(daily);
                await _db.SaveChangesAsync();
            }

            if (!await _db.AnalyticsMonthlyKpis.AnyAsync())
            {
                var monthly = new List<AnalyticsMonthlyKpi>
                    {
                        new AnalyticsMonthlyKpi { Id = Guid.NewGuid(), Year = DateTime.UtcNow.Year, Month = DateTime.UtcNow.Month, OrdersCount = 100, CompletedOrdersCount = 95, Revenue = 5000m, CreatedAt = DateTime.UtcNow },
                        new AnalyticsMonthlyKpi { Id = Guid.NewGuid(), Year = DateTime.UtcNow.Year, Month = DateTime.UtcNow.AddMonths(-1).Month, OrdersCount = 120, CompletedOrdersCount = 115, Revenue = 6000m, CreatedAt = DateTime.UtcNow },
                        new AnalyticsMonthlyKpi { Id = Guid.NewGuid(), Year = DateTime.UtcNow.Year, Month = DateTime.UtcNow.AddMonths(-2).Month, OrdersCount = 90, CompletedOrdersCount = 85, Revenue = 4200m, CreatedAt = DateTime.UtcNow }
                    };
                await _db.AnalyticsMonthlyKpis.AddRangeAsync(monthly);
                await _db.SaveChangesAsync();
            }

            if (!await _db.AnalyticsTopItemsDaily.AnyAsync())
            {
                var items = await _db.MenuItems.Take(3).ToListAsync();
                var tops = new List<AnalyticsTopItemsDaily>();
                for (int i = 0; i < 3; i++)
                {
                    var menuItem = items[Math.Min(i, items.Count - 1)];
                    tops.Add(new AnalyticsTopItemsDaily { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-i)), MenuItemId = menuItem.Id, QuantitySold = 5 + i, Revenue = (menuItem.Price * (5 + i)) });
                }
                await _db.AnalyticsTopItemsDaily.AddRangeAsync(tops);
                await _db.SaveChangesAsync();
            }

            if (!await _db.AnalyticsCategoryRevenueDaily.AnyAsync())
            {
                var cats = await _db.MenuCategories.Take(3).ToListAsync();
                var catRev = new List<AnalyticsCategoryRevenueDaily>();
                for (int i = 0; i < 3; i++)
                {
                    catRev.Add(new AnalyticsCategoryRevenueDaily { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-i)), CategoryId = cats[Math.Min(i, cats.Count - 1)].Id, OrdersCount = 3 + i, Revenue = 100m + (i * 50) });
                }
                await _db.AnalyticsCategoryRevenueDaily.AddRangeAsync(catRev);
                await _db.SaveChangesAsync();
            }

            if (!await _db.AnalyticsReservationsByHourDaily.AnyAsync())
            {
                var byHour = new List<AnalyticsReservationsByHourDaily>
                    {
                        new AnalyticsReservationsByHourDaily { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow), Hour = 18, ReservationsCount = 3, ApprovedCount = 2, RejectedCount = 0, NoShowCount = 0 },
                        new AnalyticsReservationsByHourDaily { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow), Hour = 19, ReservationsCount = 5, ApprovedCount = 4, RejectedCount = 1, NoShowCount = 0 },
                        new AnalyticsReservationsByHourDaily { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow), Hour = 20, ReservationsCount = 2, ApprovedCount = 1, RejectedCount = 0, NoShowCount = 1 }
                    };
                await _db.AnalyticsReservationsByHourDaily.AddRangeAsync(byHour);
                await _db.SaveChangesAsync();
            }

            if (!await _db.AnalyticsTableUtilizationDaily.AnyAsync())
            {
                var tables = await _db.DiningTables.Take(3).ToListAsync();
                var util = new List<AnalyticsTableUtilizationDaily>();
                for (int i = 0; i < 3; i++)
                {
                    util.Add(new AnalyticsTableUtilizationDaily { Id = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-i)), TableId = tables[Math.Min(i, tables.Count - 1)].Id, ReservationsCount = 2 + i, ApprovedReservationsCount = 2 + i, MinutesReserved = 90 + (i * 15), UtilizationPercent = 50 + (i * 10) });
                }
                await _db.AnalyticsTableUtilizationDaily.AddRangeAsync(util);
                await _db.SaveChangesAsync();
            }

            if (!await _db.DashboardSnapshots.AnyAsync())
            {
                var snaps = new List<DashboardSnapshot>
                    {
                        new DashboardSnapshot { Id = Guid.NewGuid(), SnapshotKey = "home", FromDate = DateTime.UtcNow.AddDays(-7), ToDate = DateTime.UtcNow, DataJson = "{}", GeneratedAt = DateTime.UtcNow, GeneratedBy = "seeder" },
                        new DashboardSnapshot { Id = Guid.NewGuid(), SnapshotKey = "summary", FromDate = DateTime.UtcNow.AddMonths(-1), ToDate = DateTime.UtcNow, DataJson = "{}", GeneratedAt = DateTime.UtcNow, GeneratedBy = "seeder" },
                        new DashboardSnapshot { Id = Guid.NewGuid(), SnapshotKey = "monthly", FromDate = DateTime.UtcNow.AddMonths(-2), ToDate = DateTime.UtcNow.AddMonths(-1), DataJson = "{}", GeneratedAt = DateTime.UtcNow, GeneratedBy = "seeder" }
                    };
                await _db.DashboardSnapshots.AddRangeAsync(snaps);
                await _db.SaveChangesAsync();
            }

            if (!await _db.AnalyticsJobRuns.AnyAsync())
            {
                var jobs = new List<AnalyticsJobRun>
                    {
                        new AnalyticsJobRun { Id = Guid.NewGuid(), JobName = "DailyKpi", FromDate = DateTime.UtcNow.AddDays(-1), ToDate = DateTime.UtcNow, Status = AnalyticsJobStatus.Success, StartedAt = DateTime.UtcNow.AddMinutes(-5), FinishedAt = DateTime.UtcNow },
                        new AnalyticsJobRun { Id = Guid.NewGuid(), JobName = "MonthlyKpi", FromDate = DateTime.UtcNow.AddMonths(-1), ToDate = DateTime.UtcNow, Status = AnalyticsJobStatus.Success, StartedAt = DateTime.UtcNow.AddMinutes(-10), FinishedAt = DateTime.UtcNow.AddMinutes(-5) },
                        new AnalyticsJobRun { Id = Guid.NewGuid(), JobName = "TopItems", FromDate = DateTime.UtcNow.AddDays(-7), ToDate = DateTime.UtcNow, Status = AnalyticsJobStatus.Running, StartedAt = DateTime.UtcNow }
                    };
                await _db.AnalyticsJobRuns.AddRangeAsync(jobs);
                await _db.SaveChangesAsync();
            }

            // Ensure persisted
            await _db.SaveChangesAsync();
        }
    }
}
