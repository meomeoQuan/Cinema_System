using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cinema.DataAccess.Repository;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>

    {
        //public readonly ApplicationUserRepository _applicationUserRepository;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //_applicationUserRepository = new ApplicationUserRepository(this);
        }

        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderTable> OrderTables { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }

        public DbSet<ShowtimeSeat> showTimeSeats { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<ShowTime> showTimes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FoodSelectionVM>().HasNoKey();
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");

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
                .HasOne(r => r.Theater)
                .WithMany(c => c.Rooms)
                .HasForeignKey(r => r.CinemaID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion

            // Configure Theater Table
            modelBuilder.Entity<Theater>()
                .Property(t => t.Status)
                .HasConversion<string>(); // Store enum as string

            modelBuilder.Entity<ApplicationUser>().HasData(
                 new ApplicationUser
                 {
                      FullName = "Đào Duy Quý",
                      Role = "Admin",
                      Id = "a1234567-b89c-40d4-a123-456789abcdef", // ✅ Giá trị cố định
                      AccessFailedCount = 0,
                      ConcurrencyStamp = "a1234567-b89c-40d4-a123-456789abcdef", // ✅ Giá trị cố định
                      Email = "daoduyquylop97@gmail.com",
                      EmailConfirmed = true,
                      LockoutEnabled = true,
                      NormalizedEmail = "daoduyquylop97@gmail.com",
                      NormalizedUserName = "Đào Duy Quý",
                      PasswordHash = "AQAAAAEAACcQAAAAEJ9",
                      PhoneNumber = "0123456789",
                      PhoneNumberConfirmed = true,
                      SecurityStamp = "a1234567-b89c-40d4-a123-456789abcdef", // ✅ Giá trị cố định
                      TwoFactorEnabled = false,
                      UserName = "Đào Duy Quý",
                      UserImage = "/css/images/user.png",
                      Points = 0
                  }

             );

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
                    Price = 89000,
                    Quantity = 50,
                    ProductImage = "/css/images/popcorn.png"
                },
                new Product
                {
                    ProductID = 2,
                    Name = "Soda",
                    Description = "Refreshing cold soda, 500ml.",
                    ProductType = ProductType.Drink,
                    Price = 39000,
                    Quantity = 100,
                    ProductImage = "/css/images/soda.png"
                },
                new Product
                {
                    ProductID = 3,
                    Name = "Coca",
                    Description = "Refreshing cold soda, 500ml.",
                    ProductType = ProductType.Drink,
                    Price = 39000,
                    Quantity = 100,
                    ProductImage = "/css/images/drink2.png"
                },
                new Product
                {
                    ProductID = 4,
                    Name = "Sprite",
                    Description = "Refreshing cold soda, 500ml.",
                    ProductType = ProductType.Drink,
                    Price = 39000,
                    Quantity = 100,
                    ProductImage = "/css/images/drink1.png"
                },
                new Product
                {
                    ProductID = 5,
                    Name = "Combo Couple",
                    Description = "Refreshing cold soda, 500ml.",
                    ProductType = ProductType.Gift,
                    Price = 129000,
                    Quantity = 100,
                    ProductImage = "/css/images/popcorn1.png"
                },
                new Product
                {
                    ProductID = 6,
                    Name = "Combo Full",
                    Description = "Refreshing cold soda, 500ml.",
                    ProductType = ProductType.Gift,
                    Price = 229000,
                    Quantity = 100,
                    ProductImage = "/css/images/popcorn2.png"
                }
            );

            // Seed Sample Theaters
            //var adminUser = _applicationUserRepository.FindAdminUserByName("Admin");
            modelBuilder.Entity<Theater>().HasData(
                new Theater
                {
                    CinemaID = 1,
                    Name = "Grand Cinema",
                    Address = "123 Main St, Da Nang City",
                    CinemaCity = "Danang",
                    NumberOfRooms = 5,
                    Status = CinemaStatus.Open,

                    OpeningTime = new TimeSpan(9, 0, 0),  // Changed from TimeSpan to string
                    ClosingTime = new TimeSpan(23, 0, 0),  // Changed from TimeSpan to string

                    AdminID = "a1234567-b89c-40d4-a123-456789abcdef"
                },
                new Theater
                {
                    CinemaID = 2,
                    Name = "Skyline Theater",
                    Address = "456 Broadway Ave, HCM City",
                    CinemaCity = "Ho Chi Minh",
                    NumberOfRooms = 7,
                    Status = CinemaStatus.Open,
                    OpeningTime = new TimeSpan(9, 0, 0),  // Changed from TimeSpan to string
                    ClosingTime = new TimeSpan(23, 0, 0),  // Changed from TimeSpan to string


                },
                new Theater
                {
                    CinemaID = 3,
                    Name = "CGV Cinema",
                    Address = "124 Main St, Danang City",
                    CinemaCity = "Danang",
                    NumberOfRooms = 5,
                    Status = CinemaStatus.Open,
                    OpeningTime = new TimeSpan(9, 0, 0),  // Changed from TimeSpan to string
                    ClosingTime = new TimeSpan(23, 0, 0),  // Changed from TimeSpan to string


                },
                new Theater
                {
                    CinemaID = 4,
                    Name = "HCM Cinestar Cinema",
                    Address = "124 Main St, HCM City",
                    CinemaCity = "Ho Chi Minh",
                    NumberOfRooms = 5,
                    Status = CinemaStatus.Open,
                    OpeningTime = new TimeSpan(9, 0, 0),  // Changed from TimeSpan to string
                    ClosingTime = new TimeSpan(23, 0, 0),  // Changed from TimeSpan to string


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
            // Seed Seats for RoomID = 1 (5 rows x 10 columns = 50 seats)
            modelBuilder.Entity<Seat>().HasData(
                // Row A
                new Seat { SeatID = 1, Row = "A", ColumnNumber = 1, RoomID = 1, Status = SeatStatus.Maintenance },
                new Seat { SeatID = 2, Row = "A", ColumnNumber = 2, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 3, Row = "A", ColumnNumber = 3, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 4, Row = "A", ColumnNumber = 4, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 5, Row = "A", ColumnNumber = 5, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 6, Row = "A", ColumnNumber = 6, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 7, Row = "A", ColumnNumber = 7, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 8, Row = "A", ColumnNumber = 8, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 9, Row = "A", ColumnNumber = 9, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 10, Row = "A", ColumnNumber = 10, RoomID = 1, Status = SeatStatus.Available },

                // Row B
                new Seat { SeatID = 11, Row = "B", ColumnNumber = 1, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 12, Row = "B", ColumnNumber = 2, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 13, Row = "B", ColumnNumber = 3, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 14, Row = "B", ColumnNumber = 4, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 15, Row = "B", ColumnNumber = 5, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 16, Row = "B", ColumnNumber = 6, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 17, Row = "B", ColumnNumber = 7, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 18, Row = "B", ColumnNumber = 8, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 19, Row = "B", ColumnNumber = 9, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 20, Row = "B", ColumnNumber = 10, RoomID = 1, Status = SeatStatus.Available },

                // Row C
                new Seat { SeatID = 21, Row = "C", ColumnNumber = 1, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 22, Row = "C", ColumnNumber = 2, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 23, Row = "C", ColumnNumber = 3, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 24, Row = "C", ColumnNumber = 4, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 25, Row = "C", ColumnNumber = 5, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 26, Row = "C", ColumnNumber = 6, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 27, Row = "C", ColumnNumber = 7, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 28, Row = "C", ColumnNumber = 8, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 29, Row = "C", ColumnNumber = 9, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 30, Row = "C", ColumnNumber = 10, RoomID = 1, Status = SeatStatus.Available },

                // Row D
                new Seat { SeatID = 31, Row = "D", ColumnNumber = 1, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 32, Row = "D", ColumnNumber = 2, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 33, Row = "D", ColumnNumber = 3, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 34, Row = "D", ColumnNumber = 4, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 35, Row = "D", ColumnNumber = 5, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 36, Row = "D", ColumnNumber = 6, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 37, Row = "D", ColumnNumber = 7, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 38, Row = "D", ColumnNumber = 8, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 39, Row = "D", ColumnNumber = 9, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 40, Row = "D", ColumnNumber = 10, RoomID = 1, Status = SeatStatus.Available },

                // Row E
                new Seat { SeatID = 41, Row = "E", ColumnNumber = 1, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 42, Row = "E", ColumnNumber = 2, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 43, Row = "E", ColumnNumber = 3, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 44, Row = "E", ColumnNumber = 4, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 45, Row = "E", ColumnNumber = 5, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 46, Row = "E", ColumnNumber = 6, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 47, Row = "E", ColumnNumber = 7, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 48, Row = "E", ColumnNumber = 8, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 49, Row = "E", ColumnNumber = 9, RoomID = 1, Status = SeatStatus.Available },
                new Seat { SeatID = 50, Row = "E", ColumnNumber = 10, RoomID = 1, Status = SeatStatus.Available }
            );


            // Seed ShowTimes
            modelBuilder.Entity<ShowTime>().HasData(
                new ShowTime
                {
                    ShowTimeID = 1,
                    ShowDate = new DateOnly(2025, 3, 10),
                    ShowTimes = new TimeSpan(7, 3, 0),
                    RoomID = 1,
                    MovieID = 1
                },
                new ShowTime
                {
                    ShowTimeID = 2,
                    ShowDate = new DateOnly(2025, 3, 10),
                    ShowTimes = new TimeSpan(9, 3, 0),
                    RoomID = 2,
                    MovieID = 3
                },
                new ShowTime
                {
                    ShowTimeID = 3,
                    ShowDate = new DateOnly(2025, 3, 10),
                    ShowTimes = new TimeSpan(11, 3, 0),
                    RoomID = 1,
                    MovieID = 1
                },
                new ShowTime
                {
                    ShowTimeID = 4,
                    ShowDate = new DateOnly(2025, 3, 10),
                    ShowTimes = new TimeSpan(13, 3, 0),
                    RoomID = 1,
                    MovieID = 1
                },
                new ShowTime
                {
                    ShowTimeID = 5,
                    ShowDate = new DateOnly(2025, 3, 11),
                    ShowTimes = new TimeSpan(15, 3, 0),
                    RoomID = 1,
                    MovieID = 1
                },
                new ShowTime
                {
                    ShowTimeID = 6,
                    ShowDate = new DateOnly(2025, 3, 11),
                    ShowTimes = new TimeSpan(17, 3, 0),
                    RoomID = 2,
                    MovieID = 1
                },
                new ShowTime
                {
                    ShowTimeID = 7,
                    ShowDate = new DateOnly(2025, 3, 11),
                    ShowTimes = new TimeSpan(18, 3, 0),
                    RoomID = 2,
                    MovieID = 1
                },
                new ShowTime
                {
                    ShowTimeID = 8,
                    ShowDate = new DateOnly(2025, 3, 12),
                    ShowTimes = new TimeSpan(19, 3, 0),
                    RoomID = 1,
                    MovieID = 1
                }
            );

            // Seed ShowtimeSeats for RoomID = 1 and ShowTimeID = 1
            modelBuilder.Entity<ShowtimeSeat>().HasData(
                 Enumerable.Range(1, 50).Select(seatId => new ShowtimeSeat
                 {
                     ShowtimeSeatID = seatId,  // Unique ID for each ShowtimeSeat
                     ShowtimeID = 1,           // Link to ShowTimeID = 1
                     SeatID = seatId,          // Each seat (1-50)
                     Price = 80000,            // Fixed price for all seats
                     Status = ShowtimeSeatStatus.Available,
                 }).ToArray()
             );

            // Helper method to check if a seat falls within the VIP area
            bool IsVipSeat(int seatId)
            {
                var vipSeats = new HashSet<int>
                {
                    // B3 to B8
                    13, 14, 15, 16, 17, 18,
                    // C3 to C8
                    23, 24, 25, 26, 27, 28,
                    // D3 to D8
                    33, 34, 35, 36, 37, 38
                };

                return vipSeats.Contains(seatId);
            }


            // Seed OrderTables
            modelBuilder.Entity<OrderTable>().HasData(
                new OrderTable
                {
                    OrderID = 1,
                    Status = OrderStatus.Completed,
                    TotalAmount = 124235,
                    CreatedAt = new DateTime(2025, 1, 1),
                    UserID = "a1234567-b89c-40d4-a123-456789abcdef"
                },
                new OrderTable
                {
                    OrderID = 2,
                    Status = OrderStatus.Pending,
                    TotalAmount = 747237.654,
                    CreatedAt = new DateTime(2025, 2, 1),
                    UserID = "a1234567-b89c-40d4-a123-456789abcdef"
                },
                new OrderTable
                {
                    OrderID = 3,
                    Status = OrderStatus.Completed,
                    TotalAmount = 50000,
                    CreatedAt = new DateTime(2025, 3, 1),
                    UserID = "a1234567-b89c-40d4-a123-456789abcdef"
                }
                //new OrderTable
                //{
                //    OrderID = 4,
                //    Status = OrderStatus.Pending,
                //    TotalAmount = 60000,
                //    CreatedAt = new DateTime(2025, 4, 1)
                //    //UserID = "2d595a04-e0b7-40f6-806c-a5c587b8d638"
                //},
                //new OrderTable
                //{
                //    OrderID = 5,
                //    Status = OrderStatus.Completed,
                //    TotalAmount = 70000,
                //    CreatedAt = new DateTime(2025, 5, 1)
                //    //UserID = "2d595a04-e0b7-40f6-806c-a5c587b8d638"
                //},
                //new OrderTable
                //{
                //    OrderID = 6,
                //    Status = OrderStatus.Pending,
                //    TotalAmount = 80000,
                //    CreatedAt = new DateTime(2025, 6, 1)
                //    //UserID = "2d595a04-e0b7-40f6-806c-a5c587b8d638"
                //},
                //new OrderTable
                //{
                //    OrderID = 7,
                //    Status = OrderStatus.Completed,
                //    TotalAmount = 90000,
                //    CreatedAt = new DateTime(2025, 7, 1)
                //    //UserID = "2d595a04-e0b7-40f6-806c-a5c587b8d638"
                //},
                //new OrderTable
                //{
                //    OrderID = 8,
                //    Status = OrderStatus.Pending,
                //    TotalAmount = 100000,
                //    CreatedAt = new DateTime(2025, 8, 1)
                //    //UserID = "2d595a04-e0b7-40f6-806c-a5c587b8d638"
                //},
                //new OrderTable
                //{
                //    OrderID = 9,
                //    Status = OrderStatus.Completed,
                //    TotalAmount = 110000,
                //    CreatedAt = new DateTime(2025, 9, 1)
                //    //UserID = "2d595a04-e0b7-40f6-806c-a5c587b8d638"
                //},
                //new OrderTable
                //{
                //    OrderID = 10,
                //    Status = OrderStatus.Pending,
                //    TotalAmount = 120000,
                //    CreatedAt = new DateTime(2025, 10, 1)

                //    //UserID = "1c6efd5e-0104-4967-86b4-b7549a322819"
                //},
                //new OrderTable
                //{
                //    OrderID = 11,
                //    Status = OrderStatus.Completed,
                //    TotalAmount = 130000,
                //    CreatedAt = new DateTime(2025, 11, 1)

                //    //UserID = "1c6efd5e-0104-4967-86b4-b7549a322819"
                //},
                //new OrderTable
                //{
                //    OrderID = 12,
                //    Status = OrderStatus.Pending,
                //    TotalAmount = 140000,
                //    CreatedAt = new DateTime(2025, 12, 1)

                //    //UserID = "1c6efd5e-0104-4967-86b4-b7549a322819"
                //}
            );


            //Seed OrderDetails
            //modelBuilder.Entity<OrderDetail>().HasData(
            //    new OrderDetail
            //    {
            //        OrderDetailID = 1,
            //        OrderID = 1,
            //        Quantity = 2,
            //        Price = 10.0,
            //        ProductID = 1,


            //    },
            //    new OrderDetail
            //    {
            //        OrderDetailID = 2,
            //        OrderID = 1,
            //        Price = 15.0,
            //        ShowtimeSeatID = 1

            //    }
            //);
            // Seed OrderDetails
            //modelBuilder.Entity<OrderDetail>().HasData(
            //    new OrderDetail
            //    {
            //        OrderDetailID = 1,
            //        OrderID = 1,
            //        Quantity = 2,
            //        Price = 10.0,
            //        ProductID = 1,
            //        ShowtimeSeatID = null // Explicitly set to null
            //    },
            //    new OrderDetail
            //    {
            //        OrderDetailID = 2,
            //        OrderID = 1,
            //        Quantity = 1,
            //        Price = 15.0,
            //        ProductID = null,
            //        ShowtimeSeatID = 1
            //    },
            //    new OrderDetail
            //    {
            //        OrderDetailID = 3,
            //        OrderID = 2,
            //        Quantity = 1,
            //        Price = 20.0,
            //        ProductID = 2,
            //        ShowtimeSeatID = null
            //    },
            //    new OrderDetail
            //    {
            //        OrderDetailID = 4,
            //        OrderID = 2,
            //        Quantity = 1,
            //        Price = 25.0,
            //        ProductID = null,
            //        ShowtimeSeatID = 2
            //    },
            //    new OrderDetail
            //    {
            //        OrderDetailID = 5,
            //        OrderID = 3,
            //        Quantity = 3,
            //        Price = 30.0,
            //        ProductID = 3,
            //        ShowtimeSeatID = null
            //    }
            //);


        }
    }
}
