using WangNganHR.Shared.Entities;
using WangNganHR.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace WangNganHR.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var now = DateTime.UtcNow;
        db.Users.AddRange(
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                Username = "admin",
                Email = "admin@wangngan.local",
                PasswordHash = "admin123",
                Role = UserRole.Admin,
                FullName = "ผู้ดูแลระบบ",
                IsActive = true,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                Username = "hr",
                Email = "hr@wangngan.local",
                PasswordHash = "hr123",
                Role = UserRole.HR,
                FullName = "เจ้าหน้าที่ HR",
                IsActive = true,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                Username = "manager",
                Email = "manager@wangngan.local",
                PasswordHash = "manager123",
                Role = UserRole.Manager,
                FullName = "ผู้จัดการ",
                IsActive = true,
                CreatedAt = now
            });

        await db.SaveChangesAsync();
    }

    public static async Task RepairAsync(AppDbContext db)
    {
        var names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["admin"] = "ผู้ดูแลระบบ",
            ["hr"] = "เจ้าหน้าที่ HR",
            ["manager"] = "ผู้จัดการ"
        };

        var users = await db.Users
            .Where(u => names.Keys.Contains(u.Username))
            .ToListAsync();

        var changed = false;
        foreach (var user in users)
        {
            var expected = names[user.Username];
            if (user.FullName != expected)
            {
                user.FullName = expected;
                changed = true;
            }
        }

        if (changed)
            await db.SaveChangesAsync();
    }
}
