// Seeders/AdminUserSeeder.cs
using CarShare.DAL.Enums.CarShare.DAL.Enums;
using CarShare.DAL.Enums;
using CarShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CarShare.DAL.Seeders
{
    public static class AdminUserSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Generate password hash
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash("Admin@1234", out passwordHash, out passwordSalt);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@carshare.com",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FirstName = "Admin",
                    LastName = "System",
                    PhoneNumber = "010000000000",
                    Role = UserRole.Admin, // Ensure this enum value exists
                    IsActive = true,
                    IsVerified = true,
                    AccountStatus = AccountStatus.Approved // If using approval system
                }
            );
        }

        private static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}