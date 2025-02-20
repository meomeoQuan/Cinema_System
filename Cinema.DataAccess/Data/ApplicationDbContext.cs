using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    FullName = "Huỳnh Lê Đức Thọ",
                    Email = "tho@gmail.com",
                    PhoneNumber = "0935358701",
                    UserImage = "",
                    Role = "Admin",
                    PasswordHash = "AQAAAAEAACcQAAAAEJ9Z",
                    Points = 1375427
                }
             );
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    FullName = "Đố Biết Ai",
                    Email = "nonamefegbeu@gmail.com",
                    PhoneNumber = "0323688701",
                    UserImage = "",
                    Role = "Admin",
                    PasswordHash = "AQAVVKDHJBIGRBHG",
                    Points = 7
                }
             );

            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    MovieID = 1,
                    Title = "Inception",
                    Genre = "Sci-Fi",
                    AgeLimit = "10+",
                    Synopsis = "A thief who enters the dreams of others to steal secrets.",
                    TrailerLink = "https://example.com/inception",
                    Duration = 148,
                    ReleaseDate = new DateTime(2010, 7, 16),
                    MovieImage = "",
                    IsUpcomingMovie = false
                },
                new Movie
                {
                    MovieID = 2,
                    Title = "The Dark Knight",
                    Genre = "Action",
                    AgeLimit = "18+",
                    Synopsis = "Batman faces the Joker, a criminal mastermind.",
                    TrailerLink = "https://example.com/darkknight",
                    Duration = 152,
                    ReleaseDate = new DateTime(2008, 7, 18),
                    MovieImage = "",
                    IsUpcomingMovie = false
                }
            );
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    MovieID = 3,
                    Title = "Inception",
                    Genre = "Sci-Fi",
                    AgeLimit = "10+",
                    Synopsis = "A thief who enters the dreams of others to steal secrets.",
                    TrailerLink = "https://example.com/inception",
                    Duration = 148,
                    ReleaseDate = new DateTime(2010, 7, 16),
                    MovieImage = "",
                    IsUpcomingMovie = true
                },
                new Movie
                {
                    MovieID = 4,
                    Title = "The Dark Knight",
                    AgeLimit = "18+",
                    Genre = "Action",
                    Synopsis = "Batman faces the Joker, a criminal mastermind.",
                    TrailerLink = "https://example.com/darkknight",
                    Duration = 152,
                    ReleaseDate = new DateTime(2008, 7, 18),
                    MovieImage = "",
                    IsUpcomingMovie = true
                }
            );
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    CouponID = 1,
                    Code = "TEST10",
                    DiscountPercentage = 10.00m, // Changed from string to decimal
                    UsageLimit = 10,
                    UsedCount = 1,
                    ExpireDate = new DateTime(2025, 12, 31)
                }
             );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductID = 1,
                    Name = "Popcorn",
                    Description = "A large bucket of buttered popcorn.",
                    ProductType = ProductType.Snack,
                    Price = 5.99m,
                    Quantity = 50,
                    ProductImage = ""
                },
                new Product
                {
                    ProductID = 2,
                    Name = "Soda",
                    Description = "Refreshing cold soda, 500ml.",
                    ProductType = ProductType.Drink,
                    Price = 2.99m,
                    Quantity = 100,
                    ProductImage = ""
                }
            );
        }


    }
}
