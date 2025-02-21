using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        //public DbSet<ApplicationUser> Users { get; set; } không cần vì identity của asp đã có sẵn
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //var hasher = new PasswordHasher<ApplicationUser>();
            //modelBuilder.Entity<ApplicationUser>().HasData(
            //    new ApplicationUser
            //    {
            //        FullName = "Huỳnh Lê Đức Thọ",
            //        Email = "tho@gmail.com",
            //        PhoneNumber = "0935358701",
            //        UserImage = "",
            //        Role = "Admin",
            //        PasswordHash = hasher.HashPassword(null, "123"),
            //        Points = 1375427
            //    },
            //    new ApplicationUser
            //    {
            //        FullName = "Đố Biết Ai",
            //        Email = "abc@gmail.com",
            //        PhoneNumber = "0323688701",
            //        UserImage = "",
            //        Role = "Admin",
            //        PasswordHash = hasher.HashPassword(null, "123456"),
            //        Points = 7
            //    },
            //    new ApplicationUser
            //    {
            //        Id = "3",
            //        FullName = "Lê Văn C",
            //        Email = "levanc@gmail.com",
            //        UserName = "levanc",
            //        PhoneNumber = "0927345678",
            //        UserImage = "https://randomuser.me/api/portraits/men/3.jpg",
            //        Role = "Member",
            //        PasswordHash = hasher.HashPassword(null, "123456"),
            //        Points = 1500
            //    },
            //    new ApplicationUser
            //    {
            //        Id = "4",
            //        FullName = "Hoàng Thị D",
            //        Email = "hoangthid@gmail.com",
            //        UserName = "hoangthid",
            //        PhoneNumber = "0938456789",
            //        UserImage = "https://randomuser.me/api/portraits/women/4.jpg",
            //        Role = "Member",
            //        PasswordHash = hasher.HashPassword(null, "123456"),
            //        Points = 3000
            //    },
            //    new ApplicationUser
            //    {
            //        Id = "5",
            //        FullName = "Phạm Minh E",
            //        Email = "phammine@gmail.com",
            //        UserName = "phammine",
            //        PhoneNumber = "0949567890",
            //        UserImage = "https://randomuser.me/api/portraits/men/5.jpg",
            //        Role = "Staff",
            //        PasswordHash = hasher.HashPassword(null, "123456"),
            //        Points = 1000
            //    }
            // );

            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    MovieID = 1,
                    Title = "Inception",
                    Genre = "Sci-Fi",
                    AgeLimit = "13+",
                    Synopsis = "A thief who enters the dreams of others to steal secrets.",
                    TrailerLink = "https://www.youtube.com/watch?v=YoHD9XEInc0",
                    Duration = 148,
                    //ReleaseDate = new DateTime(2010, 7, 16),
                    MovieImage = "https://m.media-amazon.com/images/I/51oBxmV-dML._AC_.jpg",
                    IsUpcomingMovie = false
                },
                new Movie
                {
                    MovieID = 2,
                    Title = "The Dark Knight",
                    Genre = "Action",
                    AgeLimit = "16+",
                    Synopsis = "Batman faces the Joker, a criminal mastermind who brings chaos to Gotham.",
                    TrailerLink = "https://www.youtube.com/watch?v=EXeTwQWrcwY",
                    Duration = 152,
                    //ReleaseDate = new DateTime(2008, 7, 18),
                    MovieImage = "https://m.media-amazon.com/images/I/81AJdOIEI2L._AC_SL1500_.jpg",
                    IsUpcomingMovie = false
                },
                new Movie
                {
                    MovieID = 3,
                    Title = "Interstellar",
                    Genre = "Sci-Fi",
                    AgeLimit = "10+",
                    Synopsis = "A team of explorers travel through a wormhole in space in an attempt to save humanity.",
                    TrailerLink = "https://www.youtube.com/watch?v=zSWdZVtXT7E",
                    Duration = 169,
                    //ReleaseDate = new DateTime(2014, 11, 7),
                    MovieImage = "https://m.media-amazon.com/images/I/91kFYg4fX3L._AC_SL1500_.jpg",
                    IsUpcomingMovie = false
                },
                new Movie
                {
                    MovieID = 4,
                    Title = "Avatar: The Way of Water",
                    Genre = "Adventure",
                    AgeLimit = "12+",
                    Synopsis = "Jake Sully and Neytiri must protect their family from an old enemy on Pandora.",
                    TrailerLink = "https://www.youtube.com/watch?v=d9MyW72ELq0",
                    Duration = 192,
                    //ReleaseDate = new DateTime(2022, 12, 16),
                    MovieImage = "https://m.media-amazon.com/images/I/81+V9U8UcPL._AC_SL1500_.jpg",
                    IsUpcomingMovie = false
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
                    //ExpireDate = new DateTime(2025, 12, 31)
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
