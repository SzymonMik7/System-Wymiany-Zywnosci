using System_wymiany_żywności.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace System_wymiany_żywności.Data
{
    // Główna klasa kontekstu bazy danych łączy nasz kod C# z silnikiem SQL Server
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Definicja tabel, które zostaną utworzone w bazie danych
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Metoda konfigurująca model bazy danych i relacje między tabelami
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Konfiguracja relacji
            builder.Entity<Reservation>(entity =>
            {
                // Informujemy EF Core że ta tabela ma Trigger SQL
                entity.ToTable(tb => tb.HasTrigger("trg_LogNewReservation"));

                // Relacja: Rezerwacja dotyczy jednej Oferty
                entity.HasOne(r => r.Offer)
                    .WithMany(o => o.Reservations)
                    .HasForeignKey(r => r.OfferId)
                    .OnDelete(DeleteBehavior.Restrict); 
                // Nie usuwaj oferty, jeśli są rezerwacje

                // Relacja: Rezerwacja jest składana przez Kupującego
                entity.HasOne(r => r.Buyer)
                    .WithMany()
                    .HasForeignKey(r => r.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict); 
                // Nie usuwaj rezerwacji przy usuwaniu usera (historia)
            });

            //Dane startowe do bazy

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Warzywa" },
                new Category { Id = 2, Name = "Owoce" },
                new Category { Id = 3, Name = "Pieczywo" },
                new Category { Id = 4, Name = "Nabiał" }
            );

            var adminRoleId = "admin-role-id";
            var userRoleId = "user-role-id";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" }
            );

            // Tworzenie domyślnego konta Administratora
            var adminId = "admin-user-id";
            var adminUser = new ApplicationUser
            {
                Id = adminId,
                UserName = "admin@food.com",
                NormalizedUserName = "ADMIN@FOOD.COM",
                Email = "admin@food.com",
                NormalizedEmail = "ADMIN@FOOD.COM",
                EmailConfirmed = true,
                WalletBalance = 9999,
                TrustScore = 100,
                SecurityStamp = "f4c2c54c-53f4-477b-8321-72995f745199"
            };

            // Haszowanie hasła administratora
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Admin123!");

            builder.Entity<ApplicationUser>().HasData(adminUser);

            // Przypisanie roli Admina do stworzonego użytkownika
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminId }
            );
        }
    }
}