﻿using System;
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

        public DbSet<Theater> Cinemas { get; set; }
        public DbSet<ShowTime> showTimes { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Room → ShowTime (Disable Cascade Delete)
            modelBuilder.Entity<ShowTime>()
                .HasOne(s => s.Room)
                .WithMany(r => r.ShowTimes)
                .HasForeignKey(s => s.RoomID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion

            // Movie → ShowTime (Disable Cascade Delete)
            modelBuilder.Entity<ShowTime>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.ShowTimes)
                .HasForeignKey(s => s.MovieID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion

            // Cinema → Room (Disable Cascade Delete)
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Cinema)
                .WithMany(c => c.Rooms)
                .HasForeignKey(r => r.CinemaID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion

            // Configure Theater Table
            modelBuilder.Entity<Theater>()
                .Property(t => t.Status)
                .HasConversion<string>(); // Store enum as string

            modelBuilder.Entity<Movie>().HasData(
                // Showing Movies (Existing + 5 New)
                new Movie
                {
                    MovieID = 1,
                    Title = "Inception",
                    Genre = "Sci-Fi",
                    AgeLimit = "13+",
                    Synopsis = "A thief who enters the dreams of others to steal secrets.",
                    TrailerLink = "https://www.youtube.com/watch?v=YoHD9XEInc0",
                    Duration = 148,
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
                    MovieImage = "https://m.media-amazon.com/images/I/A1exRxgHRRL.jpg",
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
                    MovieImage = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTfTiEWQYqVZHQuVHy6G9PQIUfa5ujUpy0e7fZ-t6TwN19glQiAuhNS3PkWt-v48Lr9pIE&usqp=CAU",
                    IsUpcomingMovie = false
                },
                new Movie { MovieID = 5, Title = "Dune", Genre = "Sci-Fi", AgeLimit = "13+", Synopsis = "A noble family's son leads a rebellion on a desert planet.", TrailerLink = "https://www.youtube.com/watch?v=n9xhJrPXop4", Duration = 155, MovieImage = "https://m.media-amazon.com/images/I/81MUHYLUf6L._AC_UF894,1000_QL80_.jpg", IsUpcomingMovie = false },
                new Movie { MovieID = 6, Title = "John Wick 4", Genre = "Action", AgeLimit = "18+", Synopsis = "John Wick takes on the High Table in his most dangerous fight.", TrailerLink = "https://www.youtube.com/watch?v=qEVUtrk8_B4", Duration = 169, MovieImage = "https://m.media-amazon.com/images/I/71tIm0Xxr2L._AC_UF894,1000_QL80_.jpg", IsUpcomingMovie = false },
                new Movie { MovieID = 7, Title = "Oppenheimer", Genre = "Biography", AgeLimit = "16+", Synopsis = "The story of J. Robert Oppenheimer and the atomic bomb.", TrailerLink = "https://www.youtube.com/watch?v=bK6ldnjE3Y0", Duration = 180, MovieImage = "https://m.media-amazon.com/images/I/71qu4p5bnDL._AC_UF894,1000_QL80_.jpg", IsUpcomingMovie = false },
                new Movie { MovieID = 8, Title = "Spider-Man: No Way Home", Genre = "Superhero", AgeLimit = "13+", Synopsis = "Spider-Man fights villains from multiple universes.", TrailerLink = "https://www.youtube.com/watch?v=JfVOs4VSpmA", Duration = 148, MovieImage = "https://m.media-amazon.com/images/I/71niXI3lxlL._AC_SL1500_.jpg", IsUpcomingMovie = false },
                new Movie { MovieID = 9, Title = "The Matrix Resurrections", Genre = "Sci-Fi", AgeLimit = "16+", Synopsis = "Neo returns to the Matrix for a new journey.", TrailerLink = "https://www.youtube.com/watch?v=9ix7TUGVYIo", Duration = 148, MovieImage = "https://m.media-amazon.com/images/I/71PQje4I99L.jpg", IsUpcomingMovie = false },

                // Upcoming Movies (5 New)
                new Movie { MovieID = 10, Title = "Deadpool 3", Genre = "Action/Comedy", AgeLimit = "18+", Synopsis = "Deadpool returns with more fourth-wall-breaking humor.", TrailerLink = "", Duration = 120, MovieImage = "https://m.media-amazon.com/images/I/71wNKMs+CvL._AC_UF894,1000_QL80_.jpg", IsUpcomingMovie = true },
                new Movie { MovieID = 11, Title = "The Batman 2", Genre = "Action", AgeLimit = "16+", Synopsis = "The Dark Knight faces a new enemy in Gotham.", TrailerLink = "", Duration = 150, MovieImage = "https://m.media-amazon.com/images/I/61NCZ4VQ8EL._AC_UF894,1000_QL80_.jpg", IsUpcomingMovie = true },
                new Movie { MovieID = 12, Title = "Avatar 3", Genre = "Adventure", AgeLimit = "12+", Synopsis = "The Na'vi continue their fight against human invaders.", TrailerLink = "", Duration = 180, MovieImage = "https://m.media-amazon.com/images/I/61SNSxk3RNL._AC_UF894,1000_QL80_.jpg", IsUpcomingMovie = true },
                new Movie { MovieID = 13, Title = "Fantastic Four", Genre = "Superhero", AgeLimit = "13+", Synopsis = "Marvel's First Family joins the MCU.", TrailerLink = "", Duration = 140, MovieImage = "https://m.media-amazon.com/images/I/81TBhA6kgBL.jpg", IsUpcomingMovie = true },
                new Movie { MovieID = 14, Title = "Shrek 5", Genre = "Animation/Comedy", AgeLimit = "All Ages", Synopsis = "Shrek and friends return for a new adventure.", TrailerLink = "", Duration = 100, MovieImage = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTJgULr6iPFNLydknD-UKqWcCsfyUZVmJrjuw&s", IsUpcomingMovie = true }
            );

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    CouponID = 1,
                    Code = "TEST10",
                    CouponImage = "",
                    DiscountPercentage = 0.1, // Changed from string to decimal
                    UsageLimit = 10,
                    UsedCount = 1
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductID = 1,
                    Name = "Popcorn",
                    Description = "A large bucket of buttered popcorn.",
                    ProductType = ProductType.Snack,
                    Price = 5.99,
                    Quantity = 50,
                    ProductImage = ""
                },
                new Product
                {
                    ProductID = 2,
                    Name = "Soda",
                    Description = "Refreshing cold soda, 500ml.",
                    ProductType = ProductType.Drink,
                    Price = 2.99,
                    Quantity = 100,
                    ProductImage = ""
                }
            );
            // Seed Sample Theaters
            modelBuilder.Entity<Theater>().HasData(
                new Theater
                {
                    CinemaID = 1,
                    Name = "Grand Cinema",
                    Address = "123 Main St, City",
                    NumberOfRooms = 5,
                    Status = CinemaStatus.Open,
                    OpeningTime = "09:00",  // Changed from TimeSpan to string
                    ClosingTime = "23:00",  // Changed from TimeSpan to string
               
                 
                },
                new Theater
                {
                    CinemaID = 2,
                    Name = "Skyline Theater",
                    Address = "456 Broadway Ave, City",
                    NumberOfRooms = 7,
                    Status = CinemaStatus.Open,
                    OpeningTime = "10:00",  // Changed from TimeSpan to string
                    ClosingTime = "22:30",  // Changed from TimeSpan to string
                  
                   
                }
            );
            modelBuilder.Entity<Room>().HasData(
    new Room
    {
        RoomID = 1,
        RoomNumber = "A1",
        Capacity = 100,
        Status = RoomStatus.Available,
        CinemaID = 1 // Matches existing Theater
    },
    new Room
    {
        RoomID = 2,
        RoomNumber = "B1",
        Capacity = 150,
        Status = RoomStatus.Available,
        CinemaID = 2 // Matches existing Theater
    }
);


            // Seed Sample ShowTimes
            modelBuilder.Entity<ShowTime>().HasData(
                new ShowTime
                {
                    ShowTimeID = 1,
                    ShowDates = "01/03/2025", // Ensure a valid date is assigned
                    ShowTimes = "18:30", // Changed from TimeSpan to string
                    CinemaID = 1,
                    RoomID = 1,
                    MovieID = 1
                },
                new ShowTime
                {
                    ShowTimeID = 2,
                    ShowDates = "01/03/2025", // Ensure a valid date is assigned
                    ShowTimes = "20:15", // Changed from TimeSpan to string
                    CinemaID = 2,
                    RoomID = 2,
                    MovieID = 2
                }
            );

        }
    }
}
